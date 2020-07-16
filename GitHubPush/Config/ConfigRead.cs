using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GitHubPush.Config
{
    internal class ConfigRead
    {
        /// <summary>
        /// 读取主要配置文件
        /// </summary>
        public MainConfig ReadConfig()
        {
            Logs.LogWrite("[INFO][Config]读取主配置");
            try
            {
                var config = JsonConvert.DeserializeObject<MainConfig>
                    (File.ReadAllText(ConfigFile.主要配置文件.FullName));
                bool save = false;
                if (config.推送群号 == null)
                {
                    config.推送群号 = new List<long>();
                    save = true;
                }
                if (config.服务器地址 == null)
                {
                    config.服务器地址 = "127.0.0.1";
                    save = true;
                }
                if (config.服务器端口 == 0)
                {
                    config.服务器端口 = 25555;
                    save = true;
                }
                if (save)
                {
                    MessageBox.Show("Mainconfig.json配置文件读取发送错误，已经重写");
                    new ConfigWrite().Config();
                }
                return config;
            }
            catch (Exception e)
            {
                MessageBox.Show("快去检查你的Mainconfig.json文件语法，用的是json就要遵守语法！", "你配置文件爆了");
                Logs.LogError(e);
                return new MainConfig();
            }
        }
    }
}
