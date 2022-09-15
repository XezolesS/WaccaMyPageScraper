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
    public class SortRecordByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sortRecordBy = (SortRecordBy)value;

            return sortRecordBy switch
            {
                SortRecordBy.Default => Localization.SortRecordBy.Default,
                SortRecordBy.Title => Localization.SortRecordBy.Title,
                SortRecordBy.Artist => Localization.SortRecordBy.Artist,
                SortRecordBy.Level => Localization.SortRecordBy.Level,
                SortRecordBy.Score => Localization.SortRecordBy.Score,
                SortRecordBy.PlayCount => Localization.SortRecordBy.PlayCount,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
