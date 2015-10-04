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
    public class GameService
    {
        private Random random;
        private DateTime time = DateTime.Now;
        public Level previous;
        public Level Level { get; set; }

        public GameService()
        {
            random = new Random();
            Level = new Level("Level1");
            previous = Level;
        }

        public bool Update()
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
                return true;
            }
            return false;
        }

        public void PhysicsUpdate()
        {
            int bulletBills = 0;
            // TODO: Fix collision bugs
            Level.Ship.Move();

            foreach (Enemy enemy in Level.Enemies)
            {
                // Check to allow movement
                if (enemy.CheckCollisions == true || enemy.GetType() == typeof(BulletBill))
                {
                    enemy.Move();
                    enemy.CollisionCheck(Level.Platforms);
                    enemy.CollisionCheck(Level.WarpPipes);
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

        private void ScreenCheck()
        {
            // Check whether we move the screen or not

            // X Check
            if ((Level.Ship.Position.X > 500 || Level.Ship.Position.X < 120)
                && (Level.Ship.PreviousPosition.X > 500 || Level.Ship.PreviousPosition.X < 120))
            {
                MoveScreen(new Vector2(-(Level.Ship.Position.X-Level.Ship.PreviousPosition.X), 0));
                Level.Ship.Position.X = Level.Ship.PreviousPosition.X;
            }
            // Y Check
            if ((Level.Ship.Position.Y < 80 || Level.Ship.Position.Y > 220) && Level.Platforms[0].OffsetY > -80)
            {
                MoveScreen(new Vector2(0, -(Level.Ship.Position.Y - Level.Ship.PreviousPosition.Y)));
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

        public string CheckType(GameObject obj)
        {
            if (obj.GetType() == typeof(BulletBill))
            {
                return "BulletBill";
            }
            if (obj.GetType() == typeof(Goomba))
            {
                return "Goomba";
            }
            if (obj.GetType() == typeof(KoopaGreen))
            {
                return "KoopaGreen";
            }

            return "GameObject";
        }

        public Bitmap FlipNpcImage(GameObject npc)
        {
            Enemy enemy = (Enemy)npc;
            return enemy.FlipNPCImage();
        }

        public void End()
        {
            Level.Platforms = null;
            Level.Coins = null;
            Level.Ship = null;
            Level.Enemies = null;
        }
    }
}
