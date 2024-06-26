using System;
using System.Collections.Generic;

namespace Tel.Core.Config
{
    public class DefaultServerConfig : IServerConfig
    {
        public string WebDomain { get; set; }

        public string[] WebAllowAccessIps { get; set; }
        public string[] ForwardAllowAccessIps { get; set; } 

        public bool EnableForward { get; set; }

        public List<string> Tokens { get; set; } = new List<string>();

        public ApiOptions Api { get; set; }

        public class ApiOptions
        {
            public JWTOptions JWT { get; set; }

            public Account[] Accounts { get; set; }
        }

        public class JWTOptions
        {
            public int ClockSkew { get; set; }

            public string ValidAudience { get; set; }

            public string ValidIssuer { get; set; }

            public string IssuerSigningKey { get; set; }

            public int Expires { get; set; }
        }

        public class Account
        {
            public string Name { get; set; }

            public string Password { get; set; }
        }
    }
}
