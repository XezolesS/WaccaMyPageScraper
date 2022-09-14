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
    public sealed class Int32ArrayConverter : TypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var textRegex = new Regex(@"^\[(\d+||,)+\]$");
            if (!textRegex.IsMatch(text))
            {
                string message = "Converting System.Int32[] is not supported.";
                throw new TypeConverterException(this, memberMapData, text, row.Context, message);
            }
            
            var splitted = text.Remove(text.Length - 1, 1).Remove(0, 1).Split(',');
            var converted = Array.ConvertAll(splitted, s => int.Parse(s));

            return converted;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is not int[])
            {
                string message = "Converting System.Int32[] is not supported.";
                throw new TypeConverterException(this, memberMapData, value, row.Context, message);
            }

            return string.Format("[{0}]", string.Join(",", value as int[]));
        }
    }
}
