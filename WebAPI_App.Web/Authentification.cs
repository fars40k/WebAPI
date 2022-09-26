using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_App.Web
{
    public class Authentification
    {
        public static Dictionary<string, string> _accountTokens = new Dictionary<string, string>();

        const string KEY = "kfryfhkey!123_494473330";

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
