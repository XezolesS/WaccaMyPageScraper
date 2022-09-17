using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Converter;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Data
{
    /// <summary>
    /// Structure data for stage records.
    /// </summary>
    public class StageMetadata
    {
        /// <summary>
        /// Stage Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Stage Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Stage Grade. <br/>
        /// <b><see cref="StageGrade"/></b>: Unknown(0), Blue(1), Silver(2), Gold(3)
        /// </summary>
        public StageGrade Grade { get; set; }

        public StageMetadata()
        {
            this.Id = 0;
            this.Name = null;
            this.Grade = StageGrade.Unknown;
        }

        public StageMetadata(int id, StageGrade grade)
        {
            this.Id = id;
            this.Name = "ステージ" + id switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                4 => "IV",
                5 => "V",
                6 => "VI",
                7 => "VII",
                8 => "VIII",
                9 => "IX",
                10 => "X",
                11 => "XI",
                12 => "XII",
                13 => "XIII",
                14 => "XIV",
                _ => "???"
            };

            this.Grade = grade;
        }

        public StageMetadata(int id, string name, StageGrade grade)
        {
            this.Id = id;
            this.Name = name;
            this.Grade = grade;
        }

        public override string ToString() => string.Format("[{0},{1},{2}]", 
            this.Id, this.Name, (int)this.Grade);
    }
    internal sealed class StageMetadataMap : ClassMap<StageMetadata>
    {
        public StageMetadataMap()
        {
            Map(m => m.Id).Index(0).Name("stage_id");
            Map(m => m.Name).Index(1).Name("stage_name");
            Map(m => m.Grade).Index(2).Name("stage_grade")
                .TypeConverter<EnumConverter<StageGrade>>();
        }
    }
}