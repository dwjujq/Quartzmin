using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.MailKitSmtp;
using FluentEmail.Razor;
using Quartz.Impl.Matchers;
using Quartz.Plugins.Nodify.Utils;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Plugins.Nodify
{
    public class NotifyPlugin : ISchedulerPlugin, IJobListener
    {
        IScheduler _scheduler;

        public string Name { get; set; }

        public string EmailFrom { get; set; } = "";

        public string EmailFromName { get; set; } = "";

        public string EmailServer { get; set; } = "";

        public int EmailPort { get; set; } = 25;

        public string EmailUser { get; set; } = "";

        public string EmailPassword { get; set; } = "";

        public Task Initialize(string pluginName, IScheduler scheduler, CancellationToken cancellationToken = default(CancellationToken))
        {
            Name = pluginName;
            _scheduler = scheduler;
            _scheduler.ListenerManager.AddJobListener(this, EverythingMatcher<JobKey>.AllJobs());

            Email.DefaultRenderer = new RazorRenderer();
            Email.DefaultSender = new MailKitSender(new SmtpClientOptions
            {
                Server = EmailServer,
                Port = EmailPort,
                User = EmailUser,
                Password = EmailPassword,
                UseSsl = false,
            });

            return Task.FromResult(0);
        }

        public Task Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task Shutdown(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (jobException == null)
            {
                return;
            }
            var shouldSendEmail = context.JobDetail.JobDataMap["SendEmail"]?.To<int>() == 1;
            if(!shouldSendEmail)
            {
                return;
            }
            if(EmailFrom.IsNullOrWhiteSpace())
            {
                throw new Exception($"{nameof(EmailFrom)} is not set.");
            }

            var email = Email.From(EmailFrom,EmailFromName);

            var emailTo = context.JobDetail.JobDataMap["EmailTo"]?.ToString();
            if(emailTo.IsNullOrWhiteSpace())
            {
                throw new Exception($"EmailTo is not set.");
            }

            var emailToList = emailTo.ToEnumerable<string>().Select(e=>new Address(e));
            email.To(emailToList);

            var emailCC = context.JobDetail.JobDataMap["EmailCC"]?.ToString();
            if(!emailCC.IsNullOrWhiteSpace())
            {
                var emailCCList= emailCC.ToEnumerable<string>().Select(e => new Address(e));
                email.CC(emailCCList);
            }

            var template = "执行任务【@Model.JobKey】失败,@Model.Message";

            await email.Subject($"执行任务【{context.JobDetail.Key}】失败")
                .UsingTemplate(template, new { JobKey = context.JobDetail.Key, Message = jobException.ToString() }).SendAsync();
        }

        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
        }
    }
}