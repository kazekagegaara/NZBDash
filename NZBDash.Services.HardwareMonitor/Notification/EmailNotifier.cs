﻿#region Copyright
// /************************************************************************
//   Copyright (c) 2016 NZBDash
//   File: EmailNotifier.cs
//   Created By: Jamie Rees
//  
//   Permission is hereby granted, free of charge, to any person obtaining
//   a copy of this software and associated documentation files (the
//   "Software"), to deal in the Software without restriction, including
//   without limitation the rights to use, copy, modify, merge, publish,
//   distribute, sublicense, and/or sell copies of the Software, and to
//   permit persons to whom the Software is furnished to do so, subject to
//   the following conditions:
//  
//   The above copyright notice and this permission notice shall be
//   included in all copies or substantial portions of the Software.
//  
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//   OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//   WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ************************************************************************/
#endregion
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

using HtmlAgilityPack;

using NZBDash.Core.Interfaces;
using NZBDash.Core.Models;
using NZBDash.Core.Models.Settings;
using NZBDash.Services.HardwareMonitor.Interfaces;

using RazorEngine;
using RazorEngine.Templating;


namespace NZBDash.Services.HardwareMonitor.Notification
{
    class EmailNotifier : INotifier
    {
        public EmailNotifier(TimeSpan interval, IEventService eventService, CpuMonitoringDto dto, EmailAlertSettingsDto emailDto, ISmtpClient client)
        {
            Interval = interval;
            EventService = eventService;
            CpuSettings = dto;
            EmailSettings = emailDto;
            SmtpClient = client;
        }

        private ISmtpClient SmtpClient { get; set; }
        private IEventService EventService { get; set; }
        private CpuMonitoringDto CpuSettings { get; set; }
        private EmailAlertSettingsDto EmailSettings { get; set; }
        private int AlertCount { get; set; }
        private TimeSpan Interval { get; set; }
        private bool SentStartNotification { get; set; }
        private DateTime StartEventTime { get; set; }
        private DateTime EndEventTime { get; set; }

        public void ResetCounter()
        {
            AlertCount = 0;
        }

        private void CheckSend()
        {
            if (AlertCount >= Interval.Seconds && !SentStartNotification)
            {
                StartEventTime = DateTime.Now;
                Console.WriteLine("Saving Start Event");
                SaveEvent();

                if (EmailSettings.AlertOnBreach)
                {
                    SendEmail();
                    Console.WriteLine("SEND OUT TEH EMAILZ");
                    SentStartNotification = true;
                }
            }


            if (AlertCount == 0 && SentStartNotification)
            {
                EndEventTime = DateTime.Now;
                Console.WriteLine("Saving End Event");
                SaveEvent();

                if (EmailSettings.AlertOnBreachEnd)
                {
                    SendEmail();
                    Console.WriteLine("ALERT, IT's ENDED");
                    SentStartNotification = false;
                }

                StartEventTime = DateTime.MinValue;
                EndEventTime = DateTime.MinValue;
            }
        }

        public void Notify(bool critical)
        {
            if (!CpuSettings.Enabled)
            {
                return;
            }
            Console.WriteLine("Current Interval {0}", Interval.Seconds);
            Console.WriteLine("ALERT! {0}", critical);

            if (critical)
            {
                AlertCount++;
            }
            else
            {
                ResetCounter();
            }

            CheckSend();
        }

        public void SaveEvent()
        {
            var dto = new MonitoringEventsDto
            {
                EventName = EventName.CpuEvent,
                EventEnd = EndEventTime,
                EventStart = StartEventTime,
                EventType = EndEventTime == DateTime.MinValue ? EventTypeDto.Start : EventTypeDto.End
            };

            var result = EventService.RecordEvent(dto);

        }

        private void SendEmail()
        {
            var m = new EmailModel
            {
                BreachEnd = EndEventTime,
                BreachStart = StartEventTime,
                TimeThresholdSec = CpuSettings.ThresholdTime,
                Percentage = CpuSettings.CpuPercentageLimit,
                BreachType = "CPU %"
            };
            var body = GenerateHtmlTemplate(m);
            var message = new MailMessage
            {
                To = { EmailSettings.RecipientAddress },
                From = new MailAddress("nzbdash@nzbdash.com", "NZBDash StartAlert"),
                IsBodyHtml = true,
                Body = body
            };
            var creds = new NetworkCredential(EmailSettings.EmailUsername, EmailSettings.EmailPassword);
            SmtpClient.Send(EmailSettings.EmailHost, EmailSettings.EmailPort, message, creds);
            //Logger.Info("StartAlert Email Sent");
        }

        private string GenerateHtmlTemplate(EmailModel model)
        {
            var template = string.Empty;
            template = File.ReadAllText("Email Templates\\Email.html");
            var document = new HtmlDocument();
            document.LoadHtml(template);

            template = document.DocumentNode.OuterHtml;

            template = RemoveBadHtml(template);

            template = Engine.Razor.RunCompile(template, model.BreachType, null, model);

            return template;
        }

        private static string RemoveBadHtml(string text)
        {
            var newRegex = new System.Text.RegularExpressions.Regex("\\r\\n");
            text = newRegex.Replace(text, string.Empty);
            return text.Replace("\\", "\"");
        }
    }
}