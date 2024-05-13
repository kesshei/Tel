using Tel.Core.Config;
using Tel.Core.Client;
using Tel.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Tel.Core.Handlers.Client
{
    public interface IClientHandler
    {
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="cleint"></param>
        /// <param name="msg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandlerMsgAsync(TelClient cleint, string msg, CancellationToken cancellationToken);
    }

}
