using Atomix.ChartBuilder.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder.VisualElements
{
    public class Scatter2DChart : ChartBaseElement
    {
        private float _dotRadius = 5;

        private Func<double[,]> _getPoints;
        private Func<Dictionary<Color, double[,]>> _getClassPoints;
        private Func<Dictionary<double[,], double>> _getValuePoints;

        private Dictionary<Vector2, Vector2Double> _plottedPositions = new Dictionary<Vector2, Vector2Double>();

        public override Vector2Double current_range_y { get; set; }
        public override Vector2Double current_range_x { get; set; }

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

            this.RegisterCallback<MouseMoveEvent>(evt => OnHoverMove(evt), TrickleDown.TrickleDown);
        }

        /// <summary>
        /// Gradient based scatter, with normalized position as gradient inputs
        /// </summary>
        /// <param name="points"></param>
        public Scatter2DChart(double[,] points)
        {
            _getPoints = () => points;

            InitDynamicRange(_getPoints());

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateGradientColoredScatter;

            this.RegisterCallback<MouseMoveEvent>(evt => OnHoverMove(evt), TrickleDown.TrickleDown);
        }

        /// <summary>
        /// Takes a dictionnary of points with a color for each collection of points
        /// </summary>
        /// <param name="classedPoints"></param>
        public Scatter2DChart(Dictionary<Color, double[,]> classedPoints)
        {
            _getClassPoints = () => classedPoints;

            InitDynamicRange(classedPoints);

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateClassColoredScatter;

            this.RegisterCallback<MouseMoveEvent>(evt => OnHoverMove(evt), TrickleDown.TrickleDown);
            this.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                TryRemoveCurrentLabel();
            });
        }

        /// <summary>
        /// Takles a matrix and color each point with normalized Element value with VisualizationSheet.visualizationSettings.
        /// </summary>
        /// <param name="classedPoints"></param>
        public Scatter2DChart(Dictionary<double[,], double> classedPoints)
        {
            _getValuePoints = () => classedPoints;

            InitDynamicRange(classedPoints);

            backgroundColor = _backgroundColor;
            generateVisualContent += GenerateValueDrivenGradientColoredScatter;

            this.RegisterCallback<MouseMoveEvent>(evt => OnHoverMove(evt), TrickleDown.TrickleDown);
            this.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                TryRemoveCurrentLabel();
                _is_computingHoverMove = false;
            });
        }

        private bool _is_computingHoverMove;

        private async void OnHoverMove(MouseMoveEvent mouseEnterEvent)
        {
            if (_is_computingHoverMove)
                return;

            _is_computingHoverMove = true;

            var local_mouse_pos = this.WorldToLocal(mouseEnterEvent.mousePosition);
            var dist = float.MaxValue;
            var pos_key = Vector2.zero;
            var pos_value = Vector2Double.zero;

            await Task.Run(() =>
            {
                foreach (var plot in _plottedPositions)
                {
                    var crt = (local_mouse_pos - plot.Key).magnitude;
                    if (crt < dist)
                    {
                        dist = crt;
                        pos_key = plot.Key;
                        pos_value = plot.Value;
                    }
                }
            });

            TryRemoveCurrentLabel();

            var label = new Label(pos_value.ToString());
            label.name = "CURRENT_HOVER";
            label.style.position = new StyleEnum<Position>(Position.Absolute);
            label.style.left = pos_key.x;
            label.style.top = pos_key.y;
            label.AddToClassList("hover-text");
            this.Add(label);

            this.MarkDirtyRepaint();

            _is_computingHoverMove = false;
        }

        private void TryRemoveCurrentLabel()
        {
            var crt_hover = this.Q<Label>("CURRENT_HOVER");
            if (crt_hover != null)
                this.Remove(crt_hover);
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

            _plottedPositions.Clear();
            InitDynamicRange(points);

            for (int i = 0; i < points.GetLength(0); ++i)
            {
                var relative_position_x = MathHelpers.Lerp(points[i, 0], current_range_x.x, current_range_x.y);
                var relative_position_y = MathHelpers.Lerp(points[i, 1], current_range_y.x, current_range_y.y);

                painter2D.BeginPath();

                var plot_position = Plot(relative_position_x, relative_position_y);
                _plottedPositions.TryAdd(plot_position, new Vector2Double(points[i, 0], points[i, 1]));

                painter2D.Arc(plot_position, _dotRadius, 0, 360);

                // coloriser avec gradient

                var color_x = VisualizationSheet.visualizationSettings.warmGradient.Evaluate((float)relative_position_x);
                var color_y = VisualizationSheet.visualizationSettings.coldGradient.Evaluate((float)relative_position_y);

                float mean = (float)(relative_position_x + relative_position_y) / 2;

                painter2D.fillColor = Color.Lerp(color_x, color_y, mean);

                painter2D.Fill();
            }
        }

        protected void GenerateClassColoredScatter(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = Color.white;

            var points = _getClassPoints();

            if (points.Count == 0)
                return;

            _plottedPositions.Clear();
            InitDynamicRange(points);

            foreach (var kvp in points)
            {
                for (int i = 0; i < kvp.Value.GetLength(0); ++i)
                {
                    var relative_position_x = MathHelpers.Lerp(kvp.Value[i, 0], current_range_x.x, current_range_x.y);
                    var relative_position_y = MathHelpers.Lerp(kvp.Value[i, 1], current_range_y.x, current_range_y.y);

                    painter2D.BeginPath();

                    var plot_position = Plot(relative_position_x, relative_position_y);
                    _plottedPositions.TryAdd(plot_position, new Vector2Double(kvp.Value[i, 0], kvp.Value[i, 1]));

                    painter2D.Arc(plot_position, _dotRadius, 0, 360);

                    painter2D.fillColor = kvp.Key;

                    painter2D.Fill();
                }
            }
        }

        protected void GenerateValueDrivenGradientColoredScatter(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = Color.white;

            var points = _getValuePoints();

            if (points.Count == 0)
                return;

            _plottedPositions.Clear();

            InitDynamicRange(points);

            Vector2 min_max_value = new Vector2(float.MaxValue, float.MinValue);
            foreach (var kvp in points)
            {
                min_max_value.x = System.Math.Min((float)kvp.Value, min_max_value.x);
                min_max_value.y = System.Math.Max((float)kvp.Value, min_max_value.y);
            }

            foreach (var kvp in points)
            {
                for (int i = 0; i < kvp.Key.GetLength(0); ++i)
                {
                    var relative_position_x = MathHelpers.Lerp(kvp.Key[i, 0], current_range_x.x, current_range_x.y);
                    var relative_position_y = MathHelpers.Lerp(kvp.Key[i, 1], current_range_y.x, current_range_y.y);

                    painter2D.BeginPath();

                    var plot_position = Plot(relative_position_x, relative_position_y);
                    _plottedPositions.TryAdd(plot_position, new Vector2Double(kvp.Key[i, 0], kvp.Key[i, 1]));

                    painter2D.Arc(plot_position, _dotRadius, 0, 360);

                    var normalized_value = (float)MathHelpers.Lerp(kvp.Value, min_max_value.x, min_max_value.y);
                    var color = VisualizationSheet.visualizationSettings.valueGradient.Evaluate(normalized_value);

                    painter2D.fillColor = color;

                    painter2D.Fill();
                }
            }
        }

        private void InitDynamicRange(Dictionary<Color, double[,]> points)
        {
            /*if (current_range_x != Vector2Double.zero && current_range_y != Vector2Double.zero)
                return;*/

            current_range_x = fixed_range_x;
            current_range_y = fixed_range_y;

            Vector2Double min_max_x = new Vector2Double(double.MaxValue, double.MinValue);
            Vector2Double min_max_y = new Vector2Double(double.MaxValue, double.MinValue);

            foreach (var kvp in points)
            {
                if (fixed_range_x == Vector2Double.zero)
                {
                    MathHelpers.ColumnMinMax(kvp.Value, 0, out var range_x);
                    min_max_x.x = System.Math.Min(min_max_x.x, range_x.x);
                    min_max_x.y = System.Math.Max(min_max_x.y, range_x.y);
                }

                if (fixed_range_y == Vector2Double.zero)
                {
                    MathHelpers.ColumnMinMax(kvp.Value, 1, out var range_y);
                    min_max_y.x = System.Math.Min(min_max_y.x, range_y.x);
                    min_max_y.y = System.Math.Max(min_max_y.y, range_y.y);
                }
            }

            if (fixed_range_x == Vector2Double.zero)
                current_range_x = min_max_x;

            if (fixed_range_y == Vector2Double.zero)
                current_range_y = min_max_y;
        }

        private void InitDynamicRange(Dictionary<double[,], double> points)
        {
           /* if (current_range_x != Vector2Double.zero && current_range_y != Vector2Double.zero)
                return;*/

            current_range_x = fixed_range_x;
            current_range_y = fixed_range_y;

            Vector2Double min_max_x = new Vector2Double(double.MaxValue, double.MinValue);
            Vector2Double min_max_y = new Vector2Double(double.MaxValue, double.MinValue);

            foreach (var kvp in points)
            {
                if (fixed_range_x == Vector2Double.zero)
                {
                    MathHelpers.ColumnMinMax(kvp.Key, 0, out var range_x);
                    min_max_x.x = System.Math.Min(min_max_x.x, range_x.x);
                    min_max_x.y = System.Math.Max(min_max_x.y, range_x.y);
                }

                if (fixed_range_y == Vector2Double.zero)
                {
                    MathHelpers.ColumnMinMax(kvp.Key, 1, out var range_y);
                    min_max_y.x = System.Math.Min(min_max_y.x, range_y.x);
                    min_max_y.y = System.Math.Max(min_max_y.y, range_y.y);
                }
            }

            if (fixed_range_x == Vector2Double.zero)
                current_range_x = min_max_x;

            if (fixed_range_y == Vector2Double.zero)
                current_range_y = min_max_y;
        }

        private void InitDynamicRange(double[,] points)
        {
            /*if (current_range_x != Vector2Double.zero && current_range_y != Vector2Double.zero)
                return;*/

            current_range_x = fixed_range_x;
            current_range_y = fixed_range_y;

            if (fixed_range_x == Vector2Double.zero)
            {
                MathHelpers.ColumnMinMax(points, 0, out var range_x);
                current_range_x = range_x;
            }

            if(fixed_range_y == Vector2Double.zero)
            {
                MathHelpers.ColumnMinMax(points, 1, out var range_y);
                current_range_y = range_y;
            }            
        }
    }
}
