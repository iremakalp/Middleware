using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using WebApi.Middlewares;

namespace WebApi
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // middleware'lar genellikle use ile baslar
        // middleware sirasi onemlidir
        // middleware'lar buralarda tanimlanir
        // middleware'lar request pipeline'da sirasiyla calisir
        // app.run() middleware'larin calismasini durdurur
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHelloMiddleware();
            
            //app.Run() 
            //bazi middleware metotlari kendinden sonraki middleware'larin calismasini durdurur
            // app.run kisa devre yaptirir
            // app.Run(async context =>Console.WriteLine("Middleware 1"));
            // app.Run(async context =>Console.WriteLine("Middleware 2"));
            
            // app.Use() 
            // app.Use() kendi islemini yapar ve sonraki middleware'larin calismasina aktarim yapar
            // asenkron bagimsiz calismak demektir

            app.Use(async (context, next)=> {
                Console.WriteLine("Use middleware tetiklendi");
                // invoke() ile sonraki middleware'larin calismasina izin verilir
                await next.Invoke();
                // Console.WriteLine("Middleware 2");
            });

            //app.Map() route'a gore middleware'larin calismasini saglar
            app.Map("/example", internalApp => 
            internalApp.Run(
                async context => 
                {
                    Console.WriteLine("/example middleware tetiklendi");
                    // response body'ye yazma
                    await context.Response.WriteAsync("/example middleware tetiklendi");
                }));

            // app.MapWhen() method'a gore middleware'larin calismasini saglar
            app.MapWhen(context => context.Request.Method=="GET", internalApp =>
            internalApp.Run(
                async context =>
                {
                    Console.WriteLine("GET methodu ile middleware tetiklendi");
                    // response body'ye yazma
                    await context.Response.WriteAsync("GET methodu ile middleware tetiklendi");
                }));
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
