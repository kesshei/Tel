using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tel.Core.Config;
using Tel.Core.Models;

namespace Tel.Core.Client
{
    public interface IClientConfig
    {
        public SuiDaoServer Server { get; set; }

        public IEnumerable<WebConfig> Webs { get; set; }

        public IEnumerable<ForwardConfig> Forwards { get; set; }
    }

}

