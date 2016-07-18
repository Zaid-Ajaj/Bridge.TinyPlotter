using Bridge.Html5;
using System;
using System.Collections.Generic;

namespace Bridge.TinyPlotter
{
    /// <summary>
    /// A minimalistic plotter for single variable functions.
    /// </summary>
    public class TinyPlotter
    {
        // by default;
        public PlotSettings Settings = new PlotSettings
        {
            Curves = new List<Curve>(),
            DrawBorder = true,
            DrawXAxis = true,
            DrawYAxis = true,
            Height = 500,
            Width = 800,
            DeltaX = 0.01,
            XMin = -10,
            XMax = 10,
            YMin = -10,
            YMax = 10 
        };


        public HTMLCanvasElement Canvas;

        public TinyPlotter(PlotSettings settings)
        {
            Settings = settings;
            InitializePlot();
        }

        public TinyPlotter(Func<double, double> f, double xmin, double xmax, double ymin, double ymax)
        {
            var black = new Color { Red = 0, Green = 0, Blue = 0 };
            Settings.Curves.Add(new Curve { Map = f, Color = black });
            Settings.XMin = xmin;
            Settings.XMax = xmax;
            Settings.YMin = ymin;
            Settings.YMax = ymax;
            InitializePlot();
        }

        TinyPlotter(params Func<double, double>[] functions)
        {
            var black = new Color { Red = 0, Green = 0, Blue = 0 };
            foreach(var func in functions)
            {
                var curve = new Curve { Color = black, Map = func };
                Settings.Curves.Add(curve);
            }
            InitializePlot();
        }

        public TinyPlotter(Func<double, double> f, double xmin, double xmax)
        {
            var black = new Color { Red = 0, Green = 0, Blue = 0 };
            Settings.Curves.Add(new Curve { Map = f, Color = black });
            Settings.XMin = xmin;
            Settings.XMax = xmax;
            InitializePlot();
        }


        private void InitializePlot()
        {
            Canvas = new HTMLCanvasElement();
            Canvas.Height = Settings.Height;
            Canvas.Width = Settings.Width;
            if (Settings.DrawBorder)
                Canvas.Style.Border = "1px solid black";
            var ctx = Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            var image = ctx.CreateImageData(Canvas.Width, Canvas.Height);
            if (Settings.DrawXAxis)
                DrawXAxis(image);
            if (Settings.DrawXAxis)
                DrawYAxis(image);
            ctx.PutImageData(image, 0, 0);
        }

        /// <summary>
        /// Draws all curves within Settings.Curves on the canvas.
        /// </summary>
        public void Draw()
        {
            if (!SettingsLookGood())
            {
                throw new Exception("Settings are invalid");
            }

            var ctx = Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            var image = ctx.CreateImageData(Canvas.Width, Canvas.Height);

            if (Settings.DrawXAxis)
                DrawXAxis(image);

            if (Settings.DrawXAxis)
                DrawYAxis(image);

            foreach (var curve in Settings.Curves)
                DrawCurve(curve, image);

            ctx.PutImageData(image, 0, 0);
        }

        void DrawXAxis(ImageData image)
        {
            var xmin = Settings.XMin;
            var xmax = Settings.XMax;
            var ymin = Settings.YMin;
            var ymax = Settings.YMax;
            var step = Settings.DeltaX;

            for (double x = xmin; x <= xmax; x += 0.01)
            {
                var point = new Point { X = x, Y = 0 };
                var pointFromPlain = FromPointOnPlain(point, xmin, xmax, ymin, ymax, Canvas.Height, Canvas.Width);
                SetPixel(image, (int)pointFromPlain.X, (int)pointFromPlain.Y, new Color { Red = 0, Blue = 0, Green = 0 });
            }
        }

        void DrawYAxis(ImageData image)
        {
            var xmin = Settings.XMin;
            var xmax = Settings.XMax;
            var ymin = Settings.YMin;
            var ymax = Settings.YMax;
            var step = Settings.DeltaX;

            for (double y = ymin; y <= ymax; y += 0.01)
            {
                var point = new Point { X = 0, Y = y };
                var pointFromPlain = FromPointOnPlain(point, xmin, xmax, ymin, ymax, Canvas.Height, Canvas.Width);
                SetPixel(image, (int)pointFromPlain.X, (int)pointFromPlain.Y, new Color { Red = 0, Blue = 0, Green = 0 });
            }
        }
        void DrawCurve(Curve curve, ImageData image)
        {
            var xmin = Settings.XMin;
            var xmax = Settings.XMax;
            var ymin = Settings.YMin;
            var ymax = Settings.YMax;
            var step = Settings.DeltaX;

            for (double x = xmin; x <= xmax; x += step)
            {
                var y = curve.Map(x);
                if (y < ymin || y > ymax || double.IsNaN(y)) // off bounds or undefined
                    continue;

                var point = new Point { X = x, Y = y };
                var pointFromPlain = FromPointOnPlain(point, xmin, xmax, ymin, ymax, Canvas.Height, Canvas.Width);
                SetPixel(image, (int)pointFromPlain.X, (int)pointFromPlain.Y, curve.Color);
            }
        }


        double Rescale(double value, Interval realRange, Interval projection)
        {
            var percentageOfProjection = (Math.Abs(projection.To - projection.From) * Math.Abs(value - realRange.From)) / Math.Abs(realRange.To - realRange.From);

            return percentageOfProjection + projection.From;
        }

        private bool SettingsLookGood()
        {
            return Settings.XMin < Settings.XMax
                && Settings.DeltaX < Math.Abs(Settings.XMax - Settings.XMin)
                && Settings.YMin < Settings.YMax
                && Settings.Height > 0
                && Settings.Width > 0;
        }

        Point FromPointOnPlain(Point p, double xmin, double xmax, double ymin, double ymax, double height, double width)
        {
            var projectedPoint = new Point
            {
                X = Rescale(p.X,
                         new Interval { From = xmin, To = xmax },
                         new Interval { From = 0, To = width }),

                Y = height - Rescale(p.Y,
                                new Interval { From = ymin, To = ymax },
                                new Interval { From = 0, To = height }),
            };


            return projectedPoint;
        }

        void SetPixel(ImageData img, int x, int y, Color color)
        {
            int index = (x + y * (int)img.Width) * 4;
            img.Data[index] = color.Red;
            img.Data[index + 1] = color.Green;
            img.Data[index + 2] = color.Blue;
            img.Data[index + 3] = 255; // alpha
        }
    }
}
