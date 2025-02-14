using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atomix.ChartBuilder.Math
{
    public static class MathHelpers
    {
        public static double Lerp(double value, double v_min, double v_max)
        {
            return Map(value, v_min, v_max, 0, 1);
        }

        public static double Map(double value, double inputMin, double inputMax, double outputMin, double outputMax)
        {
            return (value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;
        }

        public static void ColumnMinMax(double[,] matrix, int columnIndex, out double min, out double max)
        {
            max = double.MinValue;
            min = double.MaxValue;

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                max = System.Math.Max(matrix[i, columnIndex], max);
                min = System.Math.Min(matrix[i, columnIndex], min);
            }
        }

        public static void ColumnMinMax(List<double> vector, out double min, out double max)
        {
            max = double.MinValue;
            min = double.MaxValue;

            for (int i = 0; i < vector.Count; ++i)
            {
                max = System.Math.Max(vector[i], max);
                min = System.Math.Min(vector[i], min);
            }
        }

        public static void ColumnMinMax(double[] vector, out double min, out double max)
        {
            max = double.MinValue;
            min = double.MaxValue;

            for (int i = 0; i < vector.Length; ++i)
            {
                max = System.Math.Max(vector[i], max);
                min = System.Math.Min(vector[i], min);
            }
        }

        public static void ColumnMinMax(double[,] matrix, int columnIndex, out Vector2Double minMax)
        {
            minMax = new Vector2Double(double.MaxValue, double.MinValue);

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                minMax.x = System.Math.Min(matrix[i, columnIndex], minMax.x);
                minMax.y = System.Math.Max(matrix[i, columnIndex], minMax.y);
            }
        }

        public static void RowMinMax(double[,] matrix, int startIndex, out Vector2Double minMax)
        {
            minMax = new Vector2Double(double.MaxValue, double.MinValue);

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for(int j = startIndex; j < matrix.GetLength(1); ++j)
                {
                    minMax.x = System.Math.Min(matrix[i, j], minMax.x);
                    minMax.y = System.Math.Max(matrix[i, j], minMax.y);
                }
            }
        }


        public static void ColumnMinMax(List<double> vector, out Vector2Double minMax)
        {
            minMax = new Vector2Double(double.MaxValue, double.MinValue);

            for (int i = 0; i < vector.Count; ++i)
            {
                minMax.y = System.Math.Max(vector[i], minMax.y);
                minMax.x = System.Math.Min(vector[i], minMax.x);
            }
        }

        public static void ColumnMinMax(double[] vector, out Vector2Double minMax)
        {
            minMax = new Vector2Double(double.MaxValue, double.MinValue);

            for (int i = 0; i < vector.Length; ++i)
            {
                minMax.y = System.Math.Max(vector[i], minMax.y);
                minMax.x = System.Math.Min(vector[i], minMax.x);
            }
        }
    }
}
