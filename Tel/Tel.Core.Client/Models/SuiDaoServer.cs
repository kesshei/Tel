using Tel.Core.Models;
using System.Collections.Generic;

namespace Tel.Core.Config
{
    public class SuiDaoServer
    {
        public string Protocol { get; set; } = "ws";

        public string ServerAddr { get; set; }

        public int ServerPort { get; set; }
    }
}
