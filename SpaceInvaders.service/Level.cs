﻿using System;
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
        public Bitmap loadingImage;
        public Bitmap gameOver;
        public Bitmap backgroundImage;
        public Bitmap foregroundImage;
        public Bitmap platformGreyImage;
        public Bitmap platformWoodenImage;
        public Bitmap platformImage;
        public Bitmap invisiblePlatformImage;
        public Bitmap groundImage;
        public Bitmap questionBlock;
        public Bitmap usedBlock;
        public Bitmap marioPipe;
        public Bitmap shipImage;
        public Bitmap coinImage;
        public Bitmap npcImage;
        public Bitmap koopaImage;
        public Bitmap bulletBillImage;

        public double offsetX;
        public double offsetY;

        public List<GameObject> Platforms { get; set; }
        public List<GameObject> DestroyableBricks { get; set; }
        public List<GameObject> WarpPipes { get; set; }
        public List<GameObject> Coins { get; set; }
        public SpaceShip Ship { get; set; }
        public List<GameObject> Enemies { get; set; }
        public int BulletBillCount { get; set; }
        public string LevelName { get; set; }

        public Level(string level)
        {
            LevelName = level;
            ResourceLoader.LoadBitmaps(this);
            Ship = new SpaceShip(17, 30, 0, new Vector2(3, 8), new Vector2(120, 100), shipImage);
            Enemies = new List<GameObject>();
            Platforms = new List<GameObject>();
            DestroyableBricks = new List<GameObject>();
            WarpPipes = new List<GameObject>();
            Coins = new List<GameObject>();
            gifs = new Bitmap[5];

            // GetEnemies
            ResourceLoader.GetGameObjects(this, LevelName, "Enemies");
            // GetPlatforms/Pipes
            ResourceLoader.GetGameObjects(this, LevelName, "Platforms");
            ResourceLoader.GetGameObjects(this, LevelName, "DestroyableBricks");
            // GetWarpPipes
            ResourceLoader.GetGameObjects(this, LevelName, "WarpPipes");
            // GetCoins
            ResourceLoader.GetGameObjects(this, LevelName, "Coins");
            CountBulletBills();
            gifs[0] = npcImage;
            gifs[1] = koopaImage;
            gifs[2] = coinImage;
            gifs[3] = questionBlock;
            gifs[4] = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\bigMario.gif");
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
