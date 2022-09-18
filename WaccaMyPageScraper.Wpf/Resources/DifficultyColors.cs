using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Wpf.Resources
{
    public static class DifficultyColors
    {
        public static Brush Normal => GetBrush(Difficulty.Normal);

        public static Brush Hard => GetBrush(Difficulty.Hard);

        public static Brush Expert => GetBrush(Difficulty.Expert);

        public static Brush Inferno => GetBrush(Difficulty.Inferno);

        public static Brush GetBrush(Difficulty difficulty) => difficulty switch
        { 
            Difficulty.Normal => new SolidColorBrush(Color.FromRgb(0, 157, 230)),
            Difficulty.Hard => new SolidColorBrush(Color.FromRgb(254, 209, 49)),
            Difficulty.Expert => new SolidColorBrush(Color.FromRgb(252, 6, 163)),
            Difficulty.Inferno => new SolidColorBrush(Color.FromRgb(74, 0, 79)),
        }; 
    }
}
