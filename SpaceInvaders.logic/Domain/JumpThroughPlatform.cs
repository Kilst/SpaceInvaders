using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class JumpThroughPlatform : Platform
    {
        public int ImageHeight = 0;

        // Call Base class constructor
        public JumpThroughPlatform(int width, int height, int imageHeight, float mass, Vector2 position, Bitmap bitmap)
            : base(width, height, mass, position, bitmap)
        {
            ImageHeight = imageHeight;
        }
    }
}
