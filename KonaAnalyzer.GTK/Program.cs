using System;
using Gtk;
using KonaAnalyzer;
using Xamarin.Forms.Platform.GTK;

namespace Corona.Gtk
{
    class MainClass
    {
        //public static void Main(string[] args)
        //{
        //    Application.Init();
        //    MainWindow win = new MainWindow();
        //    win.Show();
        //    Application.Run();
        //}

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init(); 
            global::Xamarin.Forms.Forms.Init();
            var app = new App();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("Game of Life");
            window.Show();

            Application.Run();
        }
    }
}
