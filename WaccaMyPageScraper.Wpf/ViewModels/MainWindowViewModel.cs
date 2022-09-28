using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using WaccaMyPageScraper.Wpf.Converters;
using WaccaMyPageScraper.Wpf.Enums;
using WaccaMyPageScraper.Wpf.Views;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class MainWindowViewModel : BindableBase
    {
        private ConsoleWindow Console;
        private SettingsWindow SettingsWindow;

        public DelegateCommand OpenConsoleCommand { get; private set; }

        public DelegateCommand OpenSettingsWindowCommand { get; private set; }

        public MainWindowViewModel()
        {
            InitializeConsole();
            InitializeSettingsWindow();
            InitializeCultures();
        }

        private void InitializeConsole()
        {
            // Initializing logger console
            this.Console = new ConsoleWindow();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RichTextBox(this.Console.richTextBoxLog)
                .CreateLogger();

            // Prevent termination of console instance.
            this.Console.Closing += (sender, e) =>
            {
                this.Console.Hide();
                e.Cancel = true;
            };

            // Auto scroll when console content updated
            this.Console.richTextBoxLog.TextChanged += (sender, e) => this.Console.richTextBoxLog.ScrollToEnd();

            this.OpenConsoleCommand = new DelegateCommand(ExecuteOpenConsoleCommand);
        }

        private void InitializeSettingsWindow()
        {
            this.SettingsWindow = new SettingsWindow();

            this.OpenSettingsWindowCommand = new DelegateCommand(ExecuteSettingsWindowCommand);
        }

        private void ExecuteOpenConsoleCommand()
        {
            this.Console.Owner = Application.Current.MainWindow;
            this.Console.Show();
        }

        private void InitializeCultures()
        {
            var langConverter = new LanguageConverter();
            var language = (Language)langConverter.ConvertBack(
                WaccaMyPageScraper.Wpf.Properties.Settings.Default["Language"],
                typeof(Language),
                null,
                CultureInfo.InvariantCulture);

            string languageCode = language switch
            {
                Language.English => "en",
                Language.Korean => "ko",
                Language.Japanese => "ja",
            };

            Thread.CurrentThread.CurrentCulture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageCode);
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        private void ExecuteSettingsWindowCommand()
        {
            this.SettingsWindow.Close();
            InitializeSettingsWindow();

            this.SettingsWindow.Owner = Application.Current.MainWindow;
            this.SettingsWindow.ShowDialog();
        }
    }
}
