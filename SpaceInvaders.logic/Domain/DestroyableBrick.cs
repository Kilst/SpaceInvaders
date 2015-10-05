using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class DestroyableBrick : Platform
    {
        int eighthWidth = 0;
        int eighthHeight = 0;

        // Call Base class constructor
        public DestroyableBrick(int width, int height, float mass, Vector2 position, Bitmap bitmap)
            : base(width, height, mass, position, bitmap)
        {
            eighthWidth = width / 8;
            eighthHeight = height / 8;
            GetBounds();
        }
    }
}
