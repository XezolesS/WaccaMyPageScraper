using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Converter;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Data
{
    [TypeConverter(typeof(StageConverter))]
    public class Stage
    {
        public int Rank { get; set; }

        public StageGrade Grade { get; set; }

        public override string ToString() => string.Format("Stage {0}, {1}", this.Rank, this.Grade);
    }
}
