using ColoryrSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GitHubPush;

class Program
{
    public const string Version = "1.3.0";
    public static RobotSDK robot { get; private set; } = new();
    /// <summary>
    /// 配置文件路径
    /// </summary>
    public static string Path { get; } = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
    /// <summary>
    /// 已经启动
    /// </summary>
    public static bool IsStart = false;
    /// <summary>
    /// 主配置文件
    /// </summary>
    public static MainConfig MainConfig { get; set; }

    /// <summary>
    /// http服务器
    /// </summary>
    public static HttpListener listener = new();

    private static void ContextReady(IAsyncResult ar)
    {
        listener.BeginGetContext(ContextReady, null);
        AcceptAsync(listener.EndGetContext(ar));
    }
    class Git
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }
        public repository repository { get; set; }
        public pusher pusher { get; set; }
        public head_commit head_commit { get; set; }
    }
    class pusher
    {
        public string name { get; set; }
    }
    class repository
    {
        public string html_url { get; set; }
    }
    class head_commit
    {
        public string id { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }
    }

    public static string GetString(string a)
    {
        var index = a.LastIndexOf('/');
        return a.Substring(index + 1, a.Length - index - 1);
    }

    private static void AcceptAsync(HttpListenerContext context)
    {
        try
        {
            HttpListenerRequest request = context.Request;
            switch (request.HttpMethod)
            {
                case "POST":
                    {
                        using Stream stream = context.Request.InputStream;
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        var content = reader.ReadToEnd();
                        context.Response.StatusCode = 200;
                        var data1 = Encoding.UTF8.GetBytes("OK");
                        context.Response.OutputStream.Write(data1, 0, data1.Length);
                        context.Response.Close();
                        Task.Run(() =>
                        {

                            var obj = JObject.Parse(content).ToObject<Git>();
                            if (obj.Ref == null)
                                return;
                            Logs.LogWrite("发现推送:" + obj.repository.html_url + "|" + obj.head_commit.id);
                            string message = "GitHub仓库更新了!\n";
                            message += obj.repository.html_url + "\n";
                            message += "分支:" + obj.Ref + "\n";
                            message += "更新者:" + obj.pusher.name + "\n";
                            message += "id:" + obj.head_commit.id + "\n";
                            message += "消息:" + obj.head_commit.message + "\n";
                            message += "时间:" + obj.head_commit.timestamp;
                            var data2 = GetString(obj.repository.html_url);
                            if (MainConfig.GroupT.ContainsKey(data2))
                            {
                                var list = MainConfig.GroupT[data2];
                                foreach (var item in list)
                                {
                                    robot.AddSend(new SendGroupMessagePack
                                    {
                                        qq = robot.QQs[0],
                                        id = item,
                                        message = new()
                                        {
                                            message
                                        }
                                    }, 52);
                                }
                            }
                            else
                            {
                                foreach (var item in MainConfig.Group)
                                {
                                    robot.AddSend(new SendGroupMessagePack
                                    {
                                        qq = robot.QQs[0],
                                        id = item,
                                        message = new()
                                        {
                                            message
                                        }
                                    }, 52);
                                }
                            }
                        });
                    }
                    break;
                case "GET":
                    {
                        var data = request.QueryString;
                        context.Response.StatusCode = 200;
                        var data1 = Encoding.UTF8.GetBytes("OK");
                        context.Response.OutputStream.Write(data1, 0, data1.Length);
                        context.Response.Close();
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Logs.LogError(e);
        }
    }

    static void Main(string[] args)
    {
        MainConfig = ConfigUtils.Config(new MainConfig
        {
            Group = new(),
            GroupT = new() { { "test", new() { 123456789 } } },
            Port = 25555,
            BotIP = "127.0.0.1",
            QQ = 123456789,
            BotPort = 23335,
            AutoConnect = true,
            Time = 10000
        }, Path + "config.json");

        try
        {
            listener.Prefixes.Add("http://+:" + MainConfig.Port + "/");
            listener.Start();
            listener.BeginGetContext(ContextReady, null);
            IsStart = true;
            Logs.LogWrite("已启动");
        }
        catch (Exception e)
        {
            Logs.LogError(e);
        }

        RobotConfig config = new()
        {
            IP = MainConfig.BotIP,
            Port = MainConfig.BotPort,
            Name = "GitHubPush",
            Pack = new() { },
            RunQQ = MainConfig.QQ,
            Time = MainConfig.Time,
            Check = MainConfig.AutoConnect,
            CallAction = (a,b) => { },
            LogAction = (type,data) => { Logs.LogWrite($"日志:{type} {data}"); },
            StateAction = (type) => { Logs.LogWrite($"日志:{type}"); }
        };

        robot.Set(config);
        robot.SetPipe(new ColorMiraiNetty(robot));
        robot.Start();

        while (!robot.IsConnect) ;
        if (!MainConfig.NoInput)
        {
            while (true)
            {
                string data = Console.ReadLine();
                if (data == "stop")
                {
                    IsStart = false;
                    listener.Stop();
                    robot.Stop();
                }
            }
        }
    }
}
