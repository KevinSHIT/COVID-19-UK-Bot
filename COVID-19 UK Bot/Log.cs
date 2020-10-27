using System;

namespace COVID_19_UK_Bot
{
    public class Log
    {
        private static void WriteMsg(string tag, string msg)
            => Console.WriteLine($"[{tag}]{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}: {msg}");

        public static void i(string msg)
            => WriteMsg("I", msg);

        public static void e(string msg)
            => WriteMsg("E", msg);

        public static void w(string msg)
            => WriteMsg("W", msg);
    }
}