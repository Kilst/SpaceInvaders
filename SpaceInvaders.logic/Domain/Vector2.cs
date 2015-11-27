using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.logic.Domain
{
    public class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2 AddVector2(Vector2 add)
        {
            this.X += add.X;
            this.Y += add.Y;
            return this;
        }

        public Tuple<bool,bool> GreaterThan(Vector2 compare)
        {
            bool XCheck = false;
            bool YCheck = false;

            // Test the calling vector2 against vector2 compare
            if (this.X > compare.X)
                XCheck = true;
            else
                XCheck = false;
            if (this.Y > compare.Y)
                YCheck = true;
            else
                YCheck = false;
            return Tuple.Create(XCheck, YCheck);
        }

        public override string ToString()
        {
            return "X: " + X + " Y: " + Y;
        }
    }
}
