using AppKit;

namespace KonaAnalyzer.Mac
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppDelegate();
            Bootstrapper.Initialize();
            NSApplication.Main(args);
        }
    }

}


