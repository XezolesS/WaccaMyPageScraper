using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.Wpf.Resources
{
    public static class ImageLocator
    {
        public static byte[] GetPlayerIcon() => GetImage(DataFilePath.PlayerIcon);

        public static byte[] GetRateIcon(int playCount, Rate rate) => (playCount, rate) switch 
        {
            var (p, r) when p > 0 && r == Rate.D => Properties.Resources.RateD,
            var (p, r) when r == Rate.C => Properties.Resources.RateC,
            var (p, r) when r == Rate.B => Properties.Resources.RateB,
            var (p, r) when r == Rate.A => Properties.Resources.RateA,
            var (p, r) when r == Rate.AA => Properties.Resources.RateAA,
            var (p, r) when r == Rate.AAA => Properties.Resources.RateAAA,
            var (p, r) when r == Rate.S => Properties.Resources.RateS,
            var (p, r) when r == Rate.S_Plus => Properties.Resources.RateS_Plus,
            var (p, r) when r == Rate.SS => Properties.Resources.RateSS,
            var (p, r) when r == Rate.SS_Plus => Properties.Resources.RateS_Plus,
            var (p, r) when r == Rate.SSS => Properties.Resources.RateSSS,
            var (p, r) when r == Rate.SSS_Plus => Properties.Resources.RateSSS_Plus,
            var (p, r) when r == Rate.Master => Properties.Resources.RateMaster,
            _ => Properties.Resources.RateNo
        };

        public static byte[] GetAchieveIcon(Achieve achieve) => achieve switch
        {
            Achieve.Clear => Properties.Resources.AchieveClear,
            Achieve.Missless => Properties.Resources.AchieveMissless,
            Achieve.FullCombo => Properties.Resources.AchieveFullCombo,
            Achieve.AllMarvelous => Properties.Resources.AchieveAllMarvelous,
            _ => Properties.Resources.AchieveNo
        };

        public static byte[] GetStageIcon(StageMetadata stage) => (stage.Id, stage.Grade) switch 
        {
            var (id, grade) when id == 1 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageI_Blue,
                StageGrade.Silver => Properties.Resources.StageI_Silver,
                StageGrade.Gold => Properties.Resources.StageI_Gold,
            },
            var (id, grade) when id == 2 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageII_Blue,
                StageGrade.Silver => Properties.Resources.StageII_Silver,
                StageGrade.Gold => Properties.Resources.StageII_Gold,
            },
            var (id, grade) when id == 3 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageIII_Blue,
                StageGrade.Silver => Properties.Resources.StageIII_Silver,
                StageGrade.Gold => Properties.Resources.StageIII_Gold,

            },
            var (id, grade) when id == 4 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageIV_Blue,
                StageGrade.Silver => Properties.Resources.StageIV_Silver,
                StageGrade.Gold => Properties.Resources.StageIV_Gold,
            },
            var (id, grade) when id == 5 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageV_Blue,
                StageGrade.Silver => Properties.Resources.StageV_Silver,
                StageGrade.Gold => Properties.Resources.StageV_Gold,
            },
            var (id, grade) when id == 6 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageVI_Blue,
                StageGrade.Silver => Properties.Resources.StageVI_Silver,
                StageGrade.Gold => Properties.Resources.StageVI_Gold,
            },
            var (id, grade) when id == 7 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageVII_Blue,
                StageGrade.Silver => Properties.Resources.StageVII_Silver,
                StageGrade.Gold => Properties.Resources.StageVII_Gold,
            },
            var (id, grade) when id == 8 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageVIII_Blue,
                StageGrade.Silver => Properties.Resources.StageVIII_Silver,
                StageGrade.Gold => Properties.Resources.StageVIII_Gold,
            },
            var (id, grade) when id == 9 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageIX_Blue,
                StageGrade.Silver => Properties.Resources.StageIX_Silver,
                StageGrade.Gold => Properties.Resources.StageIX_Gold,
            },
            var (id, grade) when id == 10 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageX_Blue,
                StageGrade.Silver => Properties.Resources.StageX_Silver,
                StageGrade.Gold => Properties.Resources.StageX_Gold,
            },
            var (id, grade) when id == 11 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageXI_Blue,
                StageGrade.Silver => Properties.Resources.StageXI_Silver,
                StageGrade.Gold => Properties.Resources.StageXI_Gold,
            },
            var (id, grade) when id == 12 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageXII_Blue,
                StageGrade.Silver => Properties.Resources.StageXII_Silver,
                StageGrade.Gold => Properties.Resources.StageXII_Gold,
            },
            var (id, grade) when id == 13 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageXIII_Blue,
                StageGrade.Silver => Properties.Resources.StageXIII_Silver,
                StageGrade.Gold => Properties.Resources.StageXIII_Gold,
            },
            var (id, grade) when id == 14 => grade switch
            {
                StageGrade.Blue => Properties.Resources.StageXIV_Blue,
                StageGrade.Silver => Properties.Resources.StageXIV_Silver,
                StageGrade.Gold => Properties.Resources.StageXIV_Gold,
            },
            _ => new byte[0],
        };

        public static byte[] GetTrophyIcon(Trophy trophy) => (trophy.Category, trophy.Rarity) switch
        {
            var (c, r) when c == 1 => r switch
            {
                1 => Properties.Resources.Trophy_1_1,
                2 => Properties.Resources.Trophy_1_2,
                3 => Properties.Resources.Trophy_1_3,
                4 => Properties.Resources.Trophy_1_4,
            },
            var (c, r) when c == 2 => r switch
            {
                1 => Properties.Resources.Trophy_2_1,
                2 => Properties.Resources.Trophy_2_2,
                3 => Properties.Resources.Trophy_2_3,
                4 => Properties.Resources.Trophy_2_4,
            },
            var (c, r) when c == 3 => r switch
            {
                1 => Properties.Resources.Trophy_3_1,
                2 => Properties.Resources.Trophy_3_2,
                3 => Properties.Resources.Trophy_3_3,
                4 => Properties.Resources.Trophy_3_4,
            },
            var (c, r) when c == 4 => r switch
            {
                1 => Properties.Resources.Trophy_4_1,
                2 => Properties.Resources.Trophy_4_2,
                3 => Properties.Resources.Trophy_4_3,
                4 => Properties.Resources.Trophy_4_4,
            },
            var (c, r) when c == 5 => r switch
            {
                1 => Properties.Resources.Trophy_5_1,
                2 => Properties.Resources.Trophy_5_2,
                3 => Properties.Resources.Trophy_5_3,
                4 => Properties.Resources.Trophy_5_4,
            },
            var (c, r) when c == 6 => r switch
            {
                1 => Properties.Resources.Trophy_6_1,
                2 => Properties.Resources.Trophy_6_2,
                3 => Properties.Resources.Trophy_6_3,
                4 => Properties.Resources.Trophy_6_4,
            },
        };

        public static byte[] GetTrophyBronzeIcon() => Properties.Resources.TrophyBronze;

        public static byte[] GetTrophySilverIcon() => Properties.Resources.TrophySilver;

        public static byte[] GetTrophyGoldIcon() => Properties.Resources.TrophyGold;

        public static byte[] GetTrophyPlatinumIcon() => Properties.Resources.TrophyPlatinum;

        /// <summary>
        /// Get byte data of an located image file.
        /// </summary>
        /// <param name="file">Path of an image to get.</param>
        /// <returns>Byte data of an image.</returns>
        public static byte[] GetImage(string? file)
        {
            if (!File.Exists(file))
            {
                Log.Warning("There's no such image file: {Path}", Path.GetFullPath(file));

                return null;
            }

            byte[] image = null;
            using (FileStream? fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                image = new byte[fs.Length];
                while (fs.Read(image, 0, image.Length) > 0)
                {
                    image[image.Length - 1] = (byte)fs.ReadByte();
                }
            }

            return image;
        }
    }
}
