using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;
using SpaceInvaders.logic.Domain;

namespace SpaceInvaders.service
{
    public class Level
    {
        public Bitmap backgroundImage;
        public Bitmap platformImage;
        public Bitmap marioPipe;
        public Bitmap shipImage;
        public Bitmap coinImage;
        public Bitmap npcImage;
        public Bitmap koopaImage;
        public Bitmap bulletBillImage;

        public List<GameObject> Platforms { get; set; }
        public List<GameObject> WarpPipes { get; set; }
        public List<GameObject> Coins { get; set; }
        public SpaceShip Ship { get; set; }
        public List<GameObject> Enemies { get; set; }
        public int BulletBillCount { get; set; }
        public string Name { get; set; }

        public Level(string level)
        {
            ResourceLoader.LoadBitmaps(this);
            Ship = new SpaceShip(20, 30, 0, new Vector2(3, 8), new Vector2(120, 100), shipImage);
            Enemies = new List<GameObject>();
            Platforms = new List<GameObject>();
            WarpPipes = new List<GameObject>();
            Coins = new List<GameObject>();
            Name = level;

            // GetEnemies
            ResourceLoader.GetGameObjects(this, level, "Enemies");
            // GetPlatforms/Pipes
            ResourceLoader.GetGameObjects(this, level, "Platforms");
            // GetWarpPipes
            ResourceLoader.GetGameObjects(this, level, "WarpPipe");
            // GetCoins
            ResourceLoader.GetGameObjects(this, level, "Coins");
            CountBulletBills();
        }

        private void CountBulletBills()
        {
            BulletBillCount = 0;
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.GetType() == typeof(BulletBill))
                    BulletBillCount++;
            }
        }
    }
}
