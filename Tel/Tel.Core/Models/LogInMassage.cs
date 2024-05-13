using System.Collections.Generic;

namespace Tel.Core.Models.Massage
{
    public class LogInMassage : TunnelMassage
    {
        /// <summary>
        /// web穿透隧道列表
        /// </summary>
        public IEnumerable<WebConfig> Webs { get; set; }

        /// <summary>
        /// 端口转发隧道列表
        /// </summary>
        public IEnumerable<ForwardConfig> Forwards { get; set; }
    }
}
