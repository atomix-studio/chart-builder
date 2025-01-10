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
        public override Vector2Double dynamic_range_y { get; set; }
        public override Vector2Double dynamic_range_x { get; set; }

        /// <summary>
        /// Assuming a matrix of N-Rows and 2-Columns (X and Y value)
        /// </summary>
        /// <param name="getPoints"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Scatter2DChart(Func<double[,]> getPoints)
        {
            _getPoints = getPoints;

            InitDynamicRange(_getPoints());

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateGradientColoredScatter;
            generateVisualContent += DrawOrthonormalLines_BottomLeftAnchored;
        }

        protected void GenerateGradientColoredScatter(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = Color.white;

            var points = _getPoints();

            if (points.Length == 0)
                return;

            if (points.GetLength(1) != 2)
                throw new Exception($"Scatter2D requires only 2 column matrix");

            InitDynamicRange(points);

            for (int i = 0; i < points.GetLength(0); ++i)
            {
                var relative_position_x = MathHelpers.Lerp(points[i, 0], dynamic_range_x.x, dynamic_range_x.y);
                var relative_position_y = MathHelpers.Lerp(points[i, 1], dynamic_range_y.x, dynamic_range_y.y);

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

        private void InitDynamicRange(double[,] points)
        {
            MathHelpers.ColumnMinMax(points, 0, out var range_x);
            MathHelpers.ColumnMinMax(points, 1, out var range_y);

            dynamic_range_x = range_x;
            dynamic_range_y = range_y;
        }
    }
}
