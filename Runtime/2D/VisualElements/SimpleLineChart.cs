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
        private double[] _pointsY;
        private double[,] _pointsXY;

        private Func<List<double>> _getYValuesDelegates;
        private Func<List<Vector2>> _getXYValuesDelegates;

        /// <summary>
        /// Valeur min-max en Y (toute valeur dépassant ce seuil sera hors du graphe)
        /// </summary>
        public Vector2 yRange { get; set; } = Vector2.zero;

        /// <summary>
        /// Unidimensional mode, the points will be placed by the maximum avalaible interval on X axis
        /// If 500 px and 500 points, 1 point per pixel on X
        /// </summary>
        /// <param name="getPoints"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public SimpleLineChart(double[] pointsY)
        {
            _pointsY = pointsY;
            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateLineY;
        }

        public SimpleLineChart(double[,] pointXY)
        {
            _pointsXY = pointXY;
            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateLineXY;
        }

        public SimpleLineChart(Func<List<double>> getValuesDelegate)
        {
            _getYValuesDelegates = getValuesDelegate;
            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateLineYDynamic;
        }

        public SimpleLineChart(Func<List<Vector2>> getValuesDelegate)
        {
            _getXYValuesDelegates = getValuesDelegate;
            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateLineXYDynamic;
        }

        /// <summary>
        /// Generate the line without knowing any x value, so we assume a equal distribution of points on x and just compute the interval by pointsCount / avalaibleWidth 
        /// </summary>
        /// <param name="ctx"></param>
        protected void GenerateLineY(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            if (yRange == Vector2.zero)
                MathHelpers.ColumnMinMax(_pointsY, out y_min, out y_max);
            else
            {
                y_min = yRange.x;
                y_max = yRange.y;
            }

            x_min = 0;
            x_max = _pointsY.Length;

            painter2D.BeginPath();

            var relative_position_x = 0.0;
            var relative_position_y = MathHelpers.Lerp(_pointsY[0], y_min, y_max);

            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < _pointsY.Length; i++)
            {
                relative_position_x = MathHelpers.Lerp(i, x_min, x_max);
                relative_position_y = MathHelpers.Lerp(_pointsY[i], y_min, y_max);

                painter2D.LineTo(Plot(relative_position_x, relative_position_y));

            }

            painter2D.Stroke();
        }

        /// <summary>
        /// Generate the line without knowing any x value, so we assume a equal distribution of points on x and just compute the interval by pointsCount / avalaibleWidth 
        /// </summary>
        /// <param name="ctx"></param>
        protected void GenerateLineYDynamic(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            var points_y = _getYValuesDelegates();

            if (yRange == Vector2.zero)
                MathHelpers.ColumnMinMax(points_y, out y_min, out y_max);
            else
            {
                y_min = yRange.x;
                y_max = yRange.y;
            }

            x_min = 0;
            x_max = points_y.Count;

            painter2D.BeginPath();

            var relative_position_x = 0.0;
            var relative_position_y = MathHelpers.Lerp(points_y[0], y_min, y_max);

            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < points_y.Count; i++)
            {
                relative_position_x = MathHelpers.Lerp(i, x_min, x_max);
                relative_position_y = MathHelpers.Lerp(points_y[i], y_min, y_max);

                painter2D.LineTo(Plot(relative_position_x, relative_position_y));

            }

            painter2D.Stroke();
        }

        protected void GenerateLineXY(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;
        }

        protected void GenerateLineXYDynamic(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;
        }
    }
}
