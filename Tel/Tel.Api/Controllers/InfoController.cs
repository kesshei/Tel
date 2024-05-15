using Tel.Core.Client;
using TelServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    [HttpPost]
    public object AddIP(string username,string ip)
    {
        return "";
    }
}
