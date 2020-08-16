using System;

namespace GitHubPush
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GitHubPush.Start();
            RobotSocket.Start();

            while (true)
            {
                string data = Console.ReadLine();
                if (data == "stop")
                {
                    GitHubPush.Stop();
                }
            }
        }
    }
}
