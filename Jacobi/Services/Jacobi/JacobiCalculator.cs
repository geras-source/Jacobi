using Jacobi.Services.Storage;
using System;

namespace Jacobi.Services.Jacobi;

public class JacobiCalculator : IJacobiCalculator
{
    private readonly IAnswerStorage _answerStorage;

    public JacobiCalculator(IAnswerStorage answerStorage)
    {
        _answerStorage = answerStorage;
    }

    public void Calculate(double[,] matrixA)
    {
        var iteration = 0;
        var n = matrixA.GetLength(0);
        var currentS = SumOfNonDiagonalSquares(matrixA);
        const double tolerance = 1e-8;
        var vectorMatrix = new double[n, n];
        var values = new double[n];

        for (var i = 0; i < n; i++)
        {
            vectorMatrix[i, i] = 1;
        }

        while (currentS > tolerance)
        {
            FindMaxOffDiagonal(matrixA, out var p, out var q);
            var theta = ComputeRotationAngle(matrixA, p, q);

            var previousMatrixValue = matrixA[p, q];
            matrixA[p, q] = 0;
            matrixA[p, p] -= previousMatrixValue * theta;
            matrixA[q, q] += previousMatrixValue * theta;

            var cos = 1 / Math.Sqrt(1 + Math.Pow(theta, 2));
            var sin = theta * cos;

            for (var r = 0; r < n; r++)
            {
                if (r != p && r != q)
                {
                    matrixA[r, p] = matrixA[p, r] =
                        matrixA[r, p] - sin * (matrixA[r, q] + sin * matrixA[r, p] / (1 + cos));
                    matrixA[r, q] = matrixA[q, r] =
                        matrixA[r, q] + sin * (matrixA[r, p] - cos * matrixA[r, q] / (1 + cos));
                }

                vectorMatrix[r, p] = vectorMatrix[r, p] * cos - vectorMatrix[r, q] * sin;
                vectorMatrix[r, q] = vectorMatrix[r, p] * sin + vectorMatrix[r, q] * cos;
            }

            currentS -= Math.Pow(previousMatrixValue, 2);

            iteration++;
        }

        for (var i = 0; i < n; i++)
        {
            values[i] = matrixA[i, i];
        }

        var data = new List<double[]>();

        int rows = vectorMatrix.GetLength(0);
        int cols = vectorMatrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            double[] row = new double[cols];
            for (int j = 0; j < cols; j++)
            {
                row[j] = vectorMatrix[i, j]; // Копируем значения из массива double[,] в массив double[]
            }
            data.Add(row); // Добавляем массив в список
        }

        var answer = new Answer
        {
            Matrix = data,
            Values = values
        };

        _answerStorage.Add(answer);
    }

    double[,] CreateRotationMatrix(int n, int p, int q, double theta)
    {
        var rotationMatrix = new double[n, n];
        var cos = 1 / Math.Sqrt(1 + Math.Pow(theta, 2));
        var sin = theta * cos;

        for (var i = 0; i < n; i++) rotationMatrix[i, i] = 1.0;

        rotationMatrix[p, p] = cos;
        rotationMatrix[p, q] = -sin;
        rotationMatrix[q, p] = sin;
        rotationMatrix[q, q] = cos;

        return rotationMatrix;
    }

    double ComputeRotationAngle(double[,] A, int p, int q)
    {
        var c = (A[p, p] - A[q, q]) / (2 * A[p, q]);

        if (c >= 0)
        {
            return 1 / (c + Math.Sqrt(Math.Pow(c, 2) + 1));
        }

        return 1 / (c - Math.Sqrt(Math.Pow(c, 2) + 1));
    }

    void FindMaxOffDiagonal(double[,] A, out int p, out int q)
    {
        var n = A.GetLength(0);
        double max = 0;
        p = 0;
        q = 0;
        for (var i = 0; i < n - 1; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (i != j && Math.Abs(A[i, j]) > max)
                {
                    max = Math.Abs(A[i, j]);
                    p = i;
                    q = j;
                }
            }
        }
    }
    double SumOfNonDiagonalSquares(double[,] A)
    {
        int n = A.GetLength(0);
        double sum = 0;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (i != j) sum += Math.Pow(A[i, j], 2);
            }
        }

        return sum;
    }
}