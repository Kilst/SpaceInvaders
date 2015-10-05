using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class QuestionBlock : DestroyableBrick
    {
        int eighthWidth = 0;
        int eighthHeight = 0;

        public bool Used { get; set; }

        // Call Base class constructor
        public QuestionBlock(int width, int height, float mass, Vector2 position, Bitmap bitmap)
            : base(width, height, mass, position, bitmap)
        {
            eighthWidth = width / 8;
            eighthHeight = height / 8;
            Used = false;
            GetBounds();
        }

        public override void GetBounds()
        {
            if (!Used)
                base.GetBounds();
            else
            {
                this.TopLeft = new Vector2(Position.X, Position.Y );
                this.TopRight = new Vector2(Position.X + Width, Position.Y );
                this.BottomLeft = new Vector2(Position.X, Position.Y + Height - eighthHeight);
                this.BottomRight = new Vector2(Position.X + Width, Position.Y + Height - eighthHeight);
            }
        }
    }
}
