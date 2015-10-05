using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;
using System.Xml.Linq;
using SpaceInvaders.logic.Domain;

namespace SpaceInvaders.service
{
    public class ResourceLoader
    {
        public static void LoadBitmaps(Level level)
        {
            // Load bitmaps
            level.gameOver = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\gameOver.gif");
            GetGameObjects(level, level.Name, "Backgrounds");
            level.platformImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\wall.bmp");
            level.groundImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\ground.png");
            level.questionBlock = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\questionBlock.gif");
            level.usedBlock = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\usedBlock.png");
            level.marioPipe = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\marioPipe.png");
            level.shipImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\bigMario.gif");
            level.coinImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\marioCoin.gif");
            level.npcImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\goomba.gif");
            level.koopaImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\koopagreen.gif");
            level.bulletBillImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\bulletBill.png");
            
            
            level.marioPipe.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        public static void SwapQuestionBlockImage(Level level, QuestionBlock brick)
        {
            if (brick.Used)
                brick.Bitmap = level.usedBlock;
        }

        public static void GetGameObjects(Level level, string levelName, string gameObject)
        {
            // Messy LINQ
            XElement gameData = XElement.Load(Environment.CurrentDirectory + @"\Resources\GameData.xml");
            IEnumerable<XElement> childElements =
                from element in gameData.Elements()
                where element.Name == levelName
                && element.Parent.Name == "Levels"
                select element;
            childElements =
                from element in childElements.Elements()
                where element.Name == gameObject
                && element.Parent.Name == levelName
                select element;
            childElements =
                from element in childElements.Elements()
                select element;
            foreach (XElement element in childElements)
            {
                string name = element.Name.ToString();
                switch (name)
                {
                    case "Platform":
                        level.Platforms.Add(new Platform(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), new Bitmap(level.platformImage)));
                        break;
                    case "JumpThroughPlatform":
                        level.Platforms.Add(new JumpThroughPlatform(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("ImageHeight")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), new Bitmap(level.groundImage)));
                        break;
                    case "Pipe":
                        level.Platforms.Add(new Pipe(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), new Bitmap(level.marioPipe)));
                        break;
                    case "WarpPipe":
                        level.WarpPipes.Add(new WarpPipe(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), new Bitmap(level.marioPipe), new Vector2(int.Parse(element.Attribute(XName.Get("TeleportX")).Value), int.Parse(element.Attribute(XName.Get("TeleportY")).Value)), element.Attribute(XName.Get("WarpZoneName")).Value));
                        break;
                    case "DestroyableBrick":
                        level.DestroyableBricks.Add(new DestroyableBrick(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), new Bitmap(level.platformImage)));
                        break;
                    case "QuestionBlock":
                        level.DestroyableBricks.Add(new QuestionBlock(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), new Bitmap(level.questionBlock)));
                        break;
                    case "Goomba":
                        level.Enemies.Add(new Goomba(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("MaxVelX")).Value), int.Parse(element.Attribute(XName.Get("MaxVelY")).Value)), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), int.Parse(element.Attribute(XName.Get("MinPatrolX")).Value), int.Parse(element.Attribute(XName.Get("MaxPatrolX")).Value), new Bitmap(level.npcImage)));
                        break;
                    case "KoopaGreen":
                        level.Enemies.Add(new KoopaGreen(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("MaxVelX")).Value), int.Parse(element.Attribute(XName.Get("MaxVelY")).Value)), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), int.Parse(element.Attribute(XName.Get("MinPatrolX")).Value), int.Parse(element.Attribute(XName.Get("MaxPatrolX")).Value), new Bitmap(level.koopaImage)));
                        break;
                    case "BulletBill":
                        level.Enemies.Add(new BulletBill(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("MaxVelX")).Value), int.Parse(element.Attribute(XName.Get("MaxVelY")).Value)), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), int.Parse(element.Attribute(XName.Get("MinPatrolX")).Value), int.Parse(element.Attribute(XName.Get("MaxPatrolX")).Value), new Bitmap(level.bulletBillImage)));
                        break;
                    case "Coin":
                        level.Coins.Add(new Coin(int.Parse(element.Attribute(XName.Get("Width")).Value), int.Parse(element.Attribute(XName.Get("Height")).Value), int.Parse(element.Attribute(XName.Get("Mass")).Value), new Vector2(int.Parse(element.Attribute(XName.Get("X")).Value), int.Parse(element.Attribute(XName.Get("Y")).Value)), new Bitmap(level.coinImage)));
                        break;
                    case "Background":
                        level.backgroundImage = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"\Resources\" + element.Attribute(XName.Get("Name")).Value);
                        break;
                }
            }
        }
    }
}
