using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WaccaMyPageScraper.Wpf.Resources
{
    public static class RankingColors 
    {
        public static Brush GetColor(int ranking) => ranking switch
        {
            var r when r < 0 => new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            var r when r == 1 => new LinearGradientBrush(
                    new GradientStopCollection()
                    {
                        new GradientStop(Color.FromRgb(200, 50, 255), 0.0),
                        new GradientStop(Color.FromRgb(255, 50, 140), 0.3),
                        new GradientStop(Color.FromRgb(255, 50, 140), 0.6),
                        new GradientStop(Color.FromRgb(255, 200, 50), 1.0),
                    }, 45.0),
            var r when r == 2 => new LinearGradientBrush(
                    new GradientStopCollection()
                    {
                        new GradientStop(Color.FromRgb(255, 200, 50), 0.0),
                        new GradientStop(Color.FromRgb(200, 160, 30), 1.0),
                    }, 45.0),
            var r when r == 3 => new LinearGradientBrush(
                    new GradientStopCollection()
                    {
                        new GradientStop(Color.FromRgb(50, 150, 255), 0.0),
                        new GradientStop(Color.FromRgb(30, 120, 200), 1.0),
                    }, 45.0),
            var r when r <= 100 => new SolidColorBrush(Color.FromRgb(100, 200, 30)),
            _ => new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        };
    }
}
