﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Faker;
using Faker.Extensions;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace api.Controllers
{
    // Just use action name as route
    [Route("[action]")]
    [ApiController]
    public class GenerateController : ControllerBase
    {
        private readonly string MailHost;
        private readonly int MailPort;

        public GenerateController(IOptions<MailServerConfig> mailServerConfigAccessor)
        {
            var config = mailServerConfigAccessor.Value;
            MailHost = config.Host;
            MailPort = config.Port;
        }
        
        [HttpPost]
        public async Task EmailRandomNames(Range range, string email = "test@fake.com")
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Generator", "generator@generate.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Here are your random names!";

            message.Body = new TextPart("plain")
            {
                Text = string.Join(Environment.NewLine, range.Of(Name.FullName))
            };
            using (var mailClient = new SmtpClient())
            {
                await mailClient.ConnectAsync(MailHost, MailPort, SecureSocketOptions.None);
                await mailClient.SendAsync(message);
                await mailClient.DisconnectAsync(true);
            }
        }
        
        [HttpGet]
        public IEnumerable<string> Names([FromQuery]Range range)
            => range.Of(Name.FullName);

        [HttpGet]
        public IEnumerable<string> PhoneNumbers([FromQuery]Range range)
            => range.Of(Phone.Number);

        [HttpGet]
        public IEnumerable<int> Numbers([FromQuery]Range range)
            => range.Of(RandomNumber.Next);

        [HttpGet]
        public IEnumerable<string> Companies([FromQuery]Range range)
            => range.Of(Company.Name);

        [HttpGet]
        public IEnumerable<string> Paragraphs([FromQuery]Range range)
            => range.Of(() => Lorem.Paragraph(3));

        [HttpGet]
        public IEnumerable<string> CatchPhrases([FromQuery]Range range)
            => range.Of(Company.CatchPhrase);

        [HttpGet]
        public IEnumerable<string> Marketing([FromQuery]Range range)
            => range.Of(Company.BS);

        [HttpGet]
        public IEnumerable<string> Emails([FromQuery]Range range)
            => range.Of(Internet.Email);

        [HttpGet]
        public IEnumerable<string> Domains([FromQuery]Range range)
            => range.Of(Internet.DomainName);
    }
}
