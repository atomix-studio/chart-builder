﻿using Atomix.ChartBuilder.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder.VisualElements
{
    public class ChartBuilderElement : VisualElement
    {
        #region Padding & Margin

        private double _paddingRight = int.MinValue;
        private double _paddingBottom = int.MinValue;
        private double _paddingLeft = int.MinValue;
        private double _paddingTop = int.MinValue;

        protected double paddingRight
        {
            get
            {
                if (_paddingRight == int.MinValue)
                {
                    _paddingRight = style.paddingRight.value.value;
                }

                return _paddingRight;
            }
        }

        protected double paddingLeft
        {
            get
            {
                if (_paddingLeft == int.MinValue)
                {
                    _paddingLeft = style.paddingLeft.value.value;
                }

                return _paddingLeft;
            }
        }

        protected double paddingTop
        {
            get
            {
                if (_paddingTop == int.MinValue)
                {
                    _paddingTop = style.paddingTop.value.value;
                }

                return _paddingTop;
            }
        }

        protected double paddingBottom
        {
            get
            {
                if (_paddingBottom == int.MinValue)
                {
                    _paddingBottom = style.paddingBottom.value.value;
                }

                return _paddingBottom;
            }
        }

        protected double resolved_width => resolvedStyle.width;
        protected double resolved_height => resolvedStyle.height;
        protected double real_width => resolved_width - paddingLeft - paddingRight;
        protected double real_heigth => resolved_height - paddingTop - paddingBottom;


        public void SetMargin(int l, int r, int b, int t)
        {
            style.marginLeft = l;
            style.marginTop = t;
            style.marginRight = r;
            style.marginBottom = b;
        }

        public void SetPadding(int l, int r, int b, int t)
        {
            style.paddingLeft = l;
            style.paddingTop = t;
            style.paddingRight = r;
            style.paddingBottom = b;
        }

        #endregion

        #region Font & Text

        public virtual void SetTitle(string title, float fontSize = -1, float topOffset = 10)
        {
            var label = new Label(title);

            if (fontSize > 0)
                label.style.fontSize = fontSize;

            label.style.position = Position.Absolute;
            label.style.top = topOffset;
            label.style.alignSelf = Align.Center;  // Centers the label inside its container (vertically and horizontally)
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperCenter);

            label.AddToClassList("title-text");

            this.Add(label);

            MarkDirtyRepaint();
        }

        #endregion

        #region  Position & Dimension

        public ChartBuilderElement SetDimensions(LengthUnit lengthUnit, int width, int height)
        {
            style.width = new Length(width, lengthUnit);  // 50% of parent width
            style.height = new Length(height, lengthUnit);

            return this;
        }

        public ChartBuilderElement SetAbsolutePosition()
        {
            style.position = Position.Absolute;
            return this;
        }

        public ChartBuilderElement SetRelativePosition()
        {
            style.position = Position.Relative;
            return this;
        }

        #endregion


        public virtual void Refresh()
        {
            MarkDirtyRepaint();
        }
    }
}
