using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Threading;
using System.Drawing;
using SpaceInvaders.logic.Domain;

namespace SpaceInvaders.service
{
    public class DrawBuffer
    {
        public Bitmap buffer;
        Graphics graphics;
        bool currentlyAnimating;
        bool checkedGameEnded = false;
        Level thisLevel;
        public bool painting = false;

        public DrawBuffer(Level level)
        {
            thisLevel = level;
            buffer = new Bitmap(734, 400);
            currentlyAnimating = false;
            for (int i = 0; i < thisLevel.gifs.Count(); i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunAnimation), (int)i);
            }
        }

        void RunAnimation(object i)
        {
            int index = (int)i;
            Bitmap b = thisLevel.gifs[index];
            
            // ImageAnimator class and an event handler
            ImageAnimator.Animate(b, new EventHandler(this.OnFrameChanged));
        }

        public void AnimateImage(Bitmap animatedImage)
        {
            if (!currentlyAnimating)
            {
                //Begin the animation only once.
                ImageAnimator.Animate(animatedImage, new EventHandler(this.OnFrameChanged));
                currentlyAnimating = true;
            }
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            //Force a call to the Paint event handler.
            //this.Invalidate();
        }

        public Bitmap Draw(GameService game)
        {
            painting = true;
            graphics = Graphics.FromImage(buffer);
            ImageAnimator.UpdateFrames();
            // Render graphics
            if (game.Level.Ship.IsZoning)
            {
                graphics.Clear(Color.Black);
                graphics.DrawImage(game.Level.loadingImage, 0, 0, 700, 300);
            }
            else if (game != null && !game.Level.Ship.IsZoning && game.Level.Ship.IsAlive)
            {
                // Rendering order is important
                graphics.DrawImage(game.Level.backgroundImage, new Point(0, 0));

                for (int i = 0; i < game.Level.Platforms.Count; i++)
                {
                    if (game.Level.Platforms[i].GetType() == typeof(JumpThroughPlatform))
                    {
                        if (game.Level.Ship.CheckDistance(game.Level.Platforms[i]))
                        {
                            JumpThroughPlatform jPlatform = (JumpThroughPlatform)game.Level.Platforms[i];
                            graphics.DrawImage(jPlatform.Bitmap, (int)jPlatform.Position.X,
                        (int)jPlatform.Position.Y, jPlatform.Width, jPlatform.ImageHeight);
                        }
                    }
                    else
                        graphics.DrawImage(game.Level.Platforms[i].Bitmap, (int)game.Level.Platforms[i].Position.X,
                        (int)game.Level.Platforms[i].Position.Y, game.Level.Platforms[i].Width, game.Level.Platforms[i].Height);
                }

                for (int i = 0; i < game.Level.WarpPipes.Count; i++)
                {
                    graphics.DrawImage(game.Level.WarpPipes[i].Bitmap, (int)game.Level.WarpPipes[i].Position.X,
                    (int)game.Level.WarpPipes[i].Position.Y, game.Level.WarpPipes[i].Width, game.Level.WarpPipes[i].Height);
                }

                for (int i = 0; i < game.Level.DestroyableBricks.Count; i++)
                {
                    // Check to allow to render and calculate collisions
                    if (game.Level.Ship.CheckDistance(game.Level.DestroyableBricks[i]))
                    {
                        if (game.Level.DestroyableBricks[i].GetType() == typeof(QuestionBlock))
                        {
                            QuestionBlock brick = (QuestionBlock)game.Level.DestroyableBricks[i];
                            if (!brick.Used)
                                graphics.DrawImage(game.Level.gifs[3], (int)game.Level.DestroyableBricks[i].Position.X,
                                                    (int)game.Level.DestroyableBricks[i].Position.Y, game.Level.DestroyableBricks[i].Width, game.Level.DestroyableBricks[i].Height);
                            else
                                graphics.DrawImage(brick.Bitmap, (int)game.Level.DestroyableBricks[i].Position.X,
                                                    (int)game.Level.DestroyableBricks[i].Position.Y, game.Level.DestroyableBricks[i].Width, game.Level.DestroyableBricks[i].Height);
                        }
                        else
                            graphics.DrawImage(game.Level.DestroyableBricks[i].Bitmap, (int)game.Level.DestroyableBricks[i].Position.X,
                                                    (int)game.Level.DestroyableBricks[i].Position.Y, game.Level.DestroyableBricks[i].Width, game.Level.DestroyableBricks[i].Height);
                    }
                }

                for (int i = 0; i < game.Level.Coins.Count; i++)
                {
                    try
                    {
                        if (game.Level.Ship.CheckDistance(game.Level.Coins[i]))
                        {
                            //AnimateImage(game.Level.gifs[2]);
                            //ImageAnimator.UpdateFrames();
                            graphics.DrawImage(game.Level.gifs[2], (int)game.Level.Coins[i].Position.X,
                                (int)game.Level.Coins[i].Position.Y, game.Level.Coins[i].Width, game.Level.Coins[i].Height);
                        }
                    }
                    catch
                    {

                    }
                }

                for (int i = 0; i < game.Level.Enemies.Count; i++)
                {
                    try
                    {
                        // Check to allow to render and calculate collisions
                        if (game.Level.Ship.CheckDistance(game.Level.Enemies[i]) || game.Level.Enemies[i].GetType() == typeof(BulletBill))
                        {
                            Enemy enemy = (Enemy)game.Level.Enemies[i];
                            if (enemy.GetType() == typeof(BulletBill))
                                graphics.DrawImage(enemy.FlipNPCImage(game.Level.bulletBillImage), (int)game.Level.Enemies[i].Position.X,
                                            (int)game.Level.Enemies[i].Position.Y, game.Level.Enemies[i].Width, game.Level.Enemies[i].Height);
                            if (enemy.GetType() == typeof(Goomba))
                            {
                                //AnimateImage(game.Level.gifs[0]);
                                graphics.DrawImage(enemy.FlipNPCImage(game.Level.gifs[0]), (int)game.Level.Enemies[i].Position.X,
                                            (int)game.Level.Enemies[i].Position.Y, game.Level.Enemies[i].Width, game.Level.Enemies[i].Height);
                            }
                            if (enemy.GetType() == typeof(KoopaGreen))
                            {
                                //AnimateImage(game.Level.gifs[1]);
                                graphics.DrawImage(enemy.FlipNPCImage(game.Level.gifs[1]), (int)game.Level.Enemies[i].Position.X,
                                            (int)game.Level.Enemies[i].Position.Y, game.Level.Enemies[i].Width, game.Level.Enemies[i].Height);
                            }
                        }
                    }
                    catch
                    {

                    }
                }

                //AnimateImage(game.Level.Ship.Bitmap);
                //ImageAnimator.UpdateFrames();
                // Draw Mario
                if (game.Level.Ship.IsMoving && game.Level.Ship.IsGrounded)
                    graphics.DrawImage(game.Level.Ship.FlipShipImage(game.Level.gifs[4], 1), (int)game.Level.Ship.Position.X,
                                        (int)game.Level.Ship.Position.Y, game.Level.Ship.Width, game.Level.Ship.Height);
                else
                    graphics.DrawImage(game.Level.Ship.FlipShipImage(game.Level.shipImage, 2), (int)game.Level.Ship.Position.X,
                                        (int)game.Level.Ship.Position.Y, game.Level.Ship.Width, game.Level.Ship.Height);
                if(thisLevel.foregroundImage != null)
                graphics.DrawImage(game.Level.foregroundImage, new Point((int)thisLevel.offsetX, (int)thisLevel.offsetY - 302));
            }
            else if (game != null && !game.Level.Ship.IsZoning)
            {
                graphics.Clear(Color.Black);
                if (game.Level.Ship.IsAlive == false)
                {
                    if (!checkedGameEnded)
                    {
                        checkedGameEnded = true;
                        currentlyAnimating = false;
                    }

                    AnimateImage(game.Level.gameOver);
                    ImageAnimator.UpdateFrames();
                    //graphics.DrawRectangle(System.Drawing.Pens.Blue, (int)coin.Position.X, (int)coin.Position.Y, coin.Width, coin.Height);
                    graphics.DrawImage(game.Level.gameOver, 0, 0, 700, 300);
                    Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    graphics.FillRectangle(brush, 560, 260, 140, 40);
                    brush.Dispose();
                }
            }
            if (game.Level.Ship.IsZoning)
            {
                currentlyAnimating = false;
            }
            painting = false;
            graphics.Dispose();
            return buffer;
        }
    }
}
