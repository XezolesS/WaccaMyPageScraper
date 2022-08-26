using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaccaMyPageScraper.Wpf.Views;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand OpenConsoleCommand { get; private set; }

        public MainWindowViewModel()
        {
            this.OpenConsoleCommand = new DelegateCommand(ExecuteOpenConsoleCommand);
        }

        private void ExecuteOpenConsoleCommand()
        {
            ConsoleWindow console = new ConsoleWindow();
            console.Owner = Application.Current.MainWindow;
            console.Show();
        }
    }
}
