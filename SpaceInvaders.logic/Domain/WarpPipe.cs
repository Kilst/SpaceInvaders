using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class WarpPipe : Pipe
    {
        public Vector2 WarpLoc { get; set; }
        public string WarpZoneName { get; set; }

        public WarpPipe(int width, int height, float mass, Vector2 position, Bitmap bitmap, Vector2 loc, string warpZoneName)
            : base(width, height, mass, position, bitmap)
        {
            WarpLoc = loc;
            WarpZoneName = warpZoneName;
        }
    }
}
