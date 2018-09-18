using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expr11
{
    class Program
    {
        static void Main(string[] args)
        {
            string time = Console.ReadLine();
            string[] a = time.Split(':');
            int hours = int.Parse(a[0]);
            int minutes = int.Parse(a[1]);
            int angle = Math.Abs((((hours * 60 + minutes) / 2) % 360) - minutes * 6);
            if(angle > 180)
            {
                angle = 180 - angle;
            }
            Console.WriteLine(angle);
        }
    }
}
