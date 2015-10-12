using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using SpaceInvaders.logic.Domain;

namespace SpaceInvaders.service
{
    public class GameService : IDisposable
    {
        private Random random;
        private DateTime time = DateTime.Now;
        public Level previous;
        public Level Level { get; set; }
        public bool Zoning { get; set; }


        public GameService()
        {
            random = new Random();
            Level = new Level("Level1");
            previous = Level;
            Zoning = false;
        }

        public bool UpdateLevel()
        {
            if (Level.Ship.IsZoning == true)
            {
                
                string level = Level.Ship.WarpZoneName;
                Vector2 newPos = Level.Ship.WarpLocation;
                if (previous != null)
                {
                    if (level != previous.Name)
                    {
                        Level = new Level(level);
                        //Level.Ship.Position = newPos;
                    }
                    else
                    {
                        Level = previous;
                        //Level.Ship.Position = newPos;
                    }
                    MoveScreen(new Vector2(-newPos.X, -newPos.Y));
                    Level.Ship.GetBounds();
                }
                else
                    Level = new Level(level);
                Level.Ship.IsZoning = false;
                Zoning = true;
                return true;
            }
            Level.Ship.IsZoning = false;
            return false;
        }

        public void PhysicsUpdate()
        {
            //UpdateLevel();
            BrickCheck();
            Level.Ship.FallDeathCheck();
            int bulletBills = 0;
            // TODO: Fix collision bugs
            Level.Ship.Move();

            foreach (Enemy enemy in Level.Enemies)
            {
                // Check to allow movement
                if (enemy.CheckCollisions == true || enemy.GetType() == typeof(BulletBill))
                {
                    enemy.FallDeathCheck();

                    enemy.Move();

                    // Order is specific here
                    enemy.CollisionCheck(Level.DestroyableBricks);
                    enemy.CollisionCheck(Level.WarpPipes);
                    enemy.CollisionCheck(Level.Platforms);
                }
                // Check Bullet Bill object exists
                if (enemy.GetType() == typeof(BulletBill))
                {
                    BulletBill bill = (BulletBill)enemy;
                    bill.RandomSeconds = random.Next(7, 40);
                    bill.RandomYPosition = random.Next(-300, 300);
                    bulletBills++;
                }
            }



            // Polymorphism :)
            Level.Ship.CollisionCheck(Level.Platforms);
            //Level.Ship.CollisionCheck(Level.JumpThroughPlatforms);
            Level.Ship.CollisionCheck(Level.DestroyableBricks);
            Level.Ship.CollisionCheck(Level.WarpPipes);
            Level.Ship.CollisionCheck(Level.Coins);
            Level.Ship.CollisionCheck(Level.Enemies);
            // Screen moves when running into a wall :/
            ScreenCheck();

            if (time.AddSeconds(15) < DateTime.Now)
            {
                // Check Bullet Bill object exists. If not, create a new one.
                if (bulletBills < Level.BulletBillCount)
                    Level.Enemies.Add(new BulletBill(70, 50, 0, new Vector2(3, 7), new Vector2(1100, 30), 0, 1200, Level.bulletBillImage));

                time = DateTime.Now;
            }
        }

        private void BrickCheck()
        {
            foreach (DestroyableBrick brick in Level.DestroyableBricks)
            {
                if (brick.GetType() == typeof(QuestionBlock))
                {
                    QuestionBlock block = (QuestionBlock)brick;
                    ResourceLoader.SwapQuestionBlockImage(Level, block);
                }
            }
        }

        public void AddPlayerVelocity(int direction) // Add direction Enum, or constants
        {
            switch (direction)
            {
                case (int)Directions.left:
                    if (Level.Ship.IsGrounded == true)
                    {
                        Level.Ship.Velocity.X -= 0.2;
                    }
                    else
                        Level.Ship.Velocity.X -= 0.1;
                    Level.Ship.IsMoving = true;
                    Level.Ship.Direction = direction;
                    break;
                case (int)Directions.right:
                    if (Level.Ship.IsGrounded == true)
                    {
                        Level.Ship.Velocity.X += 0.2;
                    }
                    else
                        Level.Ship.Velocity.X += 0.1;
                    Level.Ship.IsMoving = true;
                    Level.Ship.Direction = direction;
                    break;
                case (int)Directions.up:
                    if (Level.Ship.IsGrounded == true && Level.Ship.IsJumping == false)
                    {
                        Level.Ship.Velocity.Y -= 6.3;
                        Level.Ship.IsJumping = true;
                    }
                    else if (Level.Ship.Velocity.Y < -1)
                        Level.Ship.Velocity.Y -= 0.07;
                    else
                        Level.Ship.IsJumping = false;
                    break;
                case (int)Directions.down:
                    //Level.Ship.Velocity.Y += 0.1;
                    break;
            }
        }

        public bool GetPlayerJumping()
        {
            return Level.Ship.IsJumping;
        }

        public void SetPlayerDucking(bool ducking)
        {
            if (Level.Ship.IsDucking != ducking)
            {
                Level.Ship.IsDucking = ducking;
            }
        }

        public bool IsPlayerAlive()
        {
            return Level.Ship.IsAlive;
        }

        public void SetPlayerAlive(bool aliveState)
        {
            Level.Ship.IsAlive = aliveState;
        }

        public void MovingCheck()
        {
            if (Level.Ship.Velocity.X < 0.5 || Level.Ship.Velocity.X > -0.5)
            {
                Level.Ship.IsMoving = false;
                Level.Ship.IsDucking = false;
            }
        }

        private void ScreenCheck()
        {
            // Check whether we move the screen or not

            // X Check
            if ((Level.Ship.Position.X > 500 || Level.Ship.Position.X < 120)
                && (Level.Ship.PreviousPosition.X > 500 || Level.Ship.PreviousPosition.X < 120))
            {
                double offsetX = -(Level.Ship.Position.X - Level.Ship.PreviousPosition.X);
                Level.offsetX += offsetX;
                MoveScreen(new Vector2(offsetX, 0));
                Level.Ship.Position.X = Level.Ship.PreviousPosition.X;
            }
            // Y Check
            if ((Level.Ship.Position.Y < 80 || Level.Ship.Position.Y > 220) && Level.Platforms[0].OffsetY > -80)
            {
                double offsetY = -(Level.Ship.Position.Y - Level.Ship.PreviousPosition.Y);
                Level.offsetY += offsetY;
                MoveScreen(new Vector2(0, offsetY));
                Level.Ship.Position.Y = Level.Ship.PreviousPosition.Y;
            }
            //Level.Ship.GetBounds();
        }

        private void MoveScreen(Vector2 direction)
        {
            // Move all non-player objects on the screen
            foreach (Platform platform in Level.Platforms)
            {
                platform.MovePosition(direction);
                //platform.GetBounds();
            }
            foreach (DestroyableBrick platform in Level.DestroyableBricks)
            {
                platform.MovePosition(direction);
                //platform.GetBounds();
            }
            foreach (WarpPipe warpPipe in Level.WarpPipes)
            {
                warpPipe.MovePosition(direction);
                //warpPipe.GetBounds();
            }
            foreach (Coin coin in Level.Coins)
            {
                coin.MovePosition(direction);
                //coin.GetBounds();
            }
            foreach (Enemy enemy in Level.Enemies)
            {
                enemy.MovePosition(direction);
                //enemy.GetBounds();
            }
        }

        public void Dispose()
        {
            Level.Dispose();
            Level.Platforms = null;
            Level.Coins = null;
            Level.Ship = null;
            Level.Enemies = null;
        }
    }
}