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
    public class Stage : StageMetadata
    {
        /// <summary>
        /// Scores for each tracks (Max 3).
        /// </summary>
        public int[] Scores { get; set; }

        /// <summary>
        /// Total score of the stage.
        /// </summary>
        public int TotalScore { get; set; }

        public Stage() : base()
        {
            this.Scores = new int[3];
            this.TotalScore = 0;
        }

        public Stage(int id, StageGrade grade) : base(id, grade)
        {
            this.Scores = new int[3];
            this.TotalScore = 0;
        }

        public Stage(int id, string name, StageGrade grade, int[] scores) : base(id, name, grade)
        {
            this.Scores = scores;
            this.TotalScore = scores.Sum();
        }

        public Stage(int id, string name, StageGrade grade, int[] scores, int totalScore) : base(id, name, grade)
        {
            this.Scores = scores;
            this.TotalScore = totalScore;
        }

        public Stage(StageMetadata meta, int[] scores) : base(meta.Id, meta.Name, meta.Grade)
        {
            this.Scores = scores;
            this.TotalScore = scores.Sum();
        }

        public Stage(StageMetadata meta, int[] scores, int totalScore) : base(meta.Id, meta.Name, meta.Grade)
        {
            this.Scores = scores;
            this.TotalScore = totalScore;
        }

        public override string ToString() => string.Format("[{0},{1},{2},{3},[{4}]]",
            this.Id, this.Name, this.Grade, this.TotalScore,
            string.Join(",", Scores));
    }

    internal sealed class StageMap : ClassMap<Stage>
    {
        public StageMap()
        {
            Map(m => m.Id).Index(0).Name("stage_id");
            Map(m => m.Name).Index(1).Name("stage_name");
            Map(m => m.Grade).Index(2).Name("stage_grade")
                .TypeConverter<EnumConverter<StageGrade>>();
            Map(m => m.Scores).Index(3).Name("stage_scores")
                .TypeConverter<Int32ArrayConverter>();
            Map(m => m.TotalScore).Index(4).Name("stage_total_score");
        }
    }
}
