using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = feature.Error;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 400;
        
        var messages = new List<Exception>() { new Exception(exception.Message) };
  
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { messages = messages }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}