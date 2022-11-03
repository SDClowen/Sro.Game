using System;

namespace Silkroad
{
    public class Log
    {
        public static void WriteLog(string obj, params object[] args) =>
            Console.WriteLine(obj, args);

        public static void WriteLog(object obj) =>
            Console.WriteLine(obj);

        public static void Error(object obj) => WriteLog(obj);
        public static void Warn(object obj) => WriteLog(obj);
        public static void Notify(object obj) => WriteLog(obj);
        public static void Success(object obj) => WriteLog(obj);
    }
}
