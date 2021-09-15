using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace HW1
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,] matrixA = new double[,] { { 5, 1 }, { 3, 2 } };
            double[,] vectorY = new double[,] { { 2 }, { 6 } };
            var matrixE = MatrixOperations.IdentityMatrix(matrixA.GetLength(0));

            var EminusA = MatrixOperations.MatrixAddition(matrixE, MatrixOperations.MultiplyMatrixByNumber(matrixA, -1));
            var EminusAinverse = MatrixOperations.ArrayOfArraysToMatrix(MatrixOperations.MatrixInverse(MatrixOperations.MatrixToArrayOfArrays(EminusA)));
            var result = MatrixOperations.MultiplyMatrices(EminusAinverse, vectorY);

            MatrixOperations.MatrixToConsole(result);
        }
    }
}
