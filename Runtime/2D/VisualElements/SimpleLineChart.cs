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
        private Func<double> _function;
        private double[] _pointsY;
        private double[,] _pointsXY;

        private List<double> _pointsYList;
        private List<Vector2> _pointsXYList;

        /// <summary>
        /// Creates a function graph on the interval
        /// </summary>
        /// <param name="function"></param>
        /// <param name="min_max_x"></param>
        public SimpleLineChart(Func<double> function, Vector2Double interval_x)
        {
            _function = function;
            current_range_x = interval_x;

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateFunctionLine;
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
            _pointsY = pointsY;
            InitArrayRangeY();

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateLineY;
        }

        public SimpleLineChart(double[,] pointXY)
        {
            _pointsXY = pointXY;
            InitArrayRangeXY();

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateLineXY;
        }

        /* public SimpleLineChart(List<double> getValuesDelegate)
         {
             _pointsYList = getValuesDelegate;
             InitArrayRangeY();
             backgroundColor = _backgroundColor;
             generateVisualContent += GenerateLineYDynamic;
         }*/

        /*public SimpleLineChart(List<Vector2> getValuesDelegate)
        {
            _pointsXYList = getValuesDelegate;
            InitArrayRangeXY();

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateLineXYDynamic;
        }*/

        private void InitArrayRangeY()
        {
            current_range_x = new Vector2Double(0, _pointsY.Length);

            MathHelpers.ColumnMinMax(_pointsY, out var range_y);
            current_range_y = range_y;
        }

        private void InitArrayRangeXY()
        {
            MathHelpers.ColumnMinMax(_pointsXY, 0, out var range_x);
            current_range_x = range_x;

            MathHelpers.ColumnMinMax(_pointsXY, 1, out var range_y);
            current_range_y = range_y;
        }

        /// <summary>
        /// Generate the line without knowing any x value, so we assume a equal distribution of points on x and just compute the interval by pointsCount / avalaibleWidth 
        /// </summary>
        /// <param name="ctx"></param>
        protected void GenerateFunctionLine(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            // compute y range

            // calculate interval of values from a given number of points

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

        /// <summary>
        /// Generate the line without knowing any x value, so we assume a equal distribution of points on x and just compute the interval by pointsCount / avalaibleWidth 
        /// </summary>
        /// <param name="ctx"></param>
        protected void GenerateLineY(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            InitArrayRangeY();

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

        protected void GenerateLineXY(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            InitArrayRangeXY();

            painter2D.BeginPath();

            var relative_position_x = MathHelpers.Lerp(_pointsXY[0, 0], current_range_x.x, current_range_x.y);
            var relative_position_y = MathHelpers.Lerp(_pointsXY[0, 1], current_range_y.x, current_range_y.y);

            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < _pointsXY.GetLength(0); i++)
            {
                relative_position_x = MathHelpers.Lerp(_pointsXY[i, 0], current_range_x.x, current_range_x.y);
                relative_position_y = MathHelpers.Lerp(_pointsXY[i, 1], current_range_y.x, current_range_y.y);

                painter2D.LineTo(Plot(relative_position_x, relative_position_y));
            }

            painter2D.Stroke();
        }

        protected void GenerateLineXYDynamic(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;
        }
    }
}
