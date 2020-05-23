using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                ;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name:"v1", new OpenApiInfo{Title = "Generate Random Data API", Version = "v1"});
            });

            services.Configure<MailServerConfig>(Configuration.GetSection("mailserver"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IOptions<MailServerConfig> mailServerConfigAccessor)
        {
            app.UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Generate Random Data API V1"); })
                .UseRewriter(new RewriteOptions()
                    .AddRedirect("^$", "swagger")
                )
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());

            var mailServerConfig = mailServerConfigAccessor.Value;
            Console.WriteLine($"Mail server: {mailServerConfig.Host}:{mailServerConfig.Port}");
        }
    }
}