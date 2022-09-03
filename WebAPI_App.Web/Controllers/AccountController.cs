using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI_App.Web.Middleware;

namespace WebAPI_App.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IConfiguration _configuration;

        private List<Account> accounts = new List<Account>
        {
            new Account {Login="admin", Password="12345", Role="Admin"}
        };

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("/SignIn")]
        public IActionResult SignIn(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest();
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: _configuration["AppSettings:JWT_issuer"],
                    audience: _configuration["AppSettings:JWT_audience"],
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(Double.Parse(_configuration["AppSettings:JWT_lifetime"]))),
                    signingCredentials: new SigningCredentials(Authentification.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            if (Authentification._accountTokens.ContainsKey(username))
            {

                Authentification._accountTokens[username] = encodedJwt;

            } else
            {

                Authentification._accountTokens.Add(username, encodedJwt);

            }

            return new JsonResult(response);

        }

        [HttpPost("/Register")]
        public IActionResult Register(string username, string password)
        {
            accounts.Add(new Account() { Login = username, Password = password, Role = "User" });

            return new JsonResult(null) { StatusCode = 201 };
        }

        [HttpPost("/Grant")]
        public IActionResult Grant(string username, string newRole)
        {
            if (accounts.Exists(x => x.Login == username))
            {
                var account = accounts.Find(x => x.Login == username);
                account.Role = newRole;

                return new JsonResult(null) { StatusCode = 202 };

            } else
            {
                return new JsonResult(null) { StatusCode = 404 };
            }
            
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            Account person = accounts.FirstOrDefault(x => x.Login == username && x.Password == password);

            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),

                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }

    }

    public class Account
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}