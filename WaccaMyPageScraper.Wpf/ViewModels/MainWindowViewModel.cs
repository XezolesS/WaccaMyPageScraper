using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaccaMyPageScraper.Wpf.Views;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class MainWindowViewModel : BindableBase
    {
        private ConsoleWindow Console;

        public DelegateCommand OpenConsoleCommand { get; private set; }

        public MainWindowViewModel()
        {
            InitializeConsole();
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
      
        private void ExecuteOpenConsoleCommand()
        {
            this.Console.Owner = Application.Current.MainWindow;
            this.Console.Show();
        }
    }
}
