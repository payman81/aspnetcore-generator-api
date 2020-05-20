using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace integration
{
    public class EmailTests
    {
        public const string GeneratorApiRoot = "http://generator";
        public const string MailHogApiV2Root = "http://mail:8025/api/v2";

        [Fact]
        public async Task SendEmailWithNames_IsFromGenerator()
        {
            // send email
            var client = new HttpClient();
            var sendEmail = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{GeneratorApiRoot}/EmailRandomNames"),
                Content = new StringContent("{\"count\": 0,\"sort\": true}", Encoding.UTF8, "application/json")
            };
            //sendEmail.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            Console.WriteLine($"Sending email: {sendEmail.RequestUri}");
            using (var response = await client.SendAsync(sendEmail))
            {
                response.EnsureSuccessStatusCode();
            }

            // check if email
            var checkEmails = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{MailHogApiV2Root}/messages")
            };
            Console.WriteLine($"Checking emails: {checkEmails.RequestUri}");
            using (var response = await client.SendAsync(checkEmails))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var messages = JObject.Parse(content);
                messages.Should().HaveElement("total").Which.Should().BeEquivalentTo(1);
                messages.Should().HaveElement("items")
                    .Which.Should().BeOfType<JArray>()
                    .Which.First.Should().HaveElement("From")
                    .Which.Should().HaveElement("Mailbox");
            }
        }
    }
}