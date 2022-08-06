using Silkroad.Lib;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Silkroad
{
#if WINDOWS || XBOX

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [DllImport("kernel32")]
        private static extern bool AllocConsole();

        public static Pk2.Archive Data;
        public static Pk2.Archive mediapk2;
        public static Pk2.Archive Map;
        public static MainGame Window;

        [STAThread]
        private static void Main(string[] args)
        {
#if DEBUG
            AllocConsole();
            Console.WriteLine("SME is in debug mode!");
#endif

            Data = new Pk2.Archive(Path.Combine(MainGame.Path, "Data.pk2"));
            Data.Load();
            Map = new Pk2.Archive(Path.Combine(MainGame.Path, "Map.pk2"));
            Map.Load();
            //mediapk2 = new pk2.pk2Reader(Settings.silkroadPath + @"Media.pk2");
            Window = new MainGame();
            Window.Run();
        }
    }

#endif
}