using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI_App.Data;
using WebAPI_App.Web.Middleware;

namespace WebAPI_App.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private DataAccessObject _dataAccessObject;
        private IConfiguration _configuration;

        private List<string> roles = new List<string>
        {
            "Admin"
        };

        public AccountController(DataAccessObject dataAccessObject, IConfiguration configuration)
        {
            _dataAccessObject = dataAccessObject;
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

            if (Authentification.AccountTokens.ContainsKey(username))
            {

                Authentification.AccountTokens[username] = encodedJwt;

            } else
            {

                Authentification.AccountTokens.Add(username, encodedJwt);

            }

            return new JsonResult(response);

        }

        [HttpPost("/Register")]
        public IActionResult Register(string username, string password)
        {
            string role = roles[0];
            string newCredentials = "Credentials." + username + "." + password + "." + role;

            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(newCredentials));

            _dataAccessObject.Credentials.Insert(new Credentials(bytes));

            return new JsonResult(null) { StatusCode = 201 };
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            try
            {
                List<Credentials> list = _dataAccessObject.Credentials.FindAll();

                string role = roles[0];
                string newCredentials = "Credentials." + username + "." + password + "." + role;

                SHA256 sha256Hash = SHA256.Create();
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(newCredentials));

                if (list.Any(p => p.Entry.SequenceEqual(bytes)))
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, username),

                };
                    ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }

            }
            catch (Exception ex)
            {

            }

            return null;
        }

    }
}