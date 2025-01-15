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
        protected Vector2Double _gridDelta = new Vector2Double(1, 1);

        protected Color _strokeColor = Color.black;
        protected Color _gridColor = new Color(.8f, .8f, .8f, 1f);
        protected Color _backgroundColor = Color.white;

        public float lineWidth { get { return _lineWidth; } set { _lineWidth = value; } }

        /// <summary>
        /// Step between two grid lines on x and y axis
        /// </summary>
        public Vector2Double gridDelta { get { return _gridDelta; } set { _gridDelta = value; } }

        public Color strokeColor { get { return _strokeColor; } set { _strokeColor = value; } }
        public Color gridColor { get { return _gridColor; } set { _gridColor = value; } }

        public Color backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; style.backgroundColor = new StyleColor(_backgroundColor); } }

        /// <summary>
        /// Valeur min-max en Y (toute valeur dépassant ce seuil sera hors du graphe)
        /// </summary>
        public Vector2Double fixed_range_y { get; set; }
        public Vector2Double fixed_range_x { get; set; }

        public abstract Vector2Double dynamic_range_y { get; set; }
        public abstract Vector2Double dynamic_range_x { get; set; }


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

        public int grid_points_x
        {
            get
            {
                var delta_x = dynamic_range_x.y - dynamic_range_x.x;
                int points_x = (int)(delta_x / gridDelta.x);
                return points_x;
            }
        }

        public int grid_points_y
        {
            get
            {
                var delta_y = dynamic_range_y.y - dynamic_range_y.x;
                int points_y = (int)(delta_y / gridDelta.y);
                return points_y;
            }
        }
        public void DrawAutomaticGrid(int fontSize = 12, string x_axis_title = "x", string y_axis_title = "y", float graphLineWidth = 2f)
        {
            _gridLineWidth = graphLineWidth;

            generateVisualContent += DrawAutomaticGridCallback;

            this.RegisterCallbackOnce<GeometryChangedEvent>(e =>
            {
                Debug.Log(width);

                int points_x = grid_points_x;
                int points_y = grid_points_y;

                // texte absisse
                for (int j = 0; j <= grid_points_x; ++j)
                {
                    var x = (float)j / (float)points_x;
                    var text = (dynamic_range_x.x + j * gridDelta.x).ToString();
                    SetTextOnPosition(text,
                        new Vector2(x, 0),
                         -fontSize,
                         (int)(paddingBottom / 4));
                }

                // texte ordonées
                for (int j = 0; j <= grid_points_y; ++j)
                {
                    var x = (float)j / (float)points_y;
                    var text = (dynamic_range_y.x + j * gridDelta.y).ToString();
                    SetTextOnPosition(text,
                        new Vector2(0, x),
                        x_offset: -(int)(paddingLeft / 2), // x_offset
                        y_offset: -fontSize / 2,
                        fontSize: fontSize);
                }

                SetTextOnPosition(y_axis_title, new Vector2(0, .5f), -(int)(paddingLeft + 30), 0, fontSize + 2, -90);
                SetTextOnPosition(x_axis_title, new Vector2(0.5f, 0f), -(int)(x_axis_title.Length * 2 * fontSize / 10), (int)(paddingBottom - 20), fontSize + 2);

                this.MarkDirtyRepaint();
            });

            Refresh();
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

            var delta_x = dynamic_range_x.y - dynamic_range_x.x;
            var delta_y = dynamic_range_y.y - dynamic_range_y.x;

            int points_x = (int)(delta_x / gridDelta.x);
            int points_y = (int)(delta_y / gridDelta.y);

            for (int i = 0; i <= points_y; ++i)
            {
                var y = (float)i / (float)points_y;

                painter2D.MoveTo(Plot(0, y));
                painter2D.LineTo(Plot(1, y));
            }

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
