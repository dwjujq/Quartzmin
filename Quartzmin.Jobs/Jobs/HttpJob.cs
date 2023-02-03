using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Jobs.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Quartzmin.Jobs.Jobs
{
    public class HttpJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key.ToString();
            var url = context.JobDetail.JobDataMap["url"]?.ToString();
            var queryParams = context.JobDetail.JobDataMap["queryParams"]?.ToString();
            var licenseKey = context.JobDetail.JobDataMap["licenseKey"]?.ToString();

            var result= await JobHttpClient.Create(jobKey, context.Scheduler)
                .WithUrl(url)
                .AddHeader("licenseKey",licenseKey)
                .WithQueryParams(queryParams)
                .GetAsync<Dictionary<string, object>>();

            if (result["Success"]?.To<bool>()==true)
            {
                return;
            }

            var error = result["Error"]?.ToString()??"";
            var message = result["Message"]?.ToString() ?? "";
            var errorCode = result["ErrorCode"].To<int>();

            throw new CustomException($"code is {errorCode},{Environment.NewLine}error is {error},{Environment.NewLine}message is {message}");
        }
    }
}
