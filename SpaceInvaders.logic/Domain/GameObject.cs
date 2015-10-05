using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public abstract class GameObject: iPhysics, iCollisions, IDisposable
    {
        private const double gravity = 0.3;

        public bool Enabled { get; set; }

        public int XCollisionsThisFrame { get; set; }
        public int YCollisionsThisFrame { get; set; }

        public Bitmap Bitmap { get; set; }
        public bool Flipped { get; set; }
        public bool CheckCollisions { get; set; }
        public bool IsGrounded { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public float Mass { get; set; }

        public Vector2 MaxVelocity { get; set; }
        public Vector2 Velocity { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 PreviousPosition { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
            /* Bounding Box */
        public Vector2 TopLeft { get; set; }
        public Vector2 TopRight { get; set; }
        public Vector2 BottomLeft { get; set; }
        public Vector2 BottomRight { get; set; }

        public void Dispose()
        {
            Bitmap = null;
        }

        #region 8-point bounding-box
        //public Vector2 MiddleTopLeft { get; set; }
        //public Vector2 MiddleTopRight { get; set; }
        //public Vector2 MiddleBottomLeft { get; set; }
        //public Vector2 MiddleBottomRight { get; set; }

        /* 8 points to make a bounding box

         . .
        .   .
        .   .
         . .

         */
        #endregion

        // Default Constructor
        public GameObject()
        {
            this.Velocity = new Vector2(0, 0);
            this.PreviousPosition = new Vector2(0, 0);
        }

        public GameObject(int width, int height, float mass, Vector2 position, Bitmap bitmap)
        {
            Enabled = true;
            XCollisionsThisFrame = 0;
            YCollisionsThisFrame = 0;
            this.Bitmap = bitmap;
            this.CheckCollisions = true;
            this.Width = width;
            this.Height = height;
            this.Mass = mass;
            this.MaxVelocity = new Vector2(3, 15);
            this.Velocity = new Vector2(0, 0);
            this.PreviousPosition = new Vector2(0, 0);
            this.Position = position;
            GetBounds();
        }

        public GameObject(int width, int height, float mass, Vector2 maxVelocity, Vector2 position, Bitmap bitmap)
        {
            Enabled = true;
            this.Bitmap = bitmap;
            this.CheckCollisions = true;
            this.Width = width;
            this.Height = height;
            this.Mass = mass;
            this.MaxVelocity = maxVelocity;
            this.Velocity = new Vector2(0, 0);
            this.Position = position;
            GetBounds();
        }

        /* 4-point bounding-box */
        public virtual void GetBounds()
        {
            this.TopLeft = new Vector2(Position.X,Position.Y);
            this.TopRight = new Vector2(Position.X + Width, Position.Y);
            this.BottomLeft = new Vector2(Position.X, Position.Y + Height);
            this.BottomRight = new Vector2(Position.X + Width, Position.Y + Height);
        }

        #region 8-point bounding-box
        //public void GetBoundingBox()
        //{
        //    this.TopLeft = new Vector2((this.Position.X + this.Width/10), this.Position.Y);
        //    this.TopRight = new Vector2((this.Position.X + this.Width - (this.Width / 10)), this.Position.Y);

        //    this.MiddleTopLeft = new Vector2(this.Position.X, (this.Position.Y + this.Width/10));
        //    this.MiddleTopRight = new Vector2((this.Position.X + this.Width), (this.Position.Y + this.Width / 10));

        //    this.MiddleBottomLeft = new Vector2(this.Position.X, (this.Position.Y + this.Height / 10));
        //    this.MiddleBottomRight = new Vector2((this.Position.X + this.Width), ((this.Position.Y + this.Height) - this.Height / 10));

        //    this.BottomLeft = new Vector2((this.Position.X + this.Width / 10), this.Position.Y + this.Height);
        //    this.BottomRight = new Vector2((this.Position.X + this.Width - (this.Width / 10)), this.Position.Y + this.Height);
        //}
        #endregion

        public void MovePosition(Vector2 position)
        {
            PreviousPosition = new Vector2(Position.X, Position.Y);
            OffsetX += position.X;
            OffsetY += position.Y;
            Position.X += position.X;
            Position.Y += position.Y;
            GetBounds();
        }

        public virtual void FallDeathCheck()
        {
            if (Position.Y > 500)
                Enabled = false;
        }

        public bool CheckDistance(GameObject obj)
        {
            // Check X distances
            double myPosition = this.TopLeft.X;
            double objPositionLeft = obj.TopLeft.X;
            double objPositionRight = obj.TopRight.X;

            // Compare (check the object is on the screen)
            if (objPositionLeft - myPosition < 800 && objPositionRight - myPosition > -500)
            {
                // Allow to render and calculate collisions
                obj.CheckCollisions = true;
                return true;
            }
            else
            {
                obj.CheckCollisions = false;
                return false;
            }
        }

        public void VelocityCheck()
        {
            if (Velocity.X >= 0)
            {
                if (Velocity.X >= MaxVelocity.X)
                    Velocity.X = MaxVelocity.X;
            }
            else
            {
                if (Velocity.X <= -MaxVelocity.X)
                    Velocity.X = -MaxVelocity.X;
            }

            if (Velocity.Y >= 0)
            {
                if (Velocity.Y >= MaxVelocity.Y)
                    Velocity.Y = MaxVelocity.Y;
            }
            else
            {
                if (Velocity.Y <= -MaxVelocity.Y)
                    Velocity.Y = -MaxVelocity.Y;
            }
        }

        public void AddVelocity(Vector2 velocity)
        {
            Velocity.AddVector2(velocity);
        }

        public virtual void Move()
        {
            VelocityCheck();
            ApplyGravity();
            ApplyFriction();
            // Store previous position
            PreviousPosition = new Vector2(Position.X, Position.Y);
            // Move object by velocity amount of pixels
            Position.AddVector2(Velocity);
            GetBounds();
        }

        public virtual void CollisionCheck(List<GameObject> list)
        {
            if (list.Count > 0)
            {
                if (list[0].GetType() == typeof(Platform)
                    || list[0].GetType() == typeof(Pipe)
                    || list[0].GetType() == typeof(JumpThroughPlatform))
                {
                    foreach (GameObject item in list)
                    {
                        CollisionCheckY(item);
                    }
                    XCollisionsThisFrame = 0;
                    YCollisionsThisFrame = 0;
                    return;
                    // Ground platforms must be first in xml, otherwise we fall throug floor,
                    // when colliding with another object whilr on ground
                }
            }
        }

        public virtual void XCheck(GameObject platform)
        {
            // Hack solution that works
            if (PreviousPosition.X < platform.TopLeft.X)
                Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
            else if (PreviousPosition.X + Width > platform.TopRight.X)
                Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
            //PreviousPosition = Position;
            Velocity.X = Velocity.X * 0;
        }

        public virtual void CollisionCheckY(GameObject platform)
        {
                // Check to allow calculating collisions
            if (platform.CheckCollisions != false)
            {

                if (TopLeft.X < platform.TopRight.X &&
                   TopRight.X > platform.TopLeft.X &&
                   TopLeft.Y < platform.BottomRight.Y &&
                   BottomRight.Y > platform.TopLeft.Y)
                {
                    XCollisionsThisFrame++;

                    if (PreviousPosition.X + Width < platform.TopLeft.X
                        || PreviousPosition.X > platform.TopRight.X)
                    {
                        // Hack solution
                        if (platform.GetType() != typeof(JumpThroughPlatform))
                        {
                            XCollisionsThisFrame++;
                            XCheck(platform);
                            return;
                        }
                    }

                    YCollisionsThisFrame++;

                    if ((BottomLeft.Y) >= platform.TopLeft.Y
                        && (BottomLeft.Y) <= platform.TopLeft.Y + (platform.Height / 5))
                    {
                        IsGrounded = true;
                        //Position.Y = PreviousPosition.Y + (platform.Position.Y - PreviousPosition.Y - Height);
                    }
                    else
                    {
                        IsGrounded = false;
                        //Position.Y = PreviousPosition.Y + (platform.Position.Y + platform.Height - PreviousPosition.Y);
                    }
                    if (platform.GetType() != typeof(JumpThroughPlatform))
                        Velocity.Y = 0;

                    if (PreviousPosition.Y + Height <= platform.TopLeft.Y)
                    {
                        if (platform.GetType() == typeof(JumpThroughPlatform))
                            Velocity.Y = 0;
                        //IsGrounded = true;
                        Position.Y = PreviousPosition.Y + (platform.TopLeft.Y - PreviousPosition.Y - Height);
                    }
                    else if (PreviousPosition.Y >= platform.BottomRight.Y)
                    {
                        if (platform.GetType() != typeof(JumpThroughPlatform))
                            Position.Y = PreviousPosition.Y + (platform.BottomRight.Y - PreviousPosition.Y);
                        //IsGrounded = false;
                    }
                    //Position.Y = PreviousPosition.Y;
                    GetBounds();
                    return;
                }
                else if (YCollisionsThisFrame == 0 && IsGrounded == true)
                {
                    IsGrounded = false;
                }
            }
        }

        public void ApplyFriction()
        {
            if (IsGrounded == true)
            {
                AddVelocity(new Vector2(Velocity.X * -0.1, Velocity.Y * -0.001));
            }
            else
            {
                AddVelocity(new Vector2(Velocity.X * -0.01, Velocity.Y * -0.001));
            }
        }

        public void ApplyGravity()
        {
            AddVelocity(new Vector2(0, gravity));
        }
    }
}
