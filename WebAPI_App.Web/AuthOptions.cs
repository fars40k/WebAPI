using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_App.Web
{
    public class AuthOptions
    {
        public const string ISSUER = "WebAPI"; 
        public const string AUDIENCE = "Win_Dev"; 
        const string KEY = "secretkey!123_494473330";
        public const int LIFETIME = 60;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
