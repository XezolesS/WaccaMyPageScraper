using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Converter
{
    public class StageConverter : TypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var textRegex = new Regex(@"^\[\d*,.*,[0-3]\]$");
            if (!textRegex.IsMatch(text))
            {
                string message = "Converting WaccaMyPageScraper.Data.Stage is not supported.";
                throw new TypeConverterException(this, memberMapData, text, row.Context, message);
            }

            var splitted = text.Remove(text.Length - 1, 1).Remove(0, 1).Split(',');

            int id = int.Parse(splitted[0]);
            string name = splitted[1];
            StageGrade grade = (StageGrade)int.Parse(splitted[2]);

            return new StageMetadata(id, name, grade);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is not StageMetadata)
            {
                string message = "Converting WaccaMyPageScraper.Data.Stage is not supported.";
                throw new TypeConverterException(this, memberMapData, value, row.Context, message);
            }

            return (value as StageMetadata).ToString();
        }
    }
}
