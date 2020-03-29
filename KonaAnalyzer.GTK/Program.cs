using System;
using Gtk;
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
                Gtk.Application.Init();
            Xamarin.Forms.Init();

                var app = new App();
                var window = new FormsWindow();
                window.LoadApplication(app);
                window.SetApplicationTitle("Game of Life");
                window.Show();

                Gtk.Application.Run();
            }
        }
    