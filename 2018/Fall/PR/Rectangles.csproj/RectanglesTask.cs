using System;

namespace Rectangles
{
    public static class RectanglesTask
    {
        // Пересекаются ли два прямоугольника (пересечение только по границе также считается пересечением)
        public static bool AreIntersected(Rectangle r1, Rectangle r2)
        {
            return Math.Max(r1.Left, r2.Left) <= Math.Min(r1.Right, r2.Right)
                && Math.Max(r1.Top, r2.Top) <= Math.Min(r1.Bottom, r2.Bottom);
        }

        // Площадь пересечения прямоугольников
        public static int IntersectionSquare(Rectangle r1, Rectangle r2)
        {
            if (AreIntersected(r1, r2))
            {
                return (Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left))
                    * (Math.Min(r1.Bottom, r2.Bottom) - Math.Max(r1.Top, r2.Top));
            }
            else
                return 0;
        }

        // Если один из прямоугольников целиком находится внутри другого — вернуть номер (с нуля) внутреннего.
        // Иначе вернуть -1
        // Если прямоугольники совпадают, можно вернуть номер любого из них.
        public static int IndexOfInnerRectangle(Rectangle r1, Rectangle r2)
        {
            if (r1.Top <= r2.Top && r1.Bottom >= r2.Bottom
                && r1.Right >= r2.Right && r1.Left <= r2.Left)
                return 1;
            else if (r2.Top <= r1.Top && r2.Bottom >= r1.Bottom
                && r2.Right >= r1.Right && r2.Left <= r1.Left)
                return 0;
            else return -1;
        }
    }
}