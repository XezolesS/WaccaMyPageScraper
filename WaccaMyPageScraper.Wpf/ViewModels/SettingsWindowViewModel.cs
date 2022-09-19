using ControlzEx.Theming;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using WaccaMyPageScraper.Wpf.Converters;
using WaccaMyPageScraper.Wpf.Enums;

#pragma warning disable CS8604 // Possible null reference argument.

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public class SettingsWindowViewModel : BindableBase
    {
        public Language[] LanguageOptions => Enum.GetValues<Language>();

        private Language _selectedLangauge;
        public Language SelectedLanguage
        {
            get => _selectedLangauge;
            set => SetProperty(ref _selectedLangauge, value, OnLanguageChanged);
        }

        public string[] _themes;
        public string[] Themes
        {
            get => _themes;
            set => SetProperty(ref _themes, value);
        }

        private string _selectedTheme;
        public string SelectedTheme
        {
            get => _selectedTheme;
            set => SetProperty(ref _selectedTheme, value, OnThemeChanged);
        }

        private ICollection<KeyValuePair<string, Color>> _colors;
        public ICollection<KeyValuePair<string, Color>> Colors
        {
            get => _colors;
            set => SetProperty(ref _colors, value);
        }

        private KeyValuePair<string, Color> _selectedColor;
        public KeyValuePair<string, Color> SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value, OnThemeChanged);
        }

        public DelegateCommand GitHubClickCommand { get; private set; }

        public DelegateCommand CooliconsClickCommand { get; private set; }

        public SettingsWindowViewModel()
        {
            // Load settings
            var langConverter = new LanguageConverter();
            var language = (Language)langConverter.ConvertBack(Properties.Settings.Default["Language"], 
                typeof(Language), 
                null, 
                CultureInfo.InvariantCulture);

            this._selectedLangauge = language; // Don't want a callback to call

            // Load colors and themes
            var theme = ThemeManager.Current.DetectTheme(Application.Current);

            this.Themes = new string[] { "Dark", "Light" };
            this._selectedTheme = theme.BaseColorScheme;

            this.Colors = App.Colors;
            this._selectedColor = App.Colors
                .First(kv => kv.Key == theme.ColorScheme 
                || kv.Value.ToString() == theme.ColorScheme); // Don't want a callback to call

            this.GitHubClickCommand = new DelegateCommand(GitHubClickEvent);
            this.CooliconsClickCommand = new DelegateCommand(CooliconsClickEvent);
        }

        private void GitHubClickEvent() => OpenUrl("https://github.com/XezolesS/WaccaMyPageScraper");

        private void CooliconsClickEvent() => OpenUrl("https://github.com/krystonschwarze/coolicons");

        private void OpenUrl(string url)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = url;

            process.Start();
        }

        private void OnLanguageChanged()
        {
            Properties.Settings.Default["Language"] = new LanguageConverter()
                .Convert(this.SelectedLanguage, typeof(string), null, CultureInfo.InvariantCulture).ToString();

            Properties.Settings.Default.Save();
        }

        private void OnThemeChanged()
        {
            ThemeManager.Current.ChangeTheme(
                Application.Current, 
                ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme(
                    this.SelectedTheme, this.SelectedColor.Value)));

            Properties.Settings.Default["Theme"] = this.SelectedTheme;
            Properties.Settings.Default["Color"] = this.SelectedColor.Key;
            Properties.Settings.Default.Save();

            Application.Current?.MainWindow?.Activate();
        }
    }
}
