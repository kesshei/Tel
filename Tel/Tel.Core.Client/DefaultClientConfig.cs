using Tel.Core.Client;
using Tel.Core.Models;
using System.Collections.Generic;

namespace Tel.Core.Config
{
    public class DefaultClientConfig : IClientConfig
    {
        public SuiDaoServer Server { get; set; }

        public string Token { get; set; }

        public IEnumerable<WebConfig> Webs { get; set; }

        public IEnumerable<ForwardConfig> Forwards { get; set; }
    }
}
