using Microsoft.Extensions.Logging;
using Uno.Diagnostics.Eventing;

namespace KonaAnalyzer.Wasm
{
    public class Program
    {
        private IEventProvider provider = new NullEventProvider();
        static int Main(string[] args)
        {
            //ConfigureFilters(Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
            //ReactiveUI.Uno.
            Windows.UI.Xaml.Application.Start(_ => new KonaAnalyzer.UWP.App());

            return 0;
        }

        /// <summary>
        /// Configures global logging
        /// </summary>
        /// <param name="factory"></param>
//        static void ConfigureFilters(ILoggerFactory factory)
//        {
//            factory
//                .WithFilter(new FilterLoggerSettings
//                    {
//                        { "Uno", LogLevel.Warning },
//                        { "Windows", LogLevel.Warning },

//						// Debug JS interop
//						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

//						// Generic Xaml events
//						// { "Windows.UI.Xaml", LogLevel.Debug },
//						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
//						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
//						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

//						// Layouter specific messages
//						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
//						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
//						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
//						// { "Windows.Storage", LogLevel.Debug },

//						// Binding related messages
//						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

//						// DependencyObject memory references tracking
//						// { "ReferenceHolder", LogLevel.Debug },
//					}
//                )
//#if DEBUG
//                .AddConsole(LogLevel.Debug);
//#else
//                .AddConsole(LogLevel.Information);
//#endif
//        }
    }
}
