using System.Collections.Generic;
using System.IO;

namespace GitHubPush.Config
{
    public class ConfigFile
    {
        /// <summary>
        /// 主要配置文件
        /// </summary>
        public static FileInfo 主要配置文件 { get; set; }
    }
    public class MainConfig
    {
        public List<long> 推送群号 { get; set; } = new List<long> { };
        public Dictionary<string, List<long>> 特殊推送 { get; set; } = new Dictionary<string, List<long>>();
        public string 服务器地址 { get; set; } = "127.0.0.1";
        public int 服务器端口 { get; set; } = 25555;
        public long 机器人QQ号 { get; set; } = 123456789;
    }
}
