using System;
using System.Collections.Generic;

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

            for(int i = 0; i < matrix.GetLength(0); ++i)
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
    }
}
