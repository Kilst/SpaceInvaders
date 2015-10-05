using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class Enemy : GameObject
    {
        public int PatrolMinX { get; set; }
        public int PatrolMaxX { get; set; }

        // Call Base class constructor
        public Enemy(int width, int height, float mass, Vector2 maxVelocity, Vector2 position, int minX, int maxX, Bitmap bitmap)
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

        // Call Base class constructor
        public Enemy(int width, int height, float mass, Vector2 maxVelocity, Vector2 position, Bitmap bitmap)
            : base(width, height, mass, maxVelocity, position, bitmap)
        {
            // We could set width, height, mass & position here,
            // but I was just showing what can be done by calling the base constructor instead
            IsGrounded = false;
        }

        #region Using Default Base Class Constructor
        //public Enemy(int width, int height, float mass, Vector2 position)
        //{
        //    Width = width;
        //    Height = height;
        //    Mass = mass;
        //    Position = position;
        //    IsGrounded = false;
        //    GetBounds();
        //}
        #endregion

        public override void Move()
        {
            // Call base method
            base.Move();

            // Added functionality
            if (IsGrounded == true)
                Velocity.X = Velocity.X / 1.1;
            else if (Velocity.X > 1)
                Velocity.X = 1;
            else if (Velocity.X < -1)
                Velocity.X = -1;
        }

        public void MoveTowards(SpaceShip ship)
        {
            // Get Offsets
            double XOffset = ship.Position.X - this.Position.X;
            double YOffset = ship.Position.Y - this.Position.Y;
            // Left
            if (XOffset < 0)
            {
                Velocity.X -= 0.1;
            }
            // Right
            else
            {
                Velocity.X += 0.1;
                // Invert XOffset to be consistent with YOffset
                XOffset = XOffset * -1;
            }
            // Jump
            if (YOffset < 0
                && this.IsGrounded == true
                && YOffset < XOffset) /* Only jumps if YOffset is greater */
            {
                Velocity.Y -= 7.5;
            }

            Move();
        }

        public override void CollisionCheck(List<GameObject> list)
        {
            if (list.Count > 0)
            {
                if (list[0].GetType() == typeof(Platform)
                    || list[0].GetType() == typeof(Pipe)
                    || list[0].GetType() == typeof(DestroyableBrick))
                {
                    CollisionCheckY(list);
                    //CollisionCheckX(list);

                    // Ground platforms must be first in xml, otherwise we fall throug floor,
                    // when colliding with another object whilr on ground
                }
            }
        }

        private new void CollisionCheckX(List<GameObject> list)
        {
            foreach (Platform platform in list)
            {
                if (Position.X < platform.TopRight.X &&
                       TopRight.X > platform.TopLeft.X &&
                       Position.Y < platform.BottomRight.Y &&
                       BottomRight.Y > platform.TopLeft.Y)
                {

                    if (PreviousPosition.X < platform.TopLeft.X)
                        Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
                    else if (PreviousPosition.X + Width > platform.TopRight.X)
                        Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
                    Velocity.X = Velocity.X * -1;
                    GetBounds();
                    return;
                }
            }
        }

        private void Y_XCheck(Platform platform)
        {
            // Hack solution that works
            if (PreviousPosition.X < platform.TopLeft.X)
                Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
            else if (PreviousPosition.X + Width > platform.TopRight.X)
                Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
            Velocity.X = Velocity.X * -1;
            GetBounds();
        }

        public Bitmap FlipNPCImage(Bitmap bitmap)
        {
            Bitmap newBitmap = newBitmap = new Bitmap(bitmap);
            if (Velocity.X > 0)
            {
                Flipped = true;
                newBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                return newBitmap;
            }
            else if (Velocity.X < 0)
                Flipped = false;
            return newBitmap;
        }
    }
}
