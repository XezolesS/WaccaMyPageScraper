using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WaccaMyPageScraper.Wpf.Enums;

namespace WaccaMyPageScraper.Wpf.Converters
{
    public sealed class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return Localization.Language.English;

            var language = (Language)value;

            return language switch
            {
                Language.English => Localization.Language.English,
                Language.Korean => Localization.Language.Korean,
                Language.Japanese => Localization.Language.Japanese,

                _ => Localization.Language.English,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return Language.English;

            var languageString = (string)value;

            return languageString switch
            {
                var str when str.Equals(Localization.Language.English) => Language.English,
                var str when str.Equals(Localization.Language.Korean) => Language.Korean,
                var str when str.Equals(Localization.Language.Japanese) => Language.Japanese,

                _ => Language.English,
            };
        }
    }
}
