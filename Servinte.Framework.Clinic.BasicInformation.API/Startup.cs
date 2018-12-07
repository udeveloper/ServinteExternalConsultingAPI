﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salud.Framework.Broker.Core;
using Servinte.Framework.Clinic.BasicInformation.Infraestructure;

namespace Servinte.Framework.Clinic.BasicInformation.API
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

            services.AddDbContext<ExternalConsultingContext>(x => x.UseSqlite(Configuration.GetConnectionString("Default")
                , b => b.MigrationsAssembly("Servinte.Framework.Clinic.BasicInformation.API")));
            //services.AddSingleton<IBrokerClient>(new RabbitMQBrokerClient());
            services.AddSingleton<IBrokerClient>(new
                    RabbitMQBrokerClient(Configuration.GetSection("BrokerConnection:ip").Value,
                                         Configuration.GetSection("BrokerConnection:authorization:username").Value,
                                         Configuration.GetSection("BrokerConnection:authorization:password").Value)
                                         );
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            // app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();
        }
    }
}
