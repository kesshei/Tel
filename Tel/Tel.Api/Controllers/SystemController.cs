using Tel.Core.Client;
using TelServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tel.Api.Controllers;

public class SystemController : BaseController
{
    readonly TelCoreServer TelServer;

    public SystemController(TelCoreServer TelServer)
    {
        this.TelServer = TelServer;
    }

    /// <summary>
    /// 获取当前等待响应的请求
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse GetResponseTempList()
    {
        ApiResponse.data = new
        {
            Count = TelServer.ResponseTasks.Count,
            Rows = TelServer.ResponseTasks.Select(x => new
            {
                x.Key
            })
        };

        return ApiResponse;
    }

    /// <summary>
    /// 获取当前映射的所有站点信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse GetAllWebList()
    {
        ApiResponse.data = new
        {
            Count = TelServer.WebList.Count,
            Rows = TelServer.WebList.Select(x => new { x.Key, x.Value.WebConfig.LocalIp, x.Value.WebConfig.LocalPort })
        };

        return ApiResponse;
    }

    /// <summary>
    /// 获取服务端配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse GetServerOption()
    {
        ApiResponse.data = TelServer.ServerOption;
        return ApiResponse;
    }

    /// <summary>
    /// 获取所有端口转发映射列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse GetAllForwardList()
    {
        ApiResponse.data = new
        {
            Count = TelServer.ForwardList.Count,
            Rows = TelServer.ForwardList.Select(x => new { x.Key, x.Value.SSHConfig.LocalIp, x.Value.SSHConfig.LocalPort, x.Value.SSHConfig.RemotePort })

        };

        return ApiResponse;
    }

    /// <summary>
    /// 获取当前客户端在线数量
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse GetOnlineClientCount()
    {
        ApiResponse.data = TelServer.ConnectedClientCount;
        return ApiResponse;
    }
}
