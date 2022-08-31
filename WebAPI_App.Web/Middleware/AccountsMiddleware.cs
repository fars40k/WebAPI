using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI_App.Web.Middleware
{
    public class AccountsMiddleware
    {
        public readonly List<string> unautorizedPermitRoutesCollection = new List<string>()
        {
            "/token",
            "/api/State",
            "/swagger/index.html",
            "/swagger/v1/swagger.json",
            "/api/Goals"
        };

        private RequestDelegate _next;
        public AccountsMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            bool IsAuthorisedRequest = false;

            StringValues obj;
            context.Request.Headers.TryGetValue("Authorization", out obj);

            string path = context.Request.Path;

            if (obj.Count != 0)
            {
                string jsonWebToken = obj.FirstOrDefault().Substring(7);
            }

            if ((unautorizedPermitRoutesCollection.Contains(path)) || (IsAuthorisedRequest == true))
            {

                await _next.Invoke(context);

            } else
            {

                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("");

            }
          
        }
    }
    
}
