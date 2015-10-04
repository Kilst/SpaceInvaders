using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class Coin : GameObject
    {
        int quarterWidth = 0;
        int quarterHeight = 0;
        // Call Base class constructor
        public Coin(int width, int height, float mass, Vector2 position, Bitmap bitmap)
            : base(width, height, mass, position, bitmap)
        {
            quarterWidth = width / 4;
            quarterHeight = height / 4;
            GetBounds();
        }

        // Override GetBounds to make the box smaller than the coin ("feels" nice to play)
        public override void GetBounds()
        {
            this.TopLeft = new Vector2(Position.X + quarterWidth, Position.Y + quarterHeight);
            this.TopRight = new Vector2(Position.X + Width - quarterWidth, Position.Y + quarterHeight);
            this.BottomLeft = new Vector2(Position.X + quarterWidth, Position.Y + Height - quarterHeight);
            this.BottomRight = new Vector2(Position.X + Width - quarterWidth, Position.Y + Height - quarterHeight);
        }
    }
}
