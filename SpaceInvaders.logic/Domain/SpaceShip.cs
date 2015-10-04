﻿using System;
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

        public void FlipShipImage()
        {
            if (Flipped)
            {
                Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                Flipped = false;
            }
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

        private void WarpPipeY_XCheck(WarpPipe warpPipe)
        {
            // Hack solution that works
            if (PreviousPosition.X + 2 < warpPipe.TopLeft.X)
            {
                Position.X = PreviousPosition.X + (warpPipe.TopLeft.X - PreviousPosition.X - Width) - 1;
            }
            else if (PreviousPosition.X + Width - 2 > warpPipe.TopRight.X)
            {
                Position.X = PreviousPosition.X + (warpPipe.TopRight.X - PreviousPosition.X) + 1;
            }
            Velocity.X = Velocity.X * 0.1;
            GetBounds();
        }

        private void WarpPipeCollisionChecks(List<GameObject> list)
        {
            foreach (WarpPipe warpPipe in list)
            {
                warpPipe.GetBounds();
                if (TopLeft.X < warpPipe.TopRight.X &&
                       TopRight.X > warpPipe.TopLeft.X &&
                       TopLeft.Y < warpPipe.BottomRight.Y &&
                       BottomRight.Y > warpPipe.TopLeft.Y)
                {
                    if ((BottomLeft.Y) > warpPipe.TopLeft.Y
                        && (BottomLeft.Y) < warpPipe.TopLeft.Y + (warpPipe.Height / 2))
                    {
                        if (IsDucking == true)
                        {
                            WarpLocation = warpPipe.WarpLoc;
                            WarpZoneName = warpPipe.WarpZoneName;
                            IsZoning = true;
                        }
                        return;
                    }
                }
            }
        }

        private void WarpPipeCollisionCheckX(List<GameObject> list)
        {
            foreach (WarpPipe warpPipe in list)
            {
                if (Position.X < warpPipe.TopRight.X &&
                       TopRight.X > warpPipe.TopLeft.X &&
                       Position.Y < warpPipe.BottomRight.Y &&
                       BottomRight.Y > warpPipe.TopLeft.Y)
                {

                    if (PreviousPosition.X < warpPipe.TopLeft.X)
                        Position.X = PreviousPosition.X + (warpPipe.TopLeft.X - PreviousPosition.X - Width) - 1;
                    else if (PreviousPosition.X + Width > warpPipe.TopRight.X)
                        Position.X = PreviousPosition.X + (warpPipe.TopRight.X - PreviousPosition.X) + 1;
                    Velocity.X = Velocity.X * 0;
                    GetBounds();
                    return;
                }
            }
        }

        private void WarpPipeCollisionCheckY(List<GameObject> list)
        {
            foreach (WarpPipe platform in list)
            {
                if (Position.X < platform.TopRight.X &&
                       TopRight.X > platform.TopLeft.X &&
                       Position.Y < platform.BottomRight.Y &&
                       BottomRight.Y > platform.TopLeft.Y)
                {
                    if (PreviousPosition.X + Width - 2 < platform.TopLeft.X
                        || PreviousPosition.X > platform.TopRight.X)
                    {
                        WarpPipeY_XCheck(platform);
                        return;
                    }

                    if ((BottomLeft.Y) > platform.TopLeft.Y
                        && (BottomLeft.Y) < platform.TopLeft.Y + (platform.Height / 5))
                    {
                        if (IsDucking == true)
                        {
                            WarpLocation = platform.WarpLoc;
                            WarpZoneName = platform.WarpZoneName;
                            IsZoning = true;
                        }
                        IsGrounded = true;
                    }
                    else
                    {
                        IsGrounded = false;
                        //Position.Y = PreviousPosition.Y + (platform.Position.Y + platform.Height - PreviousPosition.Y);
                    }

                    Velocity.Y = 0;

                    if (PreviousPosition.Y + 1 <= platform.TopLeft.Y)
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
            }
        }

        public override void CollisionCheck(List<GameObject> list)
        {
            if (list.Count > 0)
            {
                if (list[0].GetType().Equals(typeof(WarpPipe)))
                {
                    WarpPipeCollisionCheckY(list);
                    //WarpPipeCollisionCheckX(list);
                    return;
                }

                base.CollisionCheck(list);

                if (list[0].GetType().Equals(typeof(Coin)))
                {
                    CoinCollisionCheck(list);
                    return;
                }

                else if (list[0].GetType().Equals(typeof(Goomba))
                    || list[0].GetType().Equals(typeof(KoopaGreen))
                    || list[0].GetType().Equals(typeof(BulletBill)))
                {
                    EnemyCollisionCheckY(list);
                    //EnemyCollisionCheckX(list);
                    return;
                }
            }
        }

        private void EnemyCollisionCheckX(List<GameObject> list)
        {
            foreach (Enemy platform in list)
            {
                if (Position.X < platform.TopRight.X &&
                       TopRight.X > platform.TopLeft.X &&
                       Position.Y < platform.BottomRight.Y &&
                       BottomRight.Y > platform.TopLeft.Y)
                {

                    if (PreviousPosition.X < platform.TopLeft.X)
                        Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
                    else
                        Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
                    Velocity.X = Velocity.X * 0;
                    GetBounds();
                    IsAlive = false;
                    return;
                }
            }
        }

        private void EnemyY_XCheck(Enemy platform)
        {
            // Hack solution that works
            if (PreviousPosition.X + 2 < platform.TopLeft.X)
            {
                Position.X = PreviousPosition.X + (platform.TopLeft.X - PreviousPosition.X - Width) - 1;
            }
            else if (PreviousPosition.X + Width - 2 > platform.TopRight.X)
            {
                Position.X = PreviousPosition.X + (platform.TopRight.X - PreviousPosition.X) + 1;
            }
            Velocity.X = Velocity.X * 0.1;
            IsAlive = false;
            GetBounds();
        }

        private void EnemyCollisionCheckY(List<GameObject> list)
        {
            foreach (Enemy platform in list)
            {
                if (Position.X < platform.TopRight.X &&
                       TopRight.X > platform.TopLeft.X &&
                       Position.Y < platform.BottomRight.Y &&
                       BottomRight.Y > platform.TopLeft.Y)
                {
                    if (PreviousPosition.X + Width - 2 < platform.TopLeft.X
                        || PreviousPosition.X > platform.TopRight.X)
                    {
                        EnemyY_XCheck(platform);
                        return;
                    }


                    if ((BottomLeft.Y) > platform.TopLeft.Y
                        && (BottomLeft.Y) < platform.TopLeft.Y + (platform.Height / 2))
                    {
                        // Disable enemy
                        list.Remove(platform);
                        IsGrounded = false;
                        Velocity.Y = -4;
                        Position = PreviousPosition;
                        GetBounds();
                        return;
                    }

                    IsGrounded = false;
                    Velocity.Y = Velocity.Y * -1;
                    IsAlive = false;

                    if (PreviousPosition.Y + 1 <= platform.TopLeft.Y)
                    {
                        Position.Y = PreviousPosition.Y + (platform.TopLeft.Y - PreviousPosition.Y - Height);
                    }
                    else
                    {
                        Position.Y = PreviousPosition.Y + (platform.BottomRight.Y - PreviousPosition.Y);
                        Velocity.Y = 0;
                    }
                    //Position.Y = PreviousPosition.Y;
                    GetBounds();
                    return;
                }
            }
        }
    }
}