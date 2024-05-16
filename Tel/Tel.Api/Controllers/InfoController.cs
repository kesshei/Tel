using Tel.Core.Client;
using TelServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tel.Core.Config;

namespace Tel.Api.Controllers;

public class InfoController : BaseController
{
    /// <summary>
    /// 获取ip地址
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public object Ip()
    {
        var ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        if (string.IsNullOrEmpty(ip))
        {
            ip = HttpContext.Connection.RemoteIpAddress.ToString();
        }
        if (ip == "0.0.0.1")
        {
            ip = "127.0.0.1";
        }
        var data = new
        {
            code = 200,
            ip = ip
        };

        return data;
    }
    public class Info
    {
        public string username { get; set; }
        public string ip { get; set; }


    }
    [HttpPost]
    public object AddIP(Info info)
    {
        SystemConfig.IpInfos.ReRead();
        if ((info?.username == "我是中国人" || SystemConfig.IpInfos.CurrentConfig.Names.Contains(info?.username)) && info?.ip != null)
        {
            if (!SystemConfig.IpInfos.CurrentConfig.Ips.Contains(info.ip))
            {
                SystemConfig.IpInfos.CurrentConfig.Ips.Add(info.ip);
                SystemConfig.IpInfos.Save();
            }
            return new
            {
                code = 200
            };
        }
        return new
        {
            code = -1
        };
    }
}
