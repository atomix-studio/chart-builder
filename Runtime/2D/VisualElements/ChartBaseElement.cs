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
        protected Vector2Double _gridSize = new Vector2Double(1, 1);

        protected Color _strokeColor = Color.black;
        protected Color _gridColor = new Color(.8f, .8f, .8f, 1f);
        protected Color _backgroundColor = Color.white;

        public float lineWidth { get { return _lineWidth; } set { _lineWidth = value; } }

        public Color strokeColor { get { return _strokeColor; } set { _strokeColor = value; } }
        public Color gridColor { get { return _gridColor; } set { _gridColor = value; } }

        public Color backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; style.backgroundColor = new StyleColor(_backgroundColor); } }

        /// <summary>
        /// Valeur min-max en Y (toute valeur dépassant ce seuil sera hors du graphe)
        /// </summary>
        public Vector2Double fixed_range_y { get; set; }
        public Vector2Double fixed_range_x { get; set; }

        public abstract Vector2Double current_range_y { get; set; }
        public abstract Vector2Double current_range_x { get; set; }


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
        public Vector2Double gridSize
        {
            get
            {
                switch (gridSizeMode)
                {
                    case GridModes.FixedPointsCount:
                        return new Vector2Double((int)_gridSize.x, _gridSize.y);
                    case GridModes.FixedDeltaValue:
                        var delta_x = current_range_x.y - current_range_x.x;
                        int points_x = (int)(delta_x / _gridSize.x);

                        var delta_y = current_range_y.y - current_range_y.x;
                        int points_y = (int)(delta_y / _gridSize.x);

                        return new Vector2Double(points_x, points_y);
                }

                return new Vector2Double(3, 3);
            }
            set
            {
                _gridSize = value;
            }
        }

        public Vector2Double gridDelta
        {
            get
            {
                switch (gridSizeMode)
                {
                    case GridModes.FixedPointsCount:
                        var delta_x = current_range_x.y - current_range_x.x;
                        int points_x = (int)(delta_x / _gridSize.x);

                        var delta_y = current_range_y.y - current_range_y.x;
                        int points_y = (int)(delta_y / _gridSize.x);

                        return new Vector2Double(points_x, points_y);
                    case GridModes.FixedDeltaValue:
                        return new Vector2Double((int)_gridSize.x, _gridSize.y);
                }

                return Vector2Double.one;
            }
        }

        public GridModes gridSizeMode { get; set; } = GridModes.FixedPointsCount;

        public enum GridModes
        {
            FixedPointsCount,
            FixedDeltaValue
        }

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
            _gridLineWidth = graphLineWidth;

            generateVisualContent += DrawAutomaticGridCallback;

            this.RegisterCallbackOnce<GeometryChangedEvent>(e =>
            {
                int grid_points_x = (int)this.gridSize.x;
                int grid_points_y = (int) this.gridSize.y;

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
                var label = new Label((current_range_x.x + i * gridDelta.x).ToString());
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
                var label = new Label((current_range_y.x + (grid_points_y - i) * gridDelta.y).ToString());
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

        protected void DrawAxisCallback(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;
            painter2D.lineWidth = _gridLineWidth;
            painter2D.strokeColor = _gridColor;

            painter2D.BeginPath();

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

            var points_y = (int) gridSize.y;

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


    }
}
