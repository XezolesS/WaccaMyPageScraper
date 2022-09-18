using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Wpf.Resources
{
    public static class AchievePaints
    {
        public static IPaint<SkiaSharpDrawingContext> NoAchieve
            => new SolidColorPaint(new SkiaSharp.SKColor(80, 80, 80));

        public static IPaint<SkiaSharpDrawingContext> Clear
            => new SolidColorPaint(new SkiaSharp.SKColor(130, 120, 255));

        public static IPaint<SkiaSharpDrawingContext> Missless
            => new SolidColorPaint(new SkiaSharp.SKColor(80, 255, 120));

        public static IPaint<SkiaSharpDrawingContext> FullCombo
            => new SolidColorPaint(new SkiaSharp.SKColor(255, 80, 200));

        public static IPaint<SkiaSharpDrawingContext> AllMarvelous
            => new SolidColorPaint(new SkiaSharp.SKColor(255, 255, 80));
    }
}
