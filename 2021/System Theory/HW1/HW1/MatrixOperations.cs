using System;
using System.Collections.Generic;
using System.Text;

namespace HW1
{
    public class MatrixOperations
    {
        public static double[][] MatrixInverse(double[][] matrix)
        {
            if (MatrixDeterminant(matrix) == 0)
            {
                throw new ArgumentException();
            }

            int n = matrix.Length;
            double[][] result = MatrixCreate(n, n); // make a copy of matrix
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i][j] = matrix[i][j];

            double[][] lum; // combined lower & upper
            int[] perm;
            int toggle;
            toggle = MatrixDecompose(matrix, out lum, out perm);

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                double[] x = Helper(lum, b); // 
                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        } // MatrixInverse

        public static int MatrixDecompose(double[][] m, out double[][] lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum[][]
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = m.Length;

            // make a copy of m[][] into result lu[][]
            lum = MatrixCreate(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    lum[i][j] = m[i][j];


            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            for (int j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                double max = Math.Abs(lum[j][j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i][j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                } // i

                if (piv != j)
                {
                    double[] tmp = lum[piv]; // swap rows j, piv
                    lum[piv] = lum[j];
                    lum[j] = tmp;

                    int t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j][j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i][j] / xjj;
                        lum[i][j] = xij;
                        for (int k = j + 1; k < n; ++k)
                            lum[i][k] -= xij * lum[j][k];
                    }
                }

            } // j

            return toggle;
        } // MatrixDecompose

        public static double[] Helper(double[][] luMatrix, double[] b) // helper
        {
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        } // Helper

        public static double MatrixDeterminant(double[][] matrix)
        {
            double[][] lum;
            int[] perm;
            int toggle = MatrixDecompose(matrix, out lum, out perm);
            double result = toggle;
            for (int i = 0; i < lum.Length; ++i)
                result *= lum[i][i];
            return result;
        }

        public static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        public static double[,] MultiplyMatrices(double[,] leftMatrix, double[,] rightMatrix)
        {
            if (leftMatrix.GetLength(1) != rightMatrix.GetLength(0))
                throw new ArgumentException();
            int conformNumber = leftMatrix.GetLength(1);

            int resultX = leftMatrix.GetLength(0);
            int resultY = rightMatrix.GetLength(1);
            var resultMatrix = new double[resultX, resultY];

            for (int i = 0; i < resultX; i++)
            {
                for (int j = 0; j < resultY; j++)
                {
                    double cellValue = 0;
                    for (int k = 0; k < conformNumber; k++)
                        cellValue += leftMatrix[i, k] * rightMatrix[k, j];
                    resultMatrix[i, j] = cellValue;
                }
            }

            return resultMatrix;
        }

        public static void MatrixToConsole(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(String.Format("{0:0.##}", matrix[i, j]) + "\t");
                }
                Console.Write("\n");
            }
        }

        public static double[,] MatrixAddition(double[,] matrix1, double[,] matrix2)
        {
            if (matrix1.GetLength(0) != matrix2.GetLength(0)
                || matrix1.GetLength(1) != matrix2.GetLength(1))
                throw new ArgumentException();

            var result = new double[matrix1.GetLength(0), matrix2.GetLength(1)];

            for (int i = 0; i < matrix1.GetLength(0); i++)
                for (int j = 0; j < matrix1.GetLength(1); j++)
                    result[i, j] = matrix1[i, j] + matrix2[i, j];

            return result;
        }

        public static double[,] MultiplyMatrixByNumber(double[,] matrix, int number)
        {
            var result = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    result[i, j] = matrix[i, j] * number;

            return result;
        }

        public static double[,] IdentityMatrix(int size)
        {
            var result = new double[size, size];
            for (int i = 0; i < size; i++)
                result[i, i] = 1;
            return result;
        }

        public static double[][] MatrixToArrayOfArrays(double[,] matrix)
        {
            var result = MatrixCreate(matrix.GetLength(0), matrix.GetLength(1));
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    result[i][j] = matrix[i, j];
            return result;
        }

        public static double[,] ArrayOfArraysToMatrix(double[][] array)
        {
            var result = new double[array.Length, array[0].Length];
            for (int i = 0; i < array.Length; i++)
                for (int j = 0; j < array[0].Length; j++)
                    result[i, j] = array[i][j];
            return result;
        }
    }
}
