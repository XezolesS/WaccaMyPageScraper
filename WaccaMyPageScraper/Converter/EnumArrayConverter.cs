using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Converter
{
    public sealed class EnumArrayConverter<TEnum> : TypeConverter where TEnum : Enum
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var textRegex = new Regex(@"^\[(.+||,)+\]$");
            if (!textRegex.IsMatch(text))
            {
                string message = string.Format(Localization.Converter.ConvertingNotSupported, "System.Enum[]");
                throw new TypeConverterException(this, memberMapData, text, row.Context, message);
            }

            var splitted = text.Remove(text.Length - 1, 1).Remove(0, 1).Split(',');
            var converted = Array.ConvertAll(splitted, s => int.Parse(s));

            return converted as TEnum[];
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is not TEnum[])
            {
                string message = string.Format(Localization.Converter.ConvertingNotSupported, "System.Enum[]");
                throw new TypeConverterException(this, memberMapData, value, row.Context, message);
            }

            return string.Format("[{0}]", string.Join(",", value as int[]));
        }
    }
}
