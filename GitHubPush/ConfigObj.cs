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
        public List<long> 推送群号 { get; set; }
        public Dictionary<string, List<long>> 特殊推送 { get; set; }
        public string 服务器地址 { get; set; }
        public int 服务器端口 { get; set; }
        public long 机器人QQ号 { get; set; }
        public bool 自动重连 { get; set; }
        public int 重连时间 { get; set; }
        public string 机器人IP { get; set; }
        public int 机器人端口 { get; set; }
    }
}
