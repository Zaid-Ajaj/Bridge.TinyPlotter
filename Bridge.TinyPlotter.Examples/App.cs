using System;
using Bridge.Html5;
using Bridge.TinyPlotter;
using System.Collections.Generic;

namespace Bridge.TinyPlotter.Examples
{
    public class App
    {
        public static void Main()
        {
            var black = new Color { Red = 0, Green = 0, Blue = 0 };
            var plotter = new TinyPlotter();
            plotter.Curves = new List<Curve>()
            {
                new Curve { Map = Math.Sin, Color = black },
                new Curve { Map = Math.Cos, Color = black },
                new Curve { Map = x => x * x, Color = black }
            };

            plotter.SetViewport(-Math.PI, Math.PI, -3, 3);
            plotter.Draw();

            Document.Body.AppendChild(plotter.Canvas);
        }


    }
}
