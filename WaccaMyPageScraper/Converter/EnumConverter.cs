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
    public sealed class EnumConverter<TEnum> : TypeConverter where TEnum : Enum
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text is not string)
            {
                string message = "Converting System.Enum[] is not supported.";
                throw new TypeConverterException(this, memberMapData, text, row.Context, message);
            }

            return Enum.Parse(typeof(TEnum), text);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is not TEnum)
            {
                string message = "Converting System.Enum[] is not supported.";
                throw new TypeConverterException(this, memberMapData, value, row.Context, message);
            }

            return ((int)value).ToString();
        }
    }
}
