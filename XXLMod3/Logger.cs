using UnityModManagerNet;

namespace XXLMod3
{
    public class Logger
    {
        public static void Log(string message)
        {
            UnityModManager.Logger.Log(message);
        }
    }
}
