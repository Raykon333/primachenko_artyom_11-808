using System;

namespace DistanceTask
{
	public static class DistanceTask
	{
        static Tuple<double, double, double, double> FindSides(double ax, double ay,
            double bx, double by, double x, double y)
        {
            double a = Math.Sqrt((bx - x) * (bx - x) + (by - y) * (by - y));
            double b = Math.Sqrt((x - ax) * (x - ax) + (y - ay) * (y - ay));
            double c = Math.Sqrt((bx - ax) * (bx - ax) + (by - ay) * (by - ay));
            double p = (a + b + c) / 2;
            double h = 2 * Math.Sqrt(p * (p - a) * (p - b) * (p - c)) / c;
            return Tuple.Create(a, b, c, h);
        }

        static bool IsCosNegative(double a, double b, double c)
        {
            return (b * b + c * c - a * a) / (2 * b * c) <= 0;
        }

        // Расстояние от точки (x, y) до отрезка AB с координатами A(ax, ay), B(bx, by)
        public static double GetDistanceToSegment(double ax, double ay, double bx, double by, double x, double y)
		{
            var sides = FindSides(ax, ay, bx, by, x, y);
            double distance;
            if (IsCosNegative(sides.Item1, sides.Item2, sides.Item3))
                distance = sides.Item2;
            else if (IsCosNegative(sides.Item2, sides.Item1, sides.Item3))
                distance = sides.Item1;
            else distance = sides.Item4;
            if (sides.Item3 == 0)
                return sides.Item1;
            else return distance;
		}
	}
}