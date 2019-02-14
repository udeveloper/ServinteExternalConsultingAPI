using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Salud.Framework.Broker.Core;
using Servinte.Framework.Clinic.BasicInformation.Infraestructure;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Servinte Basic Informacion API", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "Servinte.Framework.Clinic.BasicInformation.API.xml"));
            });
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
            app.UseSwagger();
            app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Servinte Basic Informacion API V1");
                }
            );

            app.UseMvc();
        }
    }
}
