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
    /// <summary>
    /// Structure data for stage records.
    /// </summary>
    [TypeConverter(typeof(StageConverter))]
    public class Stage
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
        /// <b><see cref="StageGrade"/></b>: NotCleared(0), Blue(1), Silver(2), Gold(3)
        /// </summary>
        public StageGrade Grade { get; set; }

        public Stage()
        {
            this.Id = 0;
            this.Name = string.Empty;
            this.Grade = StageGrade.NotCleared;
        }

        public Stage(int id, StageGrade grade)
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

        public Stage(int id, string name, StageGrade grade)
        {
            this.Id = id;
            this.Name = name;
            this.Grade = grade;
        }

        public override string ToString() => string.Format("({0}){1} | {2}", this.Id, this.Name, this.Grade);
    }
}
