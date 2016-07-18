using System;

namespace Bridge.TinyPlotter
{
    [ObjectLiteral]
    public class Interval
    {
        public double From;
        public double To;
    }

    [ObjectLiteral]
    public class Color
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }

    [ObjectLiteral]
    public class Point
    {
        public double X;
        public double Y;
    }

    [ObjectLiteral]
    public class Curve
    {
        public Func<double, double> Map { get; set; }
        public Color Color { get; set; }
    }
}