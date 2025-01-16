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

        public override Vector2Double current_range_y { get ; set; }
        public override Vector2Double current_range_x { get ; set; }

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

            InitDynamicRangeY();

            current_range_x = new Vector2Double(0, _pointsY.Length);

            painter2D.BeginPath();

            var relative_position_x = 0.0;
            var relative_position_y = MathHelpers.Lerp(_pointsY[0], current_range_y.x, current_range_y.y);

            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < _pointsY.Length; i++)
            {
                relative_position_x = MathHelpers.Lerp(i, current_range_x.x, current_range_x.y);
                relative_position_y = MathHelpers.Lerp(_pointsY[i], current_range_y.x, current_range_y.y);

                painter2D.LineTo(Plot(relative_position_x, relative_position_y));

            }

            painter2D.Stroke();
        }

        private void InitDynamicRangeY()
        {
            if (fixed_range_y == Vector2Double.zero)
            {
                MathHelpers.ColumnMinMax(_pointsY, out var range_y);
                current_range_y = range_y;
            }
            else
            {
                current_range_y = fixed_range_y;
            }
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

            InitDynamicRangeY();

            current_range_x = new Vector2Double(0, points_y.Count);

            painter2D.BeginPath();

            var relative_position_x = 0.0;
            var relative_position_y = MathHelpers.Lerp(_pointsY[0], current_range_y.x, current_range_y.y);

            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < points_y.Count; i++)
            {
                relative_position_x = MathHelpers.Lerp(i, current_range_x.x, current_range_x.y);
                relative_position_y = MathHelpers.Lerp(points_y[i], current_range_y.x, current_range_y.y);

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
