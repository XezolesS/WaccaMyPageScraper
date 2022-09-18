using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WaccaMyPageScraper.Wpf.Resources
{
    public static class RateColors
    {
        #region Rate Colors
        private static LinearGradientBrush RateColor1 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#808080"),
                (Color)ColorConverter.ConvertFromString("#606060"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor2 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#404060"),
                (Color)ColorConverter.ConvertFromString("#303050"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor3 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#F0F020"),
                (Color)ColorConverter.ConvertFromString("#D0D010"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor4 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#F02020"),
                (Color)ColorConverter.ConvertFromString("#D01010"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor5 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#804080"),
                (Color)ColorConverter.ConvertFromString("#602060"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor6 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#2020F0"),
                (Color)ColorConverter.ConvertFromString("#1010D0"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor7 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#B0B0C0"),
                (Color)ColorConverter.ConvertFromString("#9090A0"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor8 = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString("#B0B040"),
                (Color)ColorConverter.ConvertFromString("#909020"),
                new Point(0, 0),
                new Point(1, 1));

        private static LinearGradientBrush RateColor9 = new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(255,0,255,255), 0.0),
                new GradientStop(Color.FromArgb(255,0,255,0), 0.1),
                new GradientStop(Color.FromArgb(255,255,255,0), 0.3),
                new GradientStop(Color.FromArgb(255,255,0,255), 0.5),
                new GradientStop(Color.FromArgb(255,255,255,0), 0.7),
                new GradientStop(Color.FromArgb(255,0,255,0), 0.9),
                new GradientStop(Color.FromArgb(255,0,255,255), 1.0),
            }, 45.0);
        #endregion

        public static Brush GetColor(int rate) => rate switch
        {
            var r when r < 300 => RateColor1,
            var r when r < 600 => RateColor2,
            var r when r < 1000 => RateColor3,
            var r when r < 1300 => RateColor4,
            var r when r < 1600 => RateColor5,
            var r when r < 1900 => RateColor6,
            var r when r < 2200 => RateColor7,
            var r when r < 2500 => RateColor8,
            var r when r >= 2500 => RateColor9,
            _ => RateColor1
        };
    }
}
