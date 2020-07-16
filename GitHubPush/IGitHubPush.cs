using Native.Sdk.Cqp;
using System.Threading.Tasks;

namespace GitHubPush
{
    public class IGitHubPush
    {
        public const string Version = "1.0.0.0";
        public static CQApi Api { get; set; }
        public static void Start()
        {
            Task.Factory.StartNew(() => GitHubPush.Start());
        }
        public static void Stop()
        {
            GitHubPush.Stop();
        }
        public static void SGroupMessage(long group, string message)
        {
            Api.SendGroupMessage(group, message);
        }
    }
}
