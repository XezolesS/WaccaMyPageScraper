using MahApps.Metro.Controls;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WaccaMyPageScraper.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : MetroWindow
    {
        public ConsoleWindow()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
              .WriteTo.RichTextBox(richTextBoxLog)
              .CreateLogger();
        }
    }
}
