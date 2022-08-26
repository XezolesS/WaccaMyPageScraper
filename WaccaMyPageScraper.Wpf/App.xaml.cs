using Prism.Ioc;
using Prism.Unity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WaccaMyPageScraper.Wpf.Views;

namespace WaccaMyPageScraper.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILogger>();
        }

        protected override Window CreateShell()
        {
            var window = Container.Resolve<MainWindow>();

            return window;
        }
    }
}
