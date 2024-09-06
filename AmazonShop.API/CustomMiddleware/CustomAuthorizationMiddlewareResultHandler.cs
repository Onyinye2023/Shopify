using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace AmazonShop.API.CustomMiddleware
{
    // A method to overide the default behaviour ASP.Net Core Authorization middleware 
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
           
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);

            
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Message = "Access denied. You do not have permission to perform this action."
                }));
            }
        }
    }

}
