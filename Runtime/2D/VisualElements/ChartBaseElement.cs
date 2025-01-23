using Atomix.ChartBuilder.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder.VisualElements
{
    public abstract class ChartBaseElement : ChartBuilderElement
    {
        protected float _lineWidth = 2;
        protected float _gridLineWidth = 2;
        protected Vector2Int _gridSize = new Vector2Int(0, 0);

        protected Color _strokeColor = Color.black;
        protected Color _gridColor = new Color(.8f, .8f, .8f, 1f);
        protected Color _backgroundColor = Color.white;

        public float lineWidth { get { return _lineWidth; } set { _lineWidth = value; } }

        public Color strokeColor { get { return _strokeColor; } set { _strokeColor = value; } }
        public Color gridColor { get { return _gridColor; } set { _gridColor = value; } }

        public Color backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; style.backgroundColor = new StyleColor(_backgroundColor); } }

        public Action onRefresh { get; set; }

        private Vector2Double _current_range_x;
        private Vector2Double _current_range_y;


        public Vector2Double current_range_x
        {
            get
            {
                return _current_range_x;
            }
            set
            {
                _current_range_x = ComputeEvenRange(value.x, value.y, (int)gridSize.x, out var interval_x);
                gridDelta = new Vector2Double(interval_x, gridDelta.y);
            }
        }

        public Vector2Double current_range_y
        {
            get
            {
                return _current_range_y;
            }
            set
            {
                _current_range_y = ComputeEvenRange(value.x, value.y, (int)gridSize.y, out var interval_y);
                gridDelta = new Vector2Double(gridDelta.x, interval_y);
            }
        }

        public static List<double> ComputeReasonableTicks(double axisMin, double axisMax, int maxTicks = 15)
        {
            // Calculate default range
            double axisRange = axisMax - axisMin;

            // Start with a rough interval estimate
            double roughInterval = axisRange / maxTicks;

            // Snap interval to nearest 'nice' value (1, 0.5, 0.2, etc.)
            double baseValue = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(roughInterval)));
            double[] niceIntervals = { 0.5, 1, 2, 5, 10, 25, 100 }; // Common intervals
            double interval = baseValue;

            foreach (var nice in niceIntervals)
            {
                double candidate = baseValue * nice;
                if (candidate >= roughInterval)
                {
                    interval = candidate;
                    break;
                }
            }

            // Generate tick positions
            double min_value = System.Math.Floor(axisMin / interval) * interval;
            double max_value = System.Math.Ceiling(axisMax / interval) * interval;

            List<double> ticks = new List<double>();
            for (double tick = min_value; tick <= max_value; tick += interval)
            {
                ticks.Add(System.Math.Round(tick, 5)); // Round to avoid floating-point precision issues
            }

            return ticks;
        }

        public Vector2Double ComputeEvenRange(double axisMin, double axisMax, int maxGridSize, out double interval)
        {
            // Calculate default range
            double axisRange = axisMax - axisMin;

            // Start with a rough interval estimate
            double roughInterval = axisRange / maxGridSize;

            // Snap interval to nearest 'nice' value (1, 0.5, 0.2, etc.)
            double baseValue = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(roughInterval)));
            double[] niceIntervals = { 0.5, 1, 2, 5, 10, 25, 100 }; // Common intervals
            interval = baseValue;

            foreach (var nice in niceIntervals)
            {
                double candidate = baseValue * nice;
                if (candidate >= roughInterval)
                {
                    interval = candidate;
                    break;
                }
            }

            // Generate tick positions
            double min_value = System.Math.Floor(axisMin / interval) * interval;
            double max_value = System.Math.Ceiling(axisMax / interval) * interval;

            return new Vector2Double(min_value, max_value);
        }

        #region Utils

        #endregion

        public override void Refresh()
        {
            base.Refresh();

            onRefresh?.Invoke();
        }

        #region Drawing

        public ChartBaseElement SetLineWidth(float lineWidth)
        {
            _lineWidth = lineWidth;
            return this;
        }

        public Vector2 Plot(double x_normalized, double y_normalized)
        {
            var x = (float)(paddingLeft + x_normalized * real_width);
            var y = (float)(paddingBottom + (1 - y_normalized) * real_heigth);

            return new Vector2(x, y);
        }

        #endregion

        #region Text

        public Label SetTextOnPosition(string content, Vector2 normalizedPosition, int x_offset = 0, int y_offset = 0, float fontSize = 12, int rotation = 0)
        {
            var label = new Label(content);

            label.style.position = Position.Absolute;
            label.style.fontSize = fontSize;
            //label.style.unityFont = font;

            var positions = Plot(normalizedPosition.x, normalizedPosition.y);

            label.style.left = positions.x + x_offset;
            label.style.top = positions.y + y_offset;
            label.style.alignSelf = Align.Center;  // Centers the label inside its container (vertically and horizontally)
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperCenter);

            if (rotation != 0)
                label.transform.rotation *= Quaternion.Euler(0, 0, rotation);

            label.AddToClassList("normal-text");

            this.Add(label);

            return label;
        }


        #endregion

        #region Orthonormal lines & Grid

        /// <summary>
        /// Step between two grid lines on x and y axis
        /// </summary>
        public Vector2Int gridSize
        {
            get
            {
                int x = (int)_gridSize.x;
                int y = (int)_gridSize.y;

                if (_gridSize.x == 0)
                    x = 10;

                if (_gridSize.y == 0)
                    y = 10;

                return new Vector2Int(x, y);
            }
            set
            {
                _gridSize = value;
            }
        }

        public Vector2Double gridDelta { get; protected set; }

        /// <summary>
        /// Affiche les lignes X, Y ancrées en bas à gauche du graphe
        /// </summary>
        public void DrawAxis(float graphLineWidth = 2f)
        {
            _gridLineWidth = graphLineWidth;

            // Implement graduation drawing logic here
            generateVisualContent += DrawAxisCallback;
            Refresh();
        }

        public void DrawAutomaticAxis(float graphLineWidth = 2f)
        {
            _gridLineWidth = graphLineWidth;

            // TODO
            Refresh();
        }

        public void DrawAutomaticGrid(int fontSize = 12, string x_axis_title = "x", string y_axis_title = "y", float graphLineWidth = 2f)
        {
            Refresh();

            _gridLineWidth = graphLineWidth;

            generateVisualContent += DrawAutomaticGridCallback;

            this.RegisterCallbackOnce<GeometryChangedEvent>(e =>
            {
                int grid_points_x = (int)this.gridSize.x;
                int grid_points_y = (int)this.gridSize.y;

                AddAxisXGraduationsText(fontSize, grid_points_x);

                AddAxisYGraduationsText(fontSize, grid_points_y);

                AddAxisXTitleText(x_axis_title, fontSize);

                SetTextOnPosition(y_axis_title, new Vector2(0, .5f), -(int)(paddingLeft + 30), 0, fontSize + 2, -90);
                SetTextOnPosition(x_axis_title, new Vector2(0.5f, 0f), -(int)(x_axis_title.Length * 2 * fontSize / 10), (int)(paddingBottom - 20), fontSize + 2);

                this.MarkDirtyRepaint();
            });

            Refresh();
        }

        private void AddAxisXTitleText(string title, int fontSize)
        {
            var axis_container = new VisualElement();
            axis_container.style.width = new Length((int)real_width, LengthUnit.Pixel);
            axis_container.style.height = 25;
            axis_container.style.backgroundColor = Color.red;// new Color(0, 0, 0, 0);//Color.red;
            axis_container.style.position = Position.Absolute;
            axis_container.style.justifyContent = Justify.Center;
            axis_container.style.alignContent = Align.Stretch;
            axis_container.style.flexDirection = FlexDirection.Row;
            axis_container.style.bottom = 0;

            this.Add(axis_container);
        }

        private void AddAxisXGraduationsText(int fontSize, int grid_points_x)
        {
            var x_label_width = (int)(real_width / grid_points_x);

            var axis_container_height = 25;
            var axis_container = new VisualElement();
            axis_container.style.width = new Length((int)real_width + x_label_width, LengthUnit.Pixel);
            axis_container.style.backgroundColor = new Color(0, 0, 0, 0);//Color.red;
            axis_container.style.position = Position.Absolute;
            axis_container.style.bottom = (int)paddingBottom - axis_container_height;
            axis_container.style.translate = new StyleTranslate(new Translate(new Length(-x_label_width / 2, LengthUnit.Pixel), new Length(), 0));// ;
            axis_container.style.justifyContent = Justify.SpaceEvenly;
            axis_container.style.alignContent = Align.Stretch;
            axis_container.style.flexDirection = FlexDirection.Row;
            axis_container.style.height = axis_container_height;

            for (int i = 0; i <= (int)this.gridSize.x; ++i)
            {
                var label = new Label(System.Math.Round((current_range_x.x + i * gridDelta.x), 2).ToString());
                label.style.position = Position.Relative;
                label.style.width = new Length(x_label_width, LengthUnit.Pixel);
                label.style.fontSize = fontSize;
                label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                label.style.alignSelf = Align.Center;  // Centers the label inside its container (vertically and horizontally)
                                                       //label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperCenter);
                label.AddToClassList("normal-text");

                axis_container.Add(label);
            }

            this.Add(axis_container);
        }

        private void AddAxisYGraduationsText(int fontSize, int grid_points_y)
        {
            var y_label_width = (int)(real_heigth / grid_points_y);

            var axis_container_height = 35;
            var axis_container = new VisualElement();
            axis_container.style.height = new Length((int)real_heigth + y_label_width, LengthUnit.Pixel);
            axis_container.style.backgroundColor = new Color(0, 0, 0, 0);// Color.green;
            axis_container.style.position = Position.Absolute;
            axis_container.style.left = (int)paddingLeft - axis_container_height;
            axis_container.style.translate = new StyleTranslate(new Translate(new Length(), new Length(-y_label_width / 2, LengthUnit.Pixel), 0));// ;
            axis_container.style.justifyContent = Justify.SpaceEvenly;
            axis_container.style.alignContent = Align.Stretch;
            axis_container.style.flexDirection = FlexDirection.Column;
            axis_container.style.width = axis_container_height;

            for (int i = 0; i <= grid_points_y; ++i)
            {
                var label = new Label(System.Math.Round((current_range_y.x + (grid_points_y - i) * gridDelta.y), 2).ToString());
                label.style.position = Position.Relative;
                label.style.height = new Length(y_label_width, LengthUnit.Pixel);
                label.style.fontSize = fontSize;
                label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                label.style.alignSelf = Align.Center;  // Centers the label inside its container (vertically and horizontally)
                                                       //label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperCenter);
                label.AddToClassList("normal-text");

                axis_container.Add(label);
            }

            this.Add(axis_container);
        }

        protected virtual void DrawAxisCallback(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;
            painter2D.lineWidth = _gridLineWidth;
            painter2D.strokeColor = _gridColor;

            painter2D.BeginPath();

            Debug.LogError("Todo calculate position of zero, zero");

            var zero_coord_x = current_range_x.y - current_range_x.x;

            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(1, 0));

            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(0, 1));

            painter2D.Stroke();
        }

        protected void DrawAutomaticGridCallback(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;
            painter2D.lineWidth = _gridLineWidth;
            painter2D.strokeColor = _gridColor;

            painter2D.BeginPath();

            var points_y = (int)gridSize.y;

            for (int i = 0; i <= points_y; ++i)
            {
                var y = (float)i / (float)points_y;

                painter2D.MoveTo(Plot(0, y));
                painter2D.LineTo(Plot(1, y));
            }

            var points_x = (int)gridSize.x;

            for (int j = 0; j <= points_x; ++j)
            {
                var x = (float)j / (float)points_x;

                painter2D.MoveTo(Plot(x, 0));
                painter2D.LineTo(Plot(x, 1));
            }

            painter2D.Stroke();
        }

        #endregion

        #region Drawing

        protected Dictionary<Vector2, Vector2Double> _plottedPositions = new Dictionary<Vector2, Vector2Double>();


        #region AddDrawing

        public void AppendLine(double[,] points, Color lineColor, float lineWidth)
        {
            generateVisualContent += (meshGenerationContext) =>
            {
                _lineWidth = lineWidth;
                _strokeColor = lineColor;

                GenerateLineXY(meshGenerationContext, points, false);
            };
        } 
        
        public void AppendScatter(double[,] points, Color scatterColor, float lineWidth)
        {
            generateVisualContent += (meshGenerationContext) =>
            {
                _lineWidth = lineWidth;
                _strokeColor = scatterColor;

                GenerateScatter(meshGenerationContext, points, false);
            };
        }

        #endregion

        #region Line


        /// <summary>
        /// Generate the line without knowing any x value, so we assume a equal distribution of points on x and just compute the interval by pointsCount / avalaibleWidth 
        /// </summary>
        /// <param name="ctx"></param>
        protected void GenerateFunctionLine(MeshGenerationContext ctx, Func<double, double> function, Vector2Double x_interval)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            // compute y range

            // calculate interval of values from a given number of points

            /*painter2D.BeginPath();

            var relative_position_x = 0.0;
            var relative_position_y = MathHelpers.Lerp(_pointsY[0], current_range_y.x, current_range_y.y);

            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < _pointsY.Length; i++)
            {
                relative_position_x = MathHelpers.Lerp(i, current_range_x.x, current_range_x.y);
                relative_position_y = MathHelpers.Lerp(_pointsY[i], current_range_y.x, current_range_y.y);

                painter2D.LineTo(Plot(relative_position_x, relative_position_y));

            }*/

            painter2D.Stroke();
        }

        /// <summary>
        /// Generate the line without knowing any x value, so we assume a equal distribution of points on x and just compute the interval by pointsCount / avalaibleWidth 
        /// </summary>
        /// <param name="ctx"></param>
        protected void GenerateLineY(MeshGenerationContext ctx, double[] points, bool initializeRange)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            if (initializeRange)
                InitRange_pointsY(points);

            painter2D.BeginPath();

            var relative_position_x = 0.0;
            var relative_position_y = MathHelpers.Lerp(points[0], current_range_y.x, current_range_y.y);

            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < points.Length; i++)
            {
                relative_position_x = MathHelpers.Lerp(i, current_range_x.x, current_range_x.y);
                relative_position_y = MathHelpers.Lerp(points[i], current_range_y.x, current_range_y.y);

                painter2D.LineTo(Plot(relative_position_x, relative_position_y));

            }

            painter2D.Stroke();
        }

        protected void GenerateLineXY(MeshGenerationContext ctx, double[,] points, bool initializeRange)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            if (initializeRange)
                InitRange_pointsXY(points);



            var relative_position_x = MathHelpers.Lerp(points[0, 0], current_range_x.x, current_range_x.y);
            var relative_position_y = MathHelpers.Lerp(points[0, 1], current_range_y.x, current_range_y.y);

            painter2D.BeginPath();
            painter2D.MoveTo(Plot(relative_position_x, relative_position_y));

            for (int i = 0; i < points.GetLength(0); i++)
            {
                relative_position_x = MathHelpers.Lerp(points[i, 0], current_range_x.x, current_range_x.y);
                relative_position_y = MathHelpers.Lerp(points[i, 1], current_range_y.x, current_range_y.y);

                painter2D.LineTo(Plot(relative_position_x, relative_position_y));
            }

            painter2D.Stroke();
        }

        #endregion

        #region Scatter

        protected void GenerateGradientColoredScatter(MeshGenerationContext ctx, double[,] pointsXY, bool initializeRange)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            if (pointsXY.Length == 0)
                return;

            if (pointsXY.GetLength(1) != 2)
                throw new Exception($"Scatter2D requires only 2 column matrix");

            _plottedPositions.Clear();

            if (initializeRange)
                InitRange_pointsXY(pointsXY);

            for (int i = 0; i < pointsXY.GetLength(0); ++i)
            {
                var relative_position_x = MathHelpers.Lerp(pointsXY[i, 0], current_range_x.x, current_range_x.y);
                var relative_position_y = MathHelpers.Lerp(pointsXY[i, 1], current_range_y.x, current_range_y.y);

                painter2D.BeginPath();

                var plot_position = Plot(relative_position_x, relative_position_y);
                _plottedPositions.TryAdd(plot_position, new Vector2Double(pointsXY[i, 0], pointsXY[i, 1]));

                painter2D.Arc(plot_position, _lineWidth, 0, 360);

                // coloriser avec gradient

                var color_x = VisualizationSheet.visualizationSettings.warmGradient.Evaluate((float)relative_position_x);
                var color_y = VisualizationSheet.visualizationSettings.coldGradient.Evaluate((float)relative_position_y);

                float mean = (float)(relative_position_x + relative_position_y) / 2;

                painter2D.fillColor = Color.Lerp(color_x, color_y, mean);

                painter2D.Fill();
            }

            painter2D.Stroke();
        }

        protected void GenerateScatter(MeshGenerationContext ctx, double[,] pointsXY, bool initializeRange)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;
            painter2D.fillColor = strokeColor;

            if (pointsXY.Length == 0)
                return;

            if (pointsXY.GetLength(1) != 2)
                throw new Exception($"Scatter2D requires only 2 column matrix");

            _plottedPositions.Clear();

            if (initializeRange)
                InitRange_pointsXY(pointsXY);

            for (int i = 0; i < pointsXY.GetLength(0); ++i)
            {
                var relative_position_x = MathHelpers.Lerp(pointsXY[i, 0], current_range_x.x, current_range_x.y);
                var relative_position_y = MathHelpers.Lerp(pointsXY[i, 1], current_range_y.x, current_range_y.y);

                painter2D.BeginPath();

                var plot_position = Plot(relative_position_x, relative_position_y);
                _plottedPositions.TryAdd(plot_position, new Vector2Double(pointsXY[i, 0], pointsXY[i, 1]));

                //painter2D.MoveTo(plot_position);
                painter2D.Arc(plot_position, _lineWidth, 0, 360);

                painter2D.Fill();
                painter2D.ClosePath();

            }
            painter2D.Stroke();
            painter2D.ClosePath();
        }

        protected void GenerateClassColoredScatter(MeshGenerationContext ctx, Dictionary<Color, double[,]> colorClassedPoints, bool initializeRange)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            if (colorClassedPoints.Count == 0)
                return;

            _plottedPositions.Clear();

            if (initializeRange)
                InitRange_colorClassedPoints(colorClassedPoints);

            foreach (var kvp in colorClassedPoints)
            {
                for (int i = 0; i < kvp.Value.GetLength(0); ++i)
                {
                    var relative_position_x = MathHelpers.Lerp(kvp.Value[i, 0], current_range_x.x, current_range_x.y);
                    var relative_position_y = MathHelpers.Lerp(kvp.Value[i, 1], current_range_y.x, current_range_y.y);

                    painter2D.BeginPath();

                    var plot_position = Plot(relative_position_x, relative_position_y);
                    _plottedPositions.TryAdd(plot_position, new Vector2Double(kvp.Value[i, 0], kvp.Value[i, 1]));

                    painter2D.Arc(plot_position, _lineWidth, 0, 360);

                    painter2D.fillColor = kvp.Key;

                    painter2D.Fill();
                }
            }

            painter2D.Stroke();
        }

        protected void GenerateValueDrivenGradientColoredScatter(MeshGenerationContext ctx, Dictionary<double[,], double> colorClassedPoints, bool initializeRange)
        {
            var painter2D = ctx.painter2D;

            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = strokeColor;

            if (colorClassedPoints.Count == 0)
                return;

            _plottedPositions.Clear();

            if (initializeRange)
                InitRange_pointsXYValues(colorClassedPoints);

            Vector2 min_max_value = new Vector2(float.MaxValue, float.MinValue);
            foreach (var kvp in colorClassedPoints)
            {
                min_max_value.x = System.Math.Min((float)kvp.Value, min_max_value.x);
                min_max_value.y = System.Math.Max((float)kvp.Value, min_max_value.y);
            }

            foreach (var kvp in colorClassedPoints)
            {
                for (int i = 0; i < kvp.Key.GetLength(0); ++i)
                {
                    var relative_position_x = MathHelpers.Lerp(kvp.Key[i, 0], current_range_x.x, current_range_x.y);
                    var relative_position_y = MathHelpers.Lerp(kvp.Key[i, 1], current_range_y.x, current_range_y.y);

                    painter2D.BeginPath();

                    var plot_position = Plot(relative_position_x, relative_position_y);
                    _plottedPositions.TryAdd(plot_position, new Vector2Double(kvp.Key[i, 0], kvp.Key[i, 1]));

                    painter2D.Arc(plot_position, _lineWidth, 0, 360);

                    var normalized_value = (float)MathHelpers.Lerp(kvp.Value, min_max_value.x, min_max_value.y);
                    var color = VisualizationSheet.visualizationSettings.valueGradient.Evaluate(normalized_value);

                    painter2D.fillColor = color;

                    painter2D.Fill();
                }
            }

            painter2D.Stroke();
        }

        #endregion

        #region Range Init


        protected void InitRange_colorClassedPoints(Dictionary<Color, double[,]> points)
        {
            Vector2Double min_max_x = new Vector2Double(double.MaxValue, double.MinValue);
            Vector2Double min_max_y = new Vector2Double(double.MaxValue, double.MinValue);

            foreach (var kvp in points)
            {
                MathHelpers.ColumnMinMax(kvp.Value, 0, out var range_x);
                min_max_x.x = System.Math.Min(min_max_x.x, range_x.x);
                min_max_x.y = System.Math.Max(min_max_x.y, range_x.y);

                MathHelpers.ColumnMinMax(kvp.Value, 1, out var range_y);
                min_max_y.x = System.Math.Min(min_max_y.x, range_y.x);
                min_max_y.y = System.Math.Max(min_max_y.y, range_y.y);
            }

            current_range_x = min_max_x;

            current_range_y = min_max_y;
        }

        protected void InitRange_pointsXYValues(Dictionary<double[,], double> points)
        {
            Vector2Double min_max_x = new Vector2Double(double.MaxValue, double.MinValue);
            Vector2Double min_max_y = new Vector2Double(double.MaxValue, double.MinValue);

            foreach (var kvp in points)
            {
                MathHelpers.ColumnMinMax(kvp.Key, 0, out var range_x);
                min_max_x.x = System.Math.Min(min_max_x.x, range_x.x);
                min_max_x.y = System.Math.Max(min_max_x.y, range_x.y);

                MathHelpers.ColumnMinMax(kvp.Key, 1, out var range_y);
                min_max_y.x = System.Math.Min(min_max_y.x, range_y.x);
                min_max_y.y = System.Math.Max(min_max_y.y, range_y.y);
            }

            current_range_x = min_max_x;

            current_range_y = min_max_y;
        }

        protected void InitRange_pointsXY(double[,] points)
        {
            MathHelpers.ColumnMinMax(points, 0, out var range_x);
            current_range_x = range_x;

            MathHelpers.ColumnMinMax(points, 1, out var range_y);
            current_range_y = range_y;
        }

        protected void InitRange_pointsY(double[] points)
        {
            current_range_x = new Vector2Double(0, points.Length);

            MathHelpers.ColumnMinMax(points, out var range_y);
            current_range_y = range_y;
        }

        #endregion

        #endregion

    }
}
