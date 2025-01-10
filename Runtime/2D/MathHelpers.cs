using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.MachineLearning.Core.Maths
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
                max = Math.Max(matrix[i, columnIndex], max);
                min = Math.Min(matrix[i, columnIndex], min);
            }
        }

        public static void ColumnMinMax(List<double> vector, out double min, out double max)
        {
            max = double.MinValue;
            min = double.MaxValue;

            for (int i = 0; i < vector.Count; ++i)
            {
                max = Math.Max(vector[i], max);
                min = Math.Min(vector[i], min);
            }
        }

        public static void ColumnMinMax(double[] vector, out double min, out double max)
        {
            max = double.MinValue;
            min = double.MaxValue;

            for (int i = 0; i < vector.Length; ++i)
            {
                max = Math.Max(vector[i], max);
                min = Math.Min(vector[i], min);
            }
        }
    }
}
