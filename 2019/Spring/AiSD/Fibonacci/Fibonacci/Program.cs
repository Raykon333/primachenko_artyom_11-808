using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fibonacci
{
    class Matrix
    {
        public int[,] Elements;

        public int GetLength(int dimension)
        {
            return Elements.GetLength(dimension);
        }

        public static Matrix GetIdentityMatrix(int size)
        {
            Matrix result = new Matrix() { Elements = new int[size, size] };
            for (int i = 0; i < size; i++)
            {
                result.Elements[i, i] = 1;
            }
            return result;
        }

        public static Matrix ProductionOf(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.GetLength(1) != matrix2.GetLength(0))
                throw new InvalidOperationException();
            int[,] result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    int sum = 0;
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                        sum += matrix1.Elements[i, k] * matrix2.Elements[k, j];
                    result[i, j] = sum;
                }
            }
            return new Matrix { Elements = result };
        }

        public Matrix Squared()
        {
            if (Elements.GetLength(0) != Elements.GetLength(1))
                throw new InvalidOperationException();
            return Matrix.ProductionOf(this, this);
        }

        public Matrix RaisedToAPowerOf(int power)
        {
            if (Elements.GetLength(0) != Elements.GetLength(1) || power < 0)
                throw new InvalidOperationException();
            Matrix empowered = this;
            Matrix result = Matrix.GetIdentityMatrix(this.GetLength(0));
            while(power > 0)
            {
                if (power % 2 == 1)
                    result = Matrix.ProductionOf(result, empowered);
                empowered = empowered.Squared();
                power = power / 2;
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Matrix matrix = new Matrix() { Elements = new int[2, 2] { { 0, 1 }, { 1, 1 } } };
            matrix = matrix.RaisedToAPowerOf(10);
            Console.WriteLine(matrix.Elements[0, 0] + " " + matrix.Elements[0, 1]);
            Console.WriteLine(matrix.Elements[1, 0] + " " + matrix.Elements[1, 1]);
        }
    }
}
