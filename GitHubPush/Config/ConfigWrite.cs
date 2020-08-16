using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GitHubPush.Config
{
    internal class ConfigWrite
    {
        public void Config()
        {
            Save(ConfigFile.主要配置文件.FullName, GitHubPush.MainConfig);
        }

        private void Save(string FileName, object obj)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    File.WriteAllText(FileName,
                    JsonConvert.SerializeObject(obj, Formatting.Indented));

                }
                catch (Exception e)
                {
                    Logs.LogError(e);
                    Console.WriteLine("配置文件在写入时发发生了错误");
                }
            });
        }
    }
}
