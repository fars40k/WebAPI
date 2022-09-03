using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_App.Data;

namespace WebAPI_App.Web.Middleware
{
    public class AccountsMiddleware
    {    

        public readonly List<string> unautorizedPermitRoutesCollection = new List<string>()
        {
            "/SignIn",
            "/api/State",
            "/swagger/index.html",
            "/swagger/v1/swagger.json",
        };

        private RequestDelegate _next;

        public AccountsMiddleware(RequestDelegate next, DataAccessObject obj)
        {
            //Creates singleton DataAccessObject with initial time delay

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

                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(jsonWebToken);

                var list = jwtSecurityToken.Claims.ToList();
                string accountName = list[0].ToString().Substring(list[0].ToString().IndexOf(" ") + 1);

                if (Authentification._accountTokens.ContainsKey(accountName))
                {
                    if (Authentification._accountTokens[accountName] == jsonWebToken)
                    {
                        var signature = jwtSecurityToken.RawSignature;
                        IsAuthorisedRequest = true;
                    }

                }
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
