using System;
using System.Diagnostics;
using System.Drawing;

namespace RefactorMe
{
    // ## Прочитайте! ##
    //
    // Ваша задача привести код в этом файле в порядок. 
    // Для начала запустите эту программу. Для этого в VS в проект подключите сборку System.Drawing.

    // Переименуйте всё, что называется неправильно. Это можно делать комбинацией клавиш Ctrl+R, Ctrl+R (дважды нажать Ctrl+R).
    // Повторяющиеся части кода вынесите во вспомогательные методы. Это можно сделать выделив несколько строк кода и нажав Ctrl+R, Ctrl+M
    // Избавьтесь от всех зашитых в коде числовых констант — положите их в переменные с понятными именами.
    // 
    // После наведения порядка проверьте, что ваш код стал лучше. 
    // На сколько проще после ваших переделок стало изменить размер фигуры? Повернуть её на некоторый угол? 
    // Научиться рисовать невозможный треугольник, вместо квадрата?

    class Risovatel
    {
        static Bitmap Image = new Bitmap(800, 600);
        static float x, y;
        static Graphics Graphics;

        public static void Initialize()
        {
            Image = new Bitmap(800, 600);
            Graphics = Graphics.FromImage(Image);
        }

        public static void SetPos(float x0, float y0)
        {
            x = x0;
            y = y0;
        }

        public static void Go(double L, double angle)
        {
            //Делает шаг длиной L в направлении angle и рисует пройденную траекторию
            var x1 = (float)(x + L * Math.Cos(angle));
            var y1 = (float)(y + L * Math.Sin(angle));
            Graphics.DrawLine(Pens.Black, x, y, x1, y1);
            x = x1;
            y = y1;
        }

        public static void ShowResult()
        {
            Image.Save("result.bmp");
            Process.Start("result.bmp");
        }
    }

    public class StrangeThing
    {
        const int size = 40;
        static int[] Pos = new int[] { 0, 1 * size, 12 * size, 11 * size, };

        public static void Main()
        {
            Risovatel.Initialize();

            for (int i = 0; i < 4; i++)
            {
                DrawPart(i);
            }

            Risovatel.ShowResult();
        }
        public static void DrawPart(int i)
        {
            double angle = Math.PI / 2 * i;
            Risovatel.SetPos(Pos[(i + 1) % 4], Pos[i % 4]);
            Risovatel.Go(10 * size, angle);
            Risovatel.Go(1 * size * Math.Sqrt(2), Math.PI / 4 + angle);
            Risovatel.Go(10 * size, Math.PI + angle);
            Risovatel.Go(10 * size - (double)1 * size, Math.PI / 2 + angle);
        }
    }
}

/*//Рисуем четыре одинаковые части невозможного квадрата.
// Часть первая:
Risovatel.SetPos(10, 0);
Risovatel.Go(100, 0);
Risovatel.Go(10 * Math.Sqrt(2), Math.PI / 4);
Risovatel.Go(100, Math.PI);
Risovatel.Go(100 - (double) 10, Math.PI / 2);

// Часть вторая:
Risovatel.SetPos(120, 10);
Risovatel.Go(100, Math.PI / 2);
Risovatel.Go(10 * Math.Sqrt(2), Math.PI / 2 + Math.PI / 4);
Risovatel.Go(100, Math.PI / 2 + Math.PI);
Risovatel.Go(100 - (double) 10, Math.PI / 2 + Math.PI / 2);

// Часть третья:
Risovatel.SetPos(110, 120);
Risovatel.Go(100, Math.PI);
Risovatel.Go(10 * Math.Sqrt(2), Math.PI + Math.PI / 4);
Risovatel.Go(100, Math.PI + Math.PI);
Risovatel.Go(100 - (double) 10, Math.PI + Math.PI / 2);

// Часть четвертая:
Risovatel.SetPos(0, 110);
Risovatel.Go(100, -Math.PI / 2);
Risovatel.Go(10 * Math.Sqrt(2), -Math.PI / 2 + Math.PI / 4);
Risovatel.Go(100, -Math.PI / 2 + Math.PI);
Risovatel.Go(100 - (double) 10, -Math.PI / 2 + Math.PI / 2);*/
