using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domains.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using WebAPI.Middlewares;

namespace WebAPI.Tests.Middlewares
{
    public class GlobalExceptionMiddlewareTest : SetupTest
    {
        private readonly GlobalExceptionMiddlewareV2 middleware;
        private readonly DefaultHttpContext defaultContext;
        public GlobalExceptionMiddlewareTest()
        {
            defaultContext = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => Task.FromException(new Exception("TumlumTumla"));
            middleware = new GlobalExceptionMiddlewareV2(next, _loggerException.Object);
        }

    

        [Fact]
        public async Task MiddlewareTest_ShouldPass() 
        {
            using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(service => {
                        service.AddWebAPIService("Xinchao C#");
                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<GlobalExceptionMiddlewareV2>();
                        app.UseMiddleware<PerformanceMiddleware>();
                    });
            })
            .StartAsync();
               
                await host.GetTestClient().GetAsync("/");
        }

        [Fact]
        public async Task MiddlewareTest_ShouldThrowException()
        {
            await middleware.InvokeAsync(defaultContext);

        }

     


       

    }
}