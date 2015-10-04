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
    public class DrawBuffer
    {
        public Bitmap buffer;
        Graphics graphics;
        bool currentlyAnimating;
        bool checkedGameEnded = false;

        public DrawBuffer()
        {
            buffer = new Bitmap(800, 400);
            currentlyAnimating = false;
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
            graphics = Graphics.FromImage(buffer);

            // Render graphics
            if (game != null && !game.Level.Ship.IsZoning && game.Level.Ship.IsAlive)
            {
                game.Level.Ship.FlipShipImage();
                graphics.DrawImage(game.Level.backgroundImage, new Point(0, 0));

                for (int i = 0; i < game.Level.WarpPipes.Count; i++)
                {
                    graphics.DrawImage(game.Level.WarpPipes[i].Bitmap, (int)game.Level.WarpPipes[i].Position.X,
                    (int)game.Level.WarpPipes[i].Position.Y, game.Level.WarpPipes[i].Width, game.Level.WarpPipes[i].Height);
                }

                for (int i = 0; i < game.Level.Platforms.Count; i++)
                {
                    graphics.DrawImage(game.Level.Platforms[i].Bitmap, (int)game.Level.Platforms[i].Position.X,
                    (int)game.Level.Platforms[i].Position.Y, game.Level.Platforms[i].Width, game.Level.Platforms[i].Height);
                }

                for (int i = 0; i < game.Level.DestroyableBricks.Count; i++)
                {
                    // Check to allow to render and calculate collisions
                    if (game.Level.Ship.CheckDistance(game.Level.DestroyableBricks[i]))
                    {
                        graphics.DrawImage(game.Level.DestroyableBricks[i].Bitmap, (int)game.Level.DestroyableBricks[i].Position.X,
                                            (int)game.Level.DestroyableBricks[i].Position.Y, game.Level.DestroyableBricks[i].Width, game.Level.DestroyableBricks[i].Height);
                    }
                }

                for (int i = 0; i < game.Level.Coins.Count; i++)
                {
                    if (game.Level.Ship.CheckDistance(game.Level.Coins[i]))
                    {
                        AnimateImage(game.Level.coinImage);
                        ImageAnimator.UpdateFrames();
                        graphics.DrawImage(game.Level.coinImage, (int)game.Level.Coins[i].Position.X,
                            (int)game.Level.Coins[i].Position.Y, game.Level.Coins[i].Width, game.Level.Coins[i].Height);
                    }
                }

                for (int i = 0; i < game.Level.Enemies.Count; i++)
                {
                    // Check to allow to render and calculate collisions
                    if (game.Level.Ship.CheckDistance(game.Level.Enemies[i]) || CastTypeHelper.CheckType(game.Level.Enemies[i]) == "BulletBill")
                    {
                        graphics.DrawImage(CastTypeHelper.EnemyNPCFlip(game.Level.Enemies[i]), (int)game.Level.Enemies[i].Position.X,
                                            (int)game.Level.Enemies[i].Position.Y, game.Level.Enemies[i].Width, game.Level.Enemies[i].Height);
                    }
                }
                // Draw Mario
                graphics.DrawImage(game.Level.Ship.Bitmap, (int)game.Level.Ship.Position.X,
                        (int)game.Level.Ship.Position.Y, game.Level.Ship.Width, game.Level.Ship.Height);
            }
            else if (game != null)
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
            //painting = false;
            graphics.Dispose();
            return buffer;
        }
    }
}
