using System.Collections.Generic;
using System.IO;

namespace GitHubPush;

public class ConfigFile
{
    /// <summary>
    /// 主要配置文件
    /// </summary>
    public static FileInfo 主要配置文件 { get; set; }
}
public class MainConfig
{
    public List<long> Group { get; set; }
    public Dictionary<string, List<long>> GroupT { get; set; }
    public int Port { get; set; }
    public long QQ { get; set; }
    public bool AutoConnect { get; set; }
    public int Time { get; set; }
    public string BotIP { get; set; }
    public int BotPort { get; set; }
    public bool NoInput { get; set; }
}
