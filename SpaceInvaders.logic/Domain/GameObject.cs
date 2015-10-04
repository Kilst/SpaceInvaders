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
        }

        public GameObject(int width, int height, float mass, Vector2 position, Bitmap bitmap)
        {
            this.Bitmap = bitmap;
            this.CheckCollisions = true;
            this.Width = width;
            this.Height = height;
            this.Mass = mass;
            this.MaxVelocity = new Vector2(3, 15);
            this.Velocity = new Vector2(0, 0);
            this.Position = position;
            GetBounds();
        }

        public GameObject(int width, int height, float mass, Vector2 maxVelocity, Vector2 position, Bitmap bitmap)
        {
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
            OffsetX += position.X;
            OffsetY += position.Y;
            Position.X += position.X;
            Position.Y += position.Y;
            GetBounds();
        }

        public bool CheckDistance(GameObject obj)
        {
            // Check X distances
            double myPosition = this.Position.X;
            double objPosition = obj.Position.X;

            // Normalise the distances
            //if (myPosition < 0)
            //    myPosition = myPosition * -1;
            //if (objPosition < 0)
            //    objPosition = objPosition * -1;

            // Compare (check the object is on the screen)
            if (objPosition - myPosition < 800 && objPosition - myPosition > -500)
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
                    || list[0].GetType() == typeof(Pipe))
                {
                    CollisionCheckY(list);
                    CollisionCheckX(list);
                    return;
                    // Ground platforms must be first in xml, otherwise we fall throug floor,
                    // when colliding with another object whilr on ground
                }
            }
        }

        public void CollisionCheckX(List<GameObject> list)
        {
            foreach (Platform platform in list)
            {
                if (TopLeft.X < platform.TopRight.X &&
                       TopRight.X > platform.TopLeft.X &&
                       TopLeft.Y < platform.BottomRight.Y &&
                       BottomRight.Y > platform.TopLeft.Y)
                {

                    if (PreviousPosition.X < platform.TopLeft.X)
                        Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
                    else if (PreviousPosition.X + Width > platform.TopRight.X)
                        Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
                    Velocity.X = Velocity.X * 0;
                    GetBounds();
                    return;
                }
            }
        }

        #region collision code (unused)
        //public void CollisionCheck(List<Platform> list)
        //{
        //    bool TopLeftColliding = false;
        //    bool TopRightColliding = false;
        //    bool BottomLeftColliding = false;
        //    bool BottomRightColliding = false;
        //    foreach (Platform platform in list)
        //    {
        //        if (Position.X < platform.Position.X + platform.Width &&
        //               Position.X + Width > platform.Position.X &&
        //               Position.Y < platform.Position.Y + platform.Height &&
        //               Height + Position.Y > platform.Position.Y)
        //        {
        //            if (TopLeft.X > platform.TopLeft.X
        //                && TopLeft.X < platform.TopRight.X
        //                && TopLeft.Y > platform.TopLeft.Y
        //                && TopLeft.Y < platform.BottomRight.Y)
        //            {
        //                Tuple<bool, bool> check = TopLeft.GreaterThan(platform.BottomRight);
        //                if (check.Item1 == true && check.Item2 == false)
        //                {
        //                    CollisionCheckX(list);
        //                    //CollisionCheckY(list);
        //                    return;
        //                }
        //                else
        //                {
        //                    CollisionCheckY(list);
        //                    //CollisionCheckX(list);
        //                    return;
        //                }
        //                TopLeftColliding = true;
        //                //if (TopLeft.X - platform.TopRight.X < 0)
        //            }
        //            if (TopRight.X > platform.TopLeft.X
        //                && TopRight.X < platform.TopRight.X
        //                && TopRight.Y > platform.TopLeft.Y
        //                && TopRight.Y < platform.BottomRight.Y)
        //            {
        //                Tuple<bool,bool> check = TopRight.GreaterThan(platform.BottomLeft);
        //                if (check.Item1 == true && check.Item2 == false)
        //                {
        //                    CollisionCheckX(list);
        //                    //CollisionCheckY(list);
        //                    return;
        //                }
        //                else
        //                {
        //                    CollisionCheckY(list);
        //                    //CollisionCheckX(list);
        //                    return;
        //                }
        //                TopRightColliding = true;
        //            }
        //            if (BottomLeft.X > platform.TopLeft.X
        //                && BottomLeft.X < platform.TopRight.X
        //                && BottomLeft.Y > platform.TopLeft.Y
        //                && BottomLeft.Y < platform.BottomRight.Y)
        //            {
        //                Tuple<bool, bool> check = BottomLeft.GreaterThan(platform.TopRight);
        //                if (check.Item1 == true && check.Item2 == false)
        //                {
        //                    CollisionCheckX(list);
        //                    //CollisionCheckY(list);
        //                    return;
        //                }
        //                else
        //                {
        //                    CollisionCheckY(list);
        //                    //CollisionCheckX(list);
        //                    return;
        //                }
        //                BottomLeftColliding = true;
        //            }
        //            if (BottomRight.X > platform.TopLeft.X
        //                && BottomRight.X < platform.TopRight.X
        //                && BottomRight.Y > platform.TopLeft.Y
        //                && BottomRight.Y < platform.BottomRight.Y)
        //            {
        //                Tuple<bool, bool> check = BottomRight.GreaterThan(platform.TopLeft);
        //                if (check.Item1 == true && check.Item2 == false)
        //                {
        //                    CollisionCheckX(list);
        //                    //CollisionCheckY(list);
        //                    return;
        //                }
        //                else
        //                {
        //                    CollisionCheckY(list);
        //                    //CollisionCheckX(list);
        //                    return;
        //                }
        //                BottomRightColliding = true;
        //            }
        //            GetBounds();
        //            return;
        //        }
        //    }
        //}
        #endregion

        private void Y_XCheck(Platform platform)
        {
            // Hack solution that works
            if (PreviousPosition.X < platform.TopLeft.X)
                Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
            else if (PreviousPosition.X + Width > platform.TopRight.X)
                Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
            //PreviousPosition = Position;
            Velocity.X = Velocity.X * 0.5;
            GetBounds();
        }

        public void CollisionCheckY(List<GameObject> list)
        {
            foreach (Platform platform in list)
            {
                // Check to allow calculating collisions
                if (platform.CheckCollisions != false)
                {

                    if (Position.X < platform.TopRight.X &&
                           TopRight.X > platform.TopLeft.X &&
                           Position.Y < platform.BottomRight.Y &&
                           BottomRight.Y > platform.TopLeft.Y)
                    {
                        if (PreviousPosition.X + Width - 1 < platform.TopLeft.X
                            || PreviousPosition.X + 1 > platform.TopRight.X)
                        {
                            // Hack solution
                            Y_XCheck(platform);
                            return;
                        }


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

                        Velocity.Y = 0;

                        if (PreviousPosition.Y <= platform.TopLeft.Y)
                        {
                            //IsGrounded = true;
                            Position.Y = PreviousPosition.Y + (platform.TopLeft.Y - PreviousPosition.Y - Height);
                        }
                        else
                        {
                            Position.Y = PreviousPosition.Y + (platform.BottomRight.Y - PreviousPosition.Y);
                            //IsGrounded = false;
                        }
                        //Position.Y = PreviousPosition.Y;
                        GetBounds();
                        return;
                    }
                    else
                    {
                        IsGrounded = false;
                    }
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
