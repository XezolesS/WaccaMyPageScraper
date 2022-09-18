using LiveChartsCore.SkiaSharpView.Painting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Wpf.Resources
{
    public static class RatePaints
    {
        public static Paint GetPaint(Rate rate) => rate switch
        {
            Rate.D => new SolidColorPaint(new SkiaSharp.SKColor(100, 100, 100)),
            Rate.C => new SolidColorPaint(new SkiaSharp.SKColor(30, 30, 200)),
            Rate.B => new SolidColorPaint(new SkiaSharp.SKColor(100, 100, 200)),
            Rate.A => new SolidColorPaint(new SkiaSharp.SKColor(255, 255, 160)),
            Rate.AA => new SolidColorPaint(new SkiaSharp.SKColor(255, 255, 130)),
            Rate.AAA => new SolidColorPaint(new SkiaSharp.SKColor(255, 255, 100)),
            Rate.S => new SolidColorPaint(new SkiaSharp.SKColor(255, 180, 255)),
            Rate.S_Plus => new SolidColorPaint(new SkiaSharp.SKColor(255, 160, 255)),
            Rate.SS => new SolidColorPaint(new SkiaSharp.SKColor(255, 140, 255)),
            Rate.SS_Plus => new SolidColorPaint(new SkiaSharp.SKColor(255, 120, 255)),
            Rate.SSS => new SolidColorPaint(new SkiaSharp.SKColor(255, 100, 255)),
            Rate.SSS_Plus => new SolidColorPaint(new SkiaSharp.SKColor(255, 80, 255)),
            Rate.Master => new SolidColorPaint(new SkiaSharp.SKColor(255, 255, 80)),
            _ => new SolidColorPaint(new SkiaSharp.SKColor(100, 100, 100)),
        };
    }
}
