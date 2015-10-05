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
    public class Level : IDisposable
    {
        public Bitmap[] gifs;
        public Bitmap gameOver;
        public Bitmap backgroundImage;
        public Bitmap platformImage;
        public Bitmap groundImage;
        public Bitmap questionBlock;
        public Bitmap usedBlock;
        public Bitmap marioPipe;
        public Bitmap shipImage;
        public Bitmap coinImage;
        public Bitmap npcImage;
        public Bitmap koopaImage;
        public Bitmap bulletBillImage;

        public List<GameObject> Platforms { get; set; }
        public List<GameObject> DestroyableBricks { get; set; }
        public List<GameObject> WarpPipes { get; set; }
        public List<GameObject> Coins { get; set; }
        public SpaceShip Ship { get; set; }
        public List<GameObject> Enemies { get; set; }
        public int BulletBillCount { get; set; }
        public string Name { get; set; }

        public Level(string level)
        {
            Name = level;
            ResourceLoader.LoadBitmaps(this);
            Ship = new SpaceShip(20, 30, 0, new Vector2(3, 8), new Vector2(120, 100), shipImage);
            Enemies = new List<GameObject>();
            Platforms = new List<GameObject>();
            DestroyableBricks = new List<GameObject>();
            WarpPipes = new List<GameObject>();
            Coins = new List<GameObject>();
            gifs = new Bitmap[4];

            // GetEnemies
            ResourceLoader.GetGameObjects(this, level, "Enemies");
            // GetPlatforms/Pipes
            ResourceLoader.GetGameObjects(this, level, "Platforms");
            ResourceLoader.GetGameObjects(this, level, "DestroyableBricks");
            // GetWarpPipes
            ResourceLoader.GetGameObjects(this, level, "WarpPipes");
            // GetCoins
            ResourceLoader.GetGameObjects(this, level, "Coins");
            CountBulletBills();
            gifs[0] = npcImage;
            gifs[1] = koopaImage;
            gifs[2] = coinImage;
            gifs[3] = questionBlock;
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

        public void Dispose()
        {
            foreach (GameObject item in Platforms)
            {
                item.Dispose();
            }
            foreach (GameObject item in DestroyableBricks)
            {
                item.Dispose();
            }
            foreach (GameObject item in WarpPipes)
            {
                item.Dispose();
            }
            foreach (GameObject item in Coins)
            {
                item.Dispose();
            }
            foreach (GameObject item in Enemies)
            {
                item.Dispose();
            }

            Ship.Dispose();

            Platforms = null;
            WarpPipes = null;
            Coins = null;
            Ship = null;
            Enemies = null;
        }
    }
}
