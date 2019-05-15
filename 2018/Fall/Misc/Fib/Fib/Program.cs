using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fib
{
    class Matrix
    {
        public int[,] matrix = new int[2, 2];

        public void Write()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    Console.WriteLine(matrix[i, j]);
        }

        public int[,] GetSquared()
        {
            return GetProduction(matrix, matrix);
        }

        public static int[,] GetProduction(int[,] matrix1, int[,] matrix2)
        {
            if (matrix1.GetLength(1) != matrix2.GetLength(0))
                throw new Exception();
            int[,] result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int k = 0; k < matrix2.GetLength(1); k++)
                {
                    int sum = 0;
                    for (int j = 0; j < matrix2.GetLength(0); j++)
                    {
                        sum += matrix1[i, j] * matrix2[j, k];
                    }
                    result[i, k] = sum;
                }
            }
            return result;
        }

        public static int[,] PowerOf(Matrix matrix, int power)
        {
            int[,] result = new int[,] { { 1, 0 }, { 0, 1 } };
            for(int i = (int)Math.Ceiling(Math.Log(power + 1, 2)) - 1; i >= 0; i--)
            {
                int bin = power % 2;
                power /= 2;
                if (bin == 1)
                    result = Matrix.GetProduction(result, matrix.matrix);
                matrix = new Matrix { matrix = matrix.GetSquared() };
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Matrix x = new Matrix();
            x.matrix = new int[,] { { 0, 1 }, { 1, 1 } };
            int[,] y = new int[2, 1] { { 1 }, { 1 } };
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine(Matrix.GetProduction(Matrix.PowerOf(x, n - 1), y)[0, 0]);
        }
    }
}
