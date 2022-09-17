using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Converter;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Data
{
    public class StageRanking : StageMetadata
    {
        /// <summary>
        /// Global rankings of each difficulties. (Max 4)
        /// </summary>
        public int Rankings { get; set; }

        public StageRanking() : base()
        {
            this.Rankings = -1;
        }

        public StageRanking(int id, StageGrade grade, int rankings) : base(id, grade)
        {
            this.Rankings = rankings;
        }

        public StageRanking(int id, string name, StageGrade grade, int rankings) : base(id, name, grade)
        {
            this.Rankings = rankings;
        }

        public StageRanking(StageMetadata meta, int rankings) : base(meta.Id, meta.Name, meta.Grade)
        {
            this.Rankings = rankings;
        }

        public override string ToString() => string.Format("[{0},{1},{2},{3}]",
           this.Id, this.Name, (int)this.Grade, this.Rankings);
    }

    internal sealed class StageRankingMap : ClassMap<StageRanking>
    {
        public StageRankingMap()
        {
            Map(m => m.Id).Index(0).Name("stage_id");
            Map(m => m.Name).Index(1).Name("stage_name");
            Map(m => m.Grade).Index(2).Name("stage_grade")
                .TypeConverter<EnumConverter<StageGrade>>();
            Map(m => m.Rankings).Index(3).Name("stage_ranking");
        }
    }
}
