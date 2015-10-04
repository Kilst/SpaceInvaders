using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class BulletBill : Enemy
    {
        private DateTime time = DateTime.Now;
        public int RandomSeconds { get; set; }
        public int RandomYPosition { get; set; }

        // Call Base class constructor
        public BulletBill(int width, int height, float mass, Vector2 maxVelocity, Vector2 position, int minX, int maxX, Bitmap bitmap)
            : base(width, height, mass, maxVelocity, position, bitmap)
        {
            // We could set width, height, mass & position here,
            // but I was just showing what can be done by calling the base constructor instead
            IsGrounded = false;
            Flipped = false;
            Velocity.X = -1;
            PatrolMinX = minX;
            PatrolMaxX = maxX;
        }

        public override void CollisionCheck(List<GameObject> list)
        {
            return;
        }

        public override void GetBounds()
        {
            this.TopLeft = new Vector2(Position.X + 10, Position.Y + 5);
            this.TopRight = new Vector2(Position.X + Width, Position.Y + 5);
            this.BottomLeft = new Vector2(Position.X + 10, Position.Y + Height - 10);
            this.BottomRight = new Vector2(Position.X + Width, Position.Y + Height - 10);
        }

        public override void Move()
        {
            VelocityCheck();
            // Store previous position
            PreviousPosition = new Vector2(Position.X, Position.Y);
            // Move object by velocity amount of pixels
            Position.AddVector2(Velocity);
            GetBounds();

            if (time.AddSeconds(RandomSeconds) < DateTime.Now)
            {
                if (Position.X < PatrolMinX)
                {
                    Position.Y = RandomYPosition;
                    Position.X = PatrolMaxX;
                    GetBounds();
                }
                time = DateTime.Now;
            }

                Velocity.X = -MaxVelocity.X;

        }
    }
}
