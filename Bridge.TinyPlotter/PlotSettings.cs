using System.Collections.Generic;

namespace Bridge.TinyPlotter
{
    [ObjectLiteral]
    public class PlotSettings
    {
        /// <summary>
        /// The curves to draw on the canvas
        /// </summary>
        public List<Curve> Curves;
        /// <summary>
        /// The delta x defines the spacing between points on the x-axis.
        /// </summary>
        public double DeltaX;
        /// <summary>
        /// The height of the canvas.
        /// </summary>
        public int Height;
        /// <summary>
        /// The width of the canvas.
        /// </summary>
        public int Width;

        // Viewport settings
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }

        public bool DrawXAxis;
        public bool DrawYAxis;
        public bool DrawBorder;
    }
}