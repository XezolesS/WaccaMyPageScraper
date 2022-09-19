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
    public sealed class SeasonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var season = (int)value;

            return string.Format("{0} {1}", WaccaMyPageScraper.Localization.Data.Season, season);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return 1;

            var seasonText = (string)value;
            var trimmed = seasonText
                .Replace(WaccaMyPageScraper.Localization.Data.Season, "")
                .Trim();

            if (!int.TryParse(trimmed, out var season))
                return 1;

            return season;
        }
    }
}
