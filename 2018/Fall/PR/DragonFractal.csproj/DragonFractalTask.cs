// В этом пространстве имен содержатся средства для работы с изображениями. Чтобы оно стало доступно, в проект был подключен Reference на сборку System.Drawing.dll
using System;
using System.Drawing;

namespace Fractals
{
	internal static class DragonFractalTask
	{
		public static void DrawDragonFractal(Pixels pixels, int iterationsCount, int seed)
		{
            /*
			Начните с точки (1, 0)
			Создайте генератор рандомных чисел с сидом seed
			
			На каждой итерации:

			1. Выберите случайно одно из следующих преобразований и примените его к текущей точке:

				Преобразование 1. (поворот на 45° и сжатие в sqrt(2) раз):
				x' = (x · cos(45°) - y · sin(45°)) / sqrt(2)
				y' = (x · sin(45°) + y · cos(45°)) / sqrt(2)

				Преобразование 2. (поворот на 135°, сжатие в sqrt(2) раз, сдвиг по X на единицу):
				x' = (x · cos(135°) - y · sin(135°)) / sqrt(2) + 1
				y' = (x · sin(135°) + y · cos(135°)) / sqrt(2)
		
			2. Нарисуйте текущую точку методом pixels.SetPixel(x, y)

			*/
            Tuple<double, double> point = new Tuple<double, double>(1, 0);
            pixels.SetPixel(point.Item1, point.Item2);
            var random = new Random(seed);
            for (int i = 0; i < iterationsCount; i++)
            {
                var nextNumber = random.Next(2);
                if (nextNumber == 0)
                    point = Compress(point);
                else
                    point = Shift(point);
                pixels.SetPixel(point.Item1, point.Item2);
            }
        }

        //Преобразование 1
        public static Tuple<double, double> Compress(Tuple<double, double> point)
        {
            return new Tuple<double, double>((point.Item1 - point.Item2) / 2,
                (point.Item1 + point.Item2) / 2);
        }

        //Преобразование 2
        public static Tuple<double, double> Shift(Tuple<double, double> point)
        {
            return new Tuple<double, double>((-point.Item1 - point.Item2) / 2 + 1,
                (point.Item1 - point.Item2) / 2);
        }
    }
}