using ControlzEx.Theming;
using MahApps.Metro.Theming;
using Prism.Ioc;
using Prism.Unity;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WaccaMyPageScraper.Wpf.Views;

namespace WaccaMyPageScraper.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public static ICollection<KeyValuePair<string, Color>> Colors = typeof(Colors)
                .GetProperties()
                .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop => new KeyValuePair<string, Color>(prop.Name, (Color)prop.GetValue(null)))
                .ToList();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILogger>();
        }

        protected override Window CreateShell()
        {
            var window = Container.Resolve<MainWindow>();

            return window;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load theme
            /*
            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            var inverseTheme = ThemeManager.Current.GetInverseTheme(theme);
            */

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            // Apply theme from settings
            string colorSetting = (string)WaccaMyPageScraper.Wpf.Properties.Settings.Default["Color"];
            string themeSetting = (string)WaccaMyPageScraper.Wpf.Properties.Settings.Default["Theme"];

            if (string.IsNullOrEmpty(colorSetting))
                colorSetting = "Pink";

            var color = new KeyValuePair<string, Color>();
            try
            {
                color = Colors.First(kv => kv.Key == colorSetting);
            }
            catch
            {
                color = Colors.First(kv => kv.Key == "Pink");
            }

            if (string.IsNullOrEmpty(themeSetting)
                && (themeSetting != "Dark" || themeSetting != "Light"))
                themeSetting = "Dark";

            ThemeManager.Current.ChangeTheme(this, RuntimeThemeGenerator.Current.GenerateRuntimeTheme(themeSetting, color.Value));

            // this.MainWindow?.Activate();
        }
    }
}
