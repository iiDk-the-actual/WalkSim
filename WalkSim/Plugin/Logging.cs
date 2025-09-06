using System;
using System.Diagnostics;
using BepInEx.Logging;

namespace WalkSim.WalkSim.Plugin
{
    public static class Logging
    {
        private static ManualLogSource logger;

        public static int DebuggerLines = 20;

        public static void Init()
        {
            logger = Logger.CreateLogSource("WalkSimulator");
        }

        public static void Exception(Exception e)
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            logger.LogWarning(string.Concat("(", method.ReflectedType.Name, ".", method.Name, "()) ",
                string.Join(" ", e.Message, e.StackTrace)));
        }

        public static void Fatal(params object[] content)
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            logger.LogFatal(string.Concat("(", method.ReflectedType.Name, ".", method.Name, "()) ",
                string.Join(" ", content)));
        }

        public static void Warning(params object[] content)
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            logger.LogWarning(string.Concat("(", method.ReflectedType.Name, ".", method.Name, "()) ",
                string.Join(" ", content)));
        }

        public static void Info(params object[] content)
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            logger.LogInfo(string.Concat("(", method.ReflectedType.Name, ".", method.Name, "()) ",
                string.Join(" ", content)));
        }

        public static void Debug(params object[] content)
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            logger.LogDebug(string.Concat("(", method.ReflectedType.Name, ".", method.Name, "()) ",
                string.Join("  ", content)));
        }

        public static void Debugger(params object[] content)
        {
            Debug(content);
        }

        public static string PrependTextToLog(string log, string text)
        {
            log = text + "\n" + log;
            var array = log.Split('\n');
            var flag = array.Length > DebuggerLines;
            var flag2 = flag;
            if (flag2) log = string.Join("\n", array, 0, DebuggerLines);
            return log;
        }
    }
}