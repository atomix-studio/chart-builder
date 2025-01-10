using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomix.ChartBuilder
{
    public struct Vector2Double
    {
        public Vector2Double(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x { get; set; }
        public double y { get; set; }

        public static Vector2Double zero
        {
            get
            {
                return new Vector2Double();
            }
        }


        // Overload for '=='
        public static bool operator ==(Vector2Double lhs, Vector2Double rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        // Overload for '!='
        public static bool operator !=(Vector2Double lhs, Vector2Double rhs)
        {
            return !(lhs == rhs);
        }

        // Overload for '+'
        public static Vector2Double operator +(Vector2Double lhs, Vector2Double rhs)
        {
            return new Vector2Double(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        // Overload for '-'
        public static Vector2Double operator -(Vector2Double lhs, Vector2Double rhs)
        {
            return new Vector2Double(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        // Overload for '*'
        public static Vector2Double operator *(Vector2Double lhs, double scalar)
        {
            return new Vector2Double(lhs.x * scalar, lhs.y * scalar);
        }

        public static Vector2Double operator *(double scalar, Vector2Double rhs)
        {
            return new Vector2Double(rhs.x * scalar, rhs.y * scalar);
        }

        // Overload for '/'
        public static Vector2Double operator /(Vector2Double lhs, double scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }
            return new Vector2Double(lhs.x / scalar, lhs.y / scalar);
        }

        // Override Equals for '=='
        public override bool Equals(object obj)
        {
            if (!(obj is Vector2Double))
            {
                return false;
            }
            Vector2Double other = (Vector2Double)obj;
            return this == other;
        }

        // Override GetHashCode
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
    }
}
