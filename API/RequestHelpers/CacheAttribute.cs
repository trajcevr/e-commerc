using System.Text;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers;

[AttributeUsage(AttributeTargets.All)]
public class CacheAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices
            .GetRequiredService<IResponseCacheService>();
        if (cacheService == null)
        {
            await next(); // Proceed if no cache service is available
            return;
        }

        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
        var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            var contentResult = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };

            context.Result = contentResult;
            return; 
        }

        var executedContext = await next(); 

        if (executedContext.Result is ObjectResult objectResult && 
            objectResult.Value is not null)
        {
            await cacheService.CacheResponseAsync(cacheKey, objectResult.Value, 
                TimeSpan.FromSeconds(timeToLiveSeconds));
        }
    }

    private static string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");

        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}
