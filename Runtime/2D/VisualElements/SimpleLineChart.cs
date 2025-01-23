using Atomix.ChartBuilder.Math;
using Atomix.ChartBuilder.VisualElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder.VisualElements
{
    /// <summary>
    /// A simple graphic to input
    /// </summary>
    public class SimpleLineChart : ChartBaseElement
    {
        /// <summary>
        /// Creates a function graph on the interval
        /// </summary>
        /// <param name="function"></param>
        /// <param name="min_max_x"></param>
        public SimpleLineChart(Func<double, double> function, Vector2Double interval_x)
        {
            current_range_x = interval_x;

            backgroundColor = _backgroundColor;
            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateFunctionLine(meshGenerationContext, function, interval_x);
            };
        }

        /// <summary>
        /// Unidimensional mode, the points will be placed by the maximum avalaible interval on X axis
        /// If 500 px and 500 points, 1 point per pixel on X
        /// </summary>
        /// <param name="getPoints"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public SimpleLineChart(double[] pointsY)
        {
            onRefresh += () => InitRange_pointsY(pointsY);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateLineY(meshGenerationContext, pointsY, true);
            };
        }

        public SimpleLineChart(double[,] pointXY)
        {
            onRefresh += () => InitRange_pointsXY(pointXY);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateLineXY(meshGenerationContext, pointXY, true);
            };
        }

    }
}
