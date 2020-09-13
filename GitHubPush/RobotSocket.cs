using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubPush
{
    class PackBase
    { 
        public long qq { get; set; }
    }
    class PackStart
    {
        public string Name { get; set; }
        public List<byte> Reg { get; set; }
    }
    class SendGroupMessagePack : PackBase
    {
        public long id { get; set; }
        public List<string> message { get; set; }
    }
    class BuildPack
    {
        public static byte[] Build(object obj, byte index)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj) + " ");
            data[data.Length - 1] = index;
            return data;
        }
    }
    class RobotTask
    {
        public byte index { get; set; }
        public string data { get; set; }
    }
    class RobotSocket
    {
        private static Socket Socket;
        private static Thread ReadThread;
        private static bool IsRun;
        private static bool IsConnect;
        private static ConcurrentBag<byte[]> QueueSend;
        private static PackStart PackStart = new PackStart
        {
            Name = "GitHubPush",
            Reg = new List<byte>()
            {  }
        };
        public static void Start()
        {
            QueueSend = new ConcurrentBag<byte[]>();

            ReadThread = new Thread(() =>
            {
                while (!IsRun)
                {
                    Thread.Sleep(100);
                }
                byte[] Send;
                //int time = 0;
                while (IsRun)
                {
                    try
                    {
                        if (!IsConnect)
                        {
                            ReConnect();
                        }
                        //else if (time >= 200)
                        //{
                        //    time = 0;
                        //    if (Socket.Poll(10000, SelectMode.SelectRead))
                        //    {
                        //        Logs.LogWrite("机器人连接中断");
                        //        IsConnect = false;
                        //        Logs.LogError("机器人20秒后重连");
                        //        Thread.Sleep(20000);
                        //        Logs.LogError("机器人重连中");
                        //    }
                        //}
                        else if (QueueSend.TryTake(out Send))
                        {
                            Socket.Send(Send);
                        }
                        //time++;
                        Thread.Sleep(50);
                    }
                    catch (Exception e)
                    {
                        Logs.LogError("机器人连接失败");
                        Logs.LogError(e);
                        IsConnect = false;
                        Logs.LogError("机器人20秒后重连");
                        Thread.Sleep(20000);
                        Logs.LogError("机器人重连中");
                    }
                }
            });
            ReadThread.Start();
            IsRun = true;
        }

        private static void ReConnect()
        {
            if (Socket != null)
                Socket.Close();
            try
            {
                Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                Socket.Connect(IPAddress.Parse("127.0.0.1"), 23333);

                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(PackStart) + " ");
                data[data.Length - 1] = 0;

                Socket.Send(data);

                QueueSend.Clear();
                Logs.LogWrite("机器人已连接");
                IsConnect = true;
            }
            catch (Exception e)
            {
                Logs.LogError("机器人连接失败");
                Logs.LogError(e);
            }
        }
        public static void SendGroupMessage(long id, string message)
        {
            var data = BuildPack.Build(new SendGroupMessagePack { qq = GitHubPush.MainConfig.机器人QQ号, id = id, message = new List<string>() { message } }, 52);
            QueueSend.Add(data);
        }
        public static void Stop()
        {
            Logs.LogWrite("机器人正在断开");
            IsRun = false;
            if (Socket != null)
                Socket.Close();
            Logs.LogWrite("机器人已断开");
        }
    }
}
