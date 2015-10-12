using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;

namespace SpaceInvaders.logic.Domain
{
    public class SpaceShip : GameObject
    {
        public bool IsMoving { get; set; }
        public bool IsJumping { get; set; }
        public bool IsAlive { get; set; }
        public bool IsDucking { get; set; }
        public bool IsZoning { get; set; }

        public int Direction { get; set; }

        public string WarpZoneName { get; set; }
        public Vector2 WarpLocation { get; set; }

        // Call Base class constructor
        public SpaceShip(int width, int height, float mass, Vector2 maxVelocity, Vector2 position, Bitmap bitmap)
            : base(width, height, mass, maxVelocity, position, bitmap)
        {
            // We could set width, height, mass & position here (see below region)
            // but this is more elegant (less repeated code)
            IsMoving = false;
            IsJumping = false;
            IsGrounded = false;
            IsAlive = true;
            IsDucking = false;
            IsZoning = false;
            Flipped = false;
            Direction = 100;
        }

        #region Using Default Base Class Constructor
        //public SpaceShip(int width, int height, float mass, Vector2 position)
        //{
        //    Width = width;
        //    Height = height;
        //    Mass = mass;
        //    Position = position;
        //    IsGrounded = false;
        //    GetBounds();
        //}
        #endregion

        public Bitmap FlipShipImage(Bitmap bitmap, int imageIndex)
        {
            Bitmap newBitmap = new Bitmap(bitmap);
            if (Direction == 100 && imageIndex == 1)
            {
                newBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            if (Direction == 100 && imageIndex == 2)
            {
                newBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            return newBitmap;
        }

        public override void FallDeathCheck()
        {
            if (Position.Y > 500)
                IsAlive = false;
        }

        private void CoinCollisionCheck(List<GameObject> list)
        {
            foreach (Coin coin in list)
            {
                coin.GetBounds();
                if (TopLeft.X < coin.TopRight.X &&
                       TopRight.X > coin.TopLeft.X &&
                       TopLeft.Y < coin.BottomRight.Y &&
                       BottomRight.Y > coin.TopLeft.Y)
                {
                    list.Remove(coin);
                    return;
                }
            }
        }

        public override void XCheck(GameObject platform)
        {
            // Hack solution that works
            if (PreviousPosition.X + Width < platform.TopLeft.X && platform.TopLeft.Y < BottomLeft.Y - 1)
            {
                Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
                Velocity.X = Velocity.X * 0;
            }
            else if (PreviousPosition.X > platform.TopRight.X && platform.TopLeft.Y < BottomLeft.Y - 1)
            {
                Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
                Velocity.X = Velocity.X * 0;
            }

            if (platform.GetType() == typeof(KoopaGreen)
                || platform.GetType() == typeof(Goomba)
                || platform.GetType() == typeof(BulletBill))
                IsAlive = false;
        }

        public override void CollisionCheckY(GameObject platform)
        {
            // Check to allow calculating collisions
            if (platform.CheckCollisions != false)
            {

                if (TopLeft.X < platform.TopRight.X &&
                   TopRight.X > platform.TopLeft.X &&
                   TopLeft.Y < platform.BottomRight.Y &&
                   BottomRight.Y > platform.TopLeft.Y)
                {

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
                        && (BottomLeft.Y) <= platform.TopLeft.Y + (platform.Height / 2)
                        && (platform.GetType() == typeof(KoopaGreen)
                        || platform.GetType() == typeof(Goomba)
                        || platform.GetType() == typeof(BulletBill)))
                    {
                        Enemy enemy = (Enemy)platform;
                        // Disable enemy
                        enemy.Enabled = false;
                        IsGrounded = false;
                        Velocity.Y = -4;
                        Position = PreviousPosition;
                        GetBounds();
                        return;
                    }

                    if ((BottomLeft.Y) >= platform.TopLeft.Y
                        && (BottomLeft.Y) <= platform.TopLeft.Y + (platform.Height / 5))
                    {
                        if (platform.GetType() == typeof(WarpPipe))
                        {
                            WarpPipe warpPipe = (WarpPipe)platform;
                            if (IsDucking == true)
                            {
                                WarpLocation = warpPipe.WarpLoc;
                                WarpZoneName = warpPipe.WarpZoneName;
                                IsZoning = true;
                                Velocity.X = 0;
                                Velocity.Y = 0;
                                IsDucking = false;
                            }
                        }
                        IsGrounded = true;
                        //Position.Y = PreviousPosition.Y + (platform.Position.Y - PreviousPosition.Y - Height);
                    }

                    if (platform.GetType() == typeof(KoopaGreen)
                        || platform.GetType() == typeof(Goomba)
                        || platform.GetType() == typeof(BulletBill))
                        IsAlive = false;

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
                        if (platform.GetType() == typeof(QuestionBlock))
                        {
                            QuestionBlock brick = (QuestionBlock)platform;
                            if (!brick.Used)
                            {
                                brick.Used = true;
                            }
                        }
                        else if (platform.GetType() == typeof(DestroyableBrick))
                        {
                            platform.Enabled = false;
                        }

                        if (platform.GetType() != typeof(JumpThroughPlatform))
                            Position.Y = PreviousPosition.Y + (platform.BottomRight.Y - PreviousPosition.Y);
                    }
                    //Position.Y = PreviousPosition.Y;
                    GetBounds();
                    return;
                }
                else if (YCollisionsThisFrame == 0 
                        && IsGrounded == true 
                        && (platform.GetType() == typeof(Platform)
                        || platform.GetType() == typeof(Pipe)
                        || platform.GetType() == typeof(JumpThroughPlatform)))
                {
                    IsGrounded = false;
                }
            }
        }

        public override void CollisionCheck(List<GameObject> list)
        {
            if (list.Count > 0)
            {
                if (list[0].GetType() != typeof(Coin))
                {
                    for (int i = 0; i < list.Count(); i++)
                    {
                        CollisionCheckY(list[i]);
                        if (!list[i].Enabled)
                            list.Remove(list[i]);
                    }
                    XCollisionsThisFrame = 0;
                    YCollisionsThisFrame = 0;
                    return;
                }

                if (list[0].GetType().Equals(typeof(Coin)))
                {
                    CoinCollisionCheck(list);
                    return;
                }
            }
        }
    }
}
