using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Quartzmin.Jobs
{
    public static class SchedulerContextExtions
    {
        public static IHttpClientFactory GetHttpClientFactory(this SchedulerContext context)
        {
            var httpClientFactory = context.Get(typeof(IHttpClientFactory).FullName) as IHttpClientFactory;
            if(httpClientFactory==null)
            {
                var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
                httpClientFactory=serviceProvider.GetRequiredService<IHttpClientFactory>();

                context.Put(typeof(IHttpClientFactory).FullName, httpClientFactory);
            }
            return httpClientFactory;
        }
    }
}
