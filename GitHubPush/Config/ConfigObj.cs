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
        public string 服务器地址 { get; set; } = "127.0.0.1";
        public int 服务器端口 { get; set; } = 25555;
    }
}
