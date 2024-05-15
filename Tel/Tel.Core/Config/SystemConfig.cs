// Tel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Tel.Core.Config;
public static class SystemConfig
{
    public static ConfigInfo<IpInfo> IpInfos = new ConfigInfo<IpInfo>("ipinfos.json");
}
public class IpInfo
{
    public List<string> Names { get; set; } = new List<string>();
    public List<string> Ips { get; set; } = new List<string>();
    public List<string> disIps { get; set; } = new List<string>();
}
