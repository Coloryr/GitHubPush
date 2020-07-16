using GitHubPush.Config;
using GitHubPush.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitHubPush
{
    internal class GitHubPush
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string Path { get; } = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "GitHubPush/";
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
        public static HttpListener listener = new HttpListener();

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

        private static void AcceptAsync(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                switch (request.HttpMethod)
                {
                    case "POST":
                        {
                            Stream stream = context.Request.InputStream;
                            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                            var content = reader.ReadToEnd();
                            //HttpProcessor.HandlePOSTRequest(content, context.Request, context.Response);
                            context.Response.StatusCode = 200;
                            var data1 = Encoding.UTF8.GetBytes("OK");
                            context.Response.OutputStream.Write(data1, 0, data1.Length);
                            context.Response.OutputStream.Close();
                            Task.Run(() =>
                            {

                                var obj = JObject.Parse(content).ToObject<Git>();
                                if (obj.Ref == null)
                                    return;
                                Logs.LogWrite("发现推送:" + obj.repository.html_url + "|" + obj.head_commit.id);
                                string message = "GitHub仓库更新了!\n";
                                message += "仓库:" + obj.repository.html_url + "\n";
                                message += "更新者:" + obj.pusher.name + "\n";
                                message += "UUID:" + obj.head_commit.id + "\n";
                                message += "消息:" + obj.head_commit.message + "\n";
                                message += "时间:" + obj.head_commit.timestamp;
                                foreach (var item in MainConfig.推送群号)
                                {
                                    IGitHubPush.SGroupMessage(item, message);
                                }
                            });
                        }
                        break;
                    case "GET":
                        {
                            var data = request.QueryString;
                            //HttpProcessor.HandleGETRequest(data, context.Request, context.Response);
                            context.Response.StatusCode = 200;
                            var data1 = Encoding.UTF8.GetBytes("OK");
                            context.Response.OutputStream.Write(data1, 0, data1.Length);
                            context.Response.OutputStream.Close();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Logs.LogError(e);
            }
        }

        /// <summary>
        /// 重载配置
        /// </summary>
        public static void Reload()
        {
            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }
            if (!File.Exists(Path + Logs.log))
            {
                try
                {
                    File.WriteAllText(Path + Logs.log, "正在尝试创建日志文件" + Environment.NewLine);
                }
                catch
                {
                    MessageBox.Show("[Minecraft_QQ]日志文件创建失败");
                    return;
                }
            }

            ConfigRead read = new ConfigRead();

            ConfigFile.主要配置文件 = new FileInfo(Path + "Mainconfig.json");

            //读取主配置文件
            if (ConfigFile.主要配置文件.Exists == false)
            {
                Logs.LogWrite("[Info][Config]新建主配置");
                MainConfig = new MainConfig();
                File.WriteAllText(ConfigFile.主要配置文件.FullName, JsonConvert.SerializeObject(MainConfig, Formatting.Indented));
            }
            else
                MainConfig = read.ReadConfig();
        }
        /// <summary>
        /// 插件启动
        /// </summary>
        public static void Start()
        {
            Reload();
            try
            {
                listener.Prefixes.Add("http://" + MainConfig.服务器地址 + ":" + MainConfig.服务器端口 + "/");
                listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(30);
                listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(30);
                listener.Start();
                listener.BeginGetContext(ContextReady, null);
                IsStart = true;
                Logs.LogWrite("已启动");
            }
            catch (Exception e)
            {
                Logs.LogError(e);
            }
        }

        public static void Stop()
        {
            IsStart = false;
            listener.Stop();
        }
    }
}
