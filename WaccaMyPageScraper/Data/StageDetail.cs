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
    /// Structure data for detailed stage record.
    /// </summary>
    public class StageDetail : Stage
    {
        /// <summary>
        /// Scores for each tracks (Max 3).
        /// </summary>
        public int[] Scores { get; set; }

        /// <summary>
        /// Total score of the stage.
        /// </summary>
        public int TotalScore { get => Scores.Sum(); }

        public StageDetail() : base()
        {
            this.Scores = new int[3]; 
        }

        public StageDetail(int id, StageGrade grade) : base(id, grade)
        {
            this.Scores = new int[3];
        }

        public StageDetail(int id, string name, StageGrade grade, int[] scores) : base(id, name, grade)
        {
            this.Scores = scores;
        }

        public StageDetail(Stage stage, int[] scores) : base(stage.Id, stage.Name, stage.Grade)
        {
            this.Scores = scores;
        }

        public override string ToString() => string.Format("[{0},{1},{2},{3},[{4}]]",
            this.Id, this.Name, this.Grade, this.TotalScore,
            string.Join(",", Scores));
    }

    public sealed class StageDetailMap : ClassMap<StageDetail>
    {
        public StageDetailMap()
        {
            Map(m => m.Id).Index(0).Name("id");
            Map(m => m.Name).Index(1).Name("name");
            Map(m => m.Grade).Index(2).Name("grade")
                .TypeConverter<EnumConverter<StageGrade>>();
            Map(m => m.Scores).Index(3).Name("scores")
                .TypeConverter<Int32ArrayConverter>();
            Map(m => m.TotalScore).Index(4).Name("total_score");
        }
    }
}
