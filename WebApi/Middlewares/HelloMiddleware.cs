using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebApi.Middlewares
{
    public class HelloMiddleware
    {
        private readonly RequestDelegate _next;

        // RequestDelegate bir sonraki middleware'i temsil eder
        public HelloMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        // Invoke metodu middleware'larin calismasini saglar
        // her bir middleware next.Invoke ile calisir
        // Task bir async metotdur
        // async kullanilirsa donus tipi Task olmalidir
        public async Task InvokeAsync(HttpContext context)
        {
           Console.WriteLine("Hello from HelloMiddleware");
            // bir sonrakinin invoke metodu cagrilir
            await _next.Invoke(context);
            Console.WriteLine("Bye world from HelloMiddleware");
        }


    }
    
    // extension method ile middleware'larin kullanilmasi kolaylastirilir
    // extension methodlar static olmalidir
    // extension methodlar bir classin icinde tanimlanir
    // extension methodlarin ilk parametresi this keywordu ile tanimlanir
    // extension methodlarin ilk parametresi extension edilecek classin tipi olur

    static public class HelloMiddlewareExtension
    {
        // UseHelloMiddleware metodu IApplicationBuilder tipinde bir nesne dondurur

        public static IApplicationBuilder UseHelloMiddleware(this IApplicationBuilder app){
            return app.UseMiddleware<HelloMiddleware>();
        }
    }
}