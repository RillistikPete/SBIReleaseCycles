using StdBdgRCCL.Infrastructure.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure.ClientBase
{
    public class SetClients
    {
        public static string _edfiToken { get; set; } = "";
        private static long _edfiTokenExpiration { get; set; }
        public static string _icToken { get; set; } = "";
        private static long _icTokenExpiration { get; set; }

        public static async Task FillTokens()
        {
            var token = await Authorization.GetEdFiToken();
            _edfiToken = token.AccessToken;
            _edfiTokenExpiration = token.ExpiresIn;

            var icToken = await Authorization.GetICToken();
            _icToken = icToken.AccessToken;
            _icTokenExpiration = icToken.ExpiresIn;
            return;
        }
    }
}
