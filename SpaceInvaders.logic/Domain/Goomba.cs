using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class Goomba : Enemy
    {
        // Call Base class constructor
        public Goomba(int width, int height, float mass, Vector2 maxVelocity, Vector2 position, int minX, int maxX, Bitmap bitmap)
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

        public override void Move()
        {
            // Call base method
            base.Move();

            if (Position.X - OffsetX < PatrolMinX || Position.X - OffsetX > PatrolMaxX)
            {
                if (Position.X - OffsetX < PatrolMinX)
                    Position.X = PatrolMinX + OffsetX;
                else
                    Position.X = PatrolMaxX + OffsetX;

                Velocity.X = Velocity.X * -1;
            }

            if (Velocity.X > 0)
                Velocity.X = 1;
            else if (Velocity.X < 0)
                Velocity.X = -1;
        }
    }
}
