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
        protected float _graphLineWidth = 2;

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

        #endregion

        #region Orthonormal lines & Grid
        /// <summary>
        /// Affiche les lignes X, Y ancrées en bas à gauche du graphe
        /// </summary>
        public void DrawBottomLeftGraduation(float graphLineWidth = 2f)
        {
            _graphLineWidth = graphLineWidth;

            // Implement graduation drawing logic here
            generateVisualContent += DrawOrthonormalLines_BottomLeftAnchored;
            Refresh();
        }

        public void DrawAutomaticCenteredGraduation(float graphLineWidth = 2f)
        {
            _graphLineWidth = graphLineWidth;

            // TODO
            Refresh();
        }

        public void DrawAutomaticGrid(float graphLineWidth = 2f)
        {
            _graphLineWidth = graphLineWidth;
            generateVisualContent += DrawAutomaticGrid;
            Refresh();
        }


        protected void DrawOrthonormalLines_BottomLeftAnchored(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;
            painter2D.lineWidth = _graphLineWidth;
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
            painter2D.lineWidth = _graphLineWidth;
            painter2D.strokeColor = _gridColor;

            painter2D.BeginPath();
            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(1, 0));

            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(0, 1));

            var delta_x = dynamic_range_x.y - dynamic_range_x.x;
            var delta_y = dynamic_range_y.y - dynamic_range_y.x;

            int points_x = (int)( delta_x / gridDelta.x);
            int points_y = (int)( delta_y / gridDelta.y);

            for(int i = 0; i <= points_y; ++i)
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
