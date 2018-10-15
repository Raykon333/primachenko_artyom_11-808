using System;
using System.Drawing;

namespace Fractals
{
    internal static class MathanFractal
    {
        public static void DrawMathanFractal(Pixels pixels, int iterationsCount, int seed)
        {
            Tuple<double, double> point = new Tuple<double, double>(0, 0); //начальная точка
            Tuple<double, double>[] triangle = new Tuple<double, double>[3];
            triangle[0] = Tuple.Create<double, double>(0, 10);
            triangle[1] = Tuple.Create<double, double>(10, -3);
            triangle[2] = Tuple.Create<double, double>(-10, -3);
            pixels.SetPixel(point.Item1, point.Item2);
            for (int i = 0; i < 3; i++)
                pixels.SetPixel(triangle[i].Item1, triangle[i].Item2);
            var random = new Random(seed);
            for (int i = 0; i < iterationsCount; i++)
            {
                var nextNumber = random.Next(3);
                point = Step(triangle[nextNumber], point);
                pixels.SetPixel(point.Item1, point.Item2);
            }
        }

        public static Tuple<double, double> Step(Tuple<double, double> dest, Tuple<double, double> point)
        {
            return new Tuple<double, double>((dest.Item1 + point.Item1) / 2,
                (dest.Item2 + point.Item2) / 2);
        }

    }
}
