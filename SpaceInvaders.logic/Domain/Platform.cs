using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class Platform : GameObject
    {
        // Call Base class constructor
        public Platform(int width, int height, float mass, Vector2 position, Bitmap bitmap)
            : base(width, height, mass, position, bitmap)
        {

        }
    }
}
