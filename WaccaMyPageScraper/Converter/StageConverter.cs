using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly Regex _stageRegex = new Regex(@"stage_icon_[0-9]+_[1-3].png");

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo culture, object value)
        {
            if (value is not string)
            {
                return base.ConvertFrom(context, culture, value);
            }

            var valueAsString = value as string;
            if (!_stageRegex.IsMatch(valueAsString))
            {
                return base.ConvertFrom(context, culture, value);
            }

            var stageIconNumbers = new Regex("[0-9]+_[1-3]").Match(valueAsString).Value.Split('_');

            int id = int.Parse(stageIconNumbers[0]);
            StageGrade grade = (StageGrade)int.Parse(stageIconNumbers[1]);

            return new Stage(id, grade);
        }
    }
}
