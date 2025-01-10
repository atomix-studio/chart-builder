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
        protected Font _gridFont;

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

        public void PlotNumber(int number, Painter2D painter2D, Vector2Double normalizedPosition, Vector2 offset, float size = 5)
        {
            var chars = number.ToString();
            var realPosition = Plot(normalizedPosition.x, normalizedPosition.y) - offset;

            for (int i = 0; i < chars.Length; ++i)
            {
                if (number < 0 && i == 0)
                {
                    var pos = realPosition + Vector2.up * size / 2 - Vector2.right * size / 2;
                    painter2D.MoveTo(pos);
                    pos += Vector2.right * size / 3;
                    painter2D.LineTo(pos);
                    realPosition += i * Vector2.right * .4f;

                    continue;
                }

                PlotDigit(int.Parse(chars[i].ToString()), painter2D, ref realPosition, size);
            }

        }

        public void PlotDigit(int digit, Painter2D painter2D, ref Vector2 realPosition, float size = 10)
        {

            // Based on the digit, draw it using painter2D
            switch (digit)
            {
                case 0:
                    var pos = realPosition;
                    painter2D.MoveTo(pos);
                    pos += Vector2.up * size;
                    painter2D.LineTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos -= Vector2.up * size;
                    painter2D.LineTo(pos);
                    pos -= Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;

                    break;
                case 1:
                    painter2D.MoveTo(realPosition);
                    painter2D.LineTo(realPosition + Vector2.up * size);
                    realPosition += Vector2.right * size * .3f;

                    break;
                case 2:
                    pos = realPosition;
                    painter2D.MoveTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.left * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;

                    break;
                case 3:
                    pos = realPosition;
                    painter2D.MoveTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.left * size / 2;
                    painter2D.LineTo(pos);
                    pos -= Vector2.left * size / 2;
                    painter2D.MoveTo(pos);
                    pos += Vector2.up * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.left * size / 2;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;
                    break;
                case 4:
                    pos = realPosition;
                    painter2D.MoveTo(pos + Vector2.right * size / 4f);
                    pos += Vector2.up * size * .8f;
                    painter2D.LineTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.down * size / 3;
                    painter2D.MoveTo(pos);
                    pos += Vector2.up * size / 1.5f;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;
                    break;
                case 5:
                    pos = realPosition + Vector2.right * size / 2f;
                    painter2D.MoveTo(pos);
                    pos -= Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size / 2;
                    painter2D.LineTo(pos);
                    pos -= Vector2.left * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size / 2;
                    painter2D.LineTo(pos);
                    pos -= Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;
                    break;
                case 6:
                    pos = realPosition + Vector2.right * size / 2f;
                    painter2D.MoveTo(pos);
                    pos -= Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size;
                    painter2D.LineTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos -= Vector2.up * size / 2;
                    painter2D.LineTo(pos);
                    pos -= Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;
                    break;
                case 7:
                    pos = realPosition ;
                    painter2D.MoveTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size - Vector2.right * size / 4;
                    painter2D.LineTo(pos);
                    pos = realPosition + Vector2.right * size / 2 + Vector2.up * size / 2;
                    painter2D.MoveTo(pos);
                    pos += Vector2.left * size / 2;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;
                    break;
                case 8:
                    pos = realPosition;
                    painter2D.MoveTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size;
                    painter2D.LineTo(pos);
                    pos += Vector2.left * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.down * size;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size / 2;
                    painter2D.MoveTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    realPosition += Vector2.right * size * .7f;
                    break;
                case 9:
                    pos = realPosition + new Vector2(size /2, size / 2);
                    painter2D.MoveTo(pos);
                    pos += Vector2.left * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.down * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.right * size / 2;
                    painter2D.LineTo(pos);
                    pos += Vector2.up * size;
                    painter2D.LineTo(pos);
                    pos += Vector2.left * size / 2;
                    painter2D.LineTo(pos);

                    realPosition += Vector2.right * size * .7f;
                    break;
                default:
                    Debug.LogWarning("Unsupported digit");
                    break;
            }
        }

        public void SetTextOnPosition(string content, Vector2 normalizedPosition, float size = 8)
        {
            var label = new Label(content);
            label.style.position = Position.Absolute;
            label.style.fontSize = size;
            label.style.unityFont = _gridFont;
            //label.style.width = parent.resolvedStyle.width;
            var positions = Plot(normalizedPosition.x, normalizedPosition.y);
            label.style.top = positions.x;
            label.style.top = positions.y;
            //label.style.left = 0; // Center horizontally
            label.style.alignSelf = Align.Center;  // Centers the label inside its container (vertically and horizontally)
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperCenter);
            this.Add(label);
        }

        #endregion

        #region Orthonormal lines & Grid
        /// <summary>
        /// Affiche les lignes X, Y ancrées en bas à gauche du graphe
        /// </summary>
        public void DrawBottomLeftGraduation(float graphLineWidth = 2f)
        {
            _gridLineWidth = graphLineWidth;

            // Implement graduation drawing logic here
            generateVisualContent += DrawOrthonormalLines_BottomLeftAnchored;
            Refresh();
        }

        public void DrawAutomaticCenteredGraduation(float graphLineWidth = 2f)
        {
            _gridLineWidth = graphLineWidth;

            // TODO
            Refresh();
        }

        public void DrawAutomaticGrid(float graphLineWidth = 2f)
        {
            _gridLineWidth = graphLineWidth;
            generateVisualContent += DrawAutomaticGrid;
            Refresh();
        }

/*        public void DrawGridTexts(Font gridFont)
        {
            _gridFont = gridFont;

            var delta_x = dynamic_range_x.y - dynamic_range_x.x;
            var delta_y = dynamic_range_y.y - dynamic_range_y.x;

            int points_x = (int)(delta_x / gridDelta.x);
            int points_y = (int)(delta_y / gridDelta.y);

            for (int i = 0; i <= points_y; ++i)
            {
                var y = (float)i / (float)points_y;
            }

            for (int j = 0; j <= points_x; ++j)
            {
                var x = (float)j / (float)points_x;

                SetTextOnPosition((dynamic_range_x.x + j * gridDelta.x).ToString(), new Vector2(x, 0));
            }
        }
*/
        protected void DrawOrthonormalLines_BottomLeftAnchored(MeshGenerationContext ctx)
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

        protected void DrawAutomaticGrid(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;
            painter2D.lineWidth = _gridLineWidth;
            painter2D.strokeColor = _gridColor;

            painter2D.BeginPath();
            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(1, 0));

            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(0, 1));

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

            painter2D.lineJoin = LineJoin.Round;
            painter2D.strokeColor = Color.black;
            painter2D.lineWidth = 1.75f;

            for (int j = 0; j <= points_x; ++j)
            {
                var x = (float)j / (float)points_x;

                PlotNumber((int)(dynamic_range_x.x + j * gridDelta.x), painter2D, new Vector2Double(x, 0), Vector2.down * 8, 10);
            }


            painter2D.Stroke();
        }

        #endregion

    }
}
