using Atomix.ChartBuilder.Math;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder.VisualElements
{
    public class Scatter2DChart : ChartBaseElement
    {
        private float _dotRadius = 5;

        private Func<double[,]> _getPoints;

        /// <summary>
        /// Assuming a matrix of N-Rows and 2-Columns (X and Y value)
        /// </summary>
        /// <param name="getPoints"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Scatter2DChart(Func<double[,]> getPoints, int width = 300, int height = 300)
        {
            _getPoints = getPoints;

            style.backgroundColor = new StyleColor(Color.white);

            generateVisualContent += GenerateGradientColoredScatter;
            generateVisualContent += DrawOrthonormalLines_BottomLeftAnchored;
        }

        protected void GenerateGradientColoredScatter(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = 2f;
            painter2D.strokeColor = Color.white;

            var points = _getPoints();

            if (points.Length == 0)
                return;

            if (points.GetLength(1) != 2)
                throw new Exception($"Scatter2D requires only 2 column matrix");

            MathHelpers.ColumnMinMax(points, 0, out x_min, out x_max);
            MathHelpers.ColumnMinMax(points, 1, out y_min, out y_max);

            for (int i = 0; i < points.GetLength(0); ++i)
            {
                var relative_position_x = MathHelpers.Lerp(points[i, 0], x_min, x_max);
                var relative_position_y = 1 - MathHelpers.Lerp(points[i, 1], y_min, y_max);

                painter2D.BeginPath();
                painter2D.Arc(Plot(relative_position_x, relative_position_y), _dotRadius, 0, 360);

                // coloriser avec gradient

                var color_x = VisualizationSheet.visualizationSettings.warmGradient.Evaluate((float)relative_position_x);
                var color_y = VisualizationSheet.visualizationSettings.coldGradient.Evaluate((float)relative_position_y);

                float mean = (float)(relative_position_x + relative_position_y) / 2;

                painter2D.fillColor = Color.Lerp(color_x, color_y, mean);

                painter2D.Fill();
            }
        }
    }
}
