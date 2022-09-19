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
    public sealed class SortRecordByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sortRecordBy = (SortRecordBy)value;

            return sortRecordBy switch
            {
                SortRecordBy.Default => WaccaMyPageScraper.Localization.Data.Default,
                SortRecordBy.Title => WaccaMyPageScraper.Localization.Data.Title,
                SortRecordBy.Artist => WaccaMyPageScraper.Localization.Data.Artist,
                SortRecordBy.Level => WaccaMyPageScraper.Localization.Data.Level,
                SortRecordBy.Score => WaccaMyPageScraper.Localization.Data.Score,
                SortRecordBy.PlayCount => WaccaMyPageScraper.Localization.Data.PlayCount,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
