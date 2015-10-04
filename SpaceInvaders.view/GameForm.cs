using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//..
using System.Threading;
using SpaceInvaders.logic;
using SpaceInvaders.logic.Domain;
using SpaceInvaders.service;

namespace SpaceInvaders.view
{
    public partial class GameForm : Form
    {
        private KeyMessageFilter m_filter = new KeyMessageFilter();

        public GameForm()
        {
            Application.AddMessageFilter(m_filter);

            InitializeComponent();
            // Enable double buffering (stop flickering)
            // Can also be set under form properties
            SetStyle(ControlStyles.AllPaintingInWmPaint
             | ControlStyles.UserPaint
             | ControlStyles.DoubleBuffer, true);
            //Application.Idle += HandleApplicationIdle;
        }

        #region Loop
        //bool IsApplicationIdle()
        //{
        //    NativeMessage result;
        //    return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        //}

        //void HandleApplicationIdle(object sender, EventArgs e)
        //{
        //    while (IsApplicationIdle())
        //    {
        //        UpdateGame();
        //        Render();
        //    }
        //}

        //void UpdateGame()
        //{
        //    //GameLoop();
        //}

        //void Render()
        //{
        //    //Game_Paint();
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //public struct NativeMessage
        //{
        //    public IntPtr Handle;
        //    public uint Message;
        //    public IntPtr WParameter;
        //    public IntPtr LParameter;
        //    public uint Time;
        //    public Point Location;
        //}

        //[DllImport("user32.dll")]
        //public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
        #endregion

        //private Bitmap buffer;
        //private Bitmap newBuffer;
        Graphics graphics;
        Graphics bufferGrph;
        Graphics g;
        Thread thread;
        Thread keysThread;
        GameService game;
        DateTime time;

        const int left = 100;
        const int right = 101;
        //int direction = 100;
        bool currentlyAnimating = false;
        bool painting = false;
        bool updatingPhysics = false;

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Text = "Reset";
        Start:
            if (thread == null)
            {
                btnStart.Hide();
                // Window size
                //newBuffer = new Bitmap(800, 400);
                //buffer = new Bitmap(800, 400);
                g = this.CreateGraphics();
                time = new DateTime();
                game = new GameService();
                thread = new Thread(new ThreadStart(GameLoop));
                thread.Start();
                keysThread = new Thread(new ThreadStart(KeyLoop));
                keysThread.Start();
            }
            else
            {
                game.Level.Ship.IsAlive = false;
                time = DateTime.Now;
                while (time.AddSeconds(2) > DateTime.Now)
                {
                    // Needed to let the game end
                }
                game.Level.Dispose();
                game.End();
                game = null;
                //game.Level.Ship.Direction = left;
                thread = null;
                m_filter = null;
                keysThread = null;
                currentlyAnimating = false;
                m_filter = new KeyMessageFilter();
                Application.AddMessageFilter(m_filter);
                goto Start;
            }
        }

        public void GameLoop()
        {
            bool newLevel = false;
            while (game.Level.Ship.IsAlive == true)
            {
                // Timer for logic (40FPS)
                if (time.AddMilliseconds(25) < DateTime.Now)
                {
                    updatingPhysics = true;
                    game.PhysicsUpdate();
                    time = DateTime.Now;
                    updatingPhysics = false;
                    if (game.Level.Ship.IsZoning == true)
                    {
                        Thread.Sleep(1000);
                        newLevel = game.UpdateLevel();
                    }
                }

                if (painting == false)
                {
                    //PaintGame();
                    //PaintMe();
                }

                // Calls GameForm_Paint
                Invalidate();

                //Thread.Sleep(10);
            }
            if (game.Level.Ship.IsAlive == false)
            {
                ImageAnimator.Animate(game.Level.Coins[0].Bitmap, null);
                ImageAnimator.Animate(game.Level.gameOver, new EventHandler(this.OnFrameChanged));
                Invalidate();
            }
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
            this.Invalidate();
        }

        private void PaintGame()
        {
            //painting = true;
            //graphics = Graphics.FromImage(buffer);
            //// Rendering graphics from here stops flickering (used in conjunction with double buffering)

            ////graphics.Clear(Color.BlanchedAlmond);
            
            //// Render graphics
            ////graphics.FillRectangle(new SolidBrush(Color.BlanchedAlmond), 0, 0, 1000, 1000);
            //if (game != null)
            //{
            //    graphics.DrawImage(game.Level.backgroundImage, new Point(0, -70));

            //    graphics.DrawImage(game.Level.backgroundImage, new Point(0, -70));
            //    foreach (Platform platform in game.Level.Platforms)
            //    {
            //        //if (game.Ship.CheckDistance(platform) || platform.Width > 160)
            //        //graphics.DrawRectangle(System.Drawing.Pens.Blue, (int)platform.Position.X, (int)platform.Position.Y, platform.Width, platform.Height);
            //        graphics.DrawImage(platform.Bitmap, (int)platform.Position.X,
            //            (int)platform.Position.Y, platform.Width, platform.Height);
            //    }

            //    //for (int i = 0; i < game.Coins.Count; i++)
            //    //{
            //    //    if (game.Ship.CheckDistance(game.Coins[i]))
            //    //    {
            //    //        AnimateImage(game.Coins[i].Bitmap);
            //    //        ImageAnimator.UpdateFrames();
            //    //        //graphics.DrawRectangle(System.Drawing.Pens.Blue, (int)coin.Position.X, (int)coin.Position.Y, coin.Width, coin.Height);
            //    //        graphics.DrawImage(game.Coins[i].Bitmap, (int)game.Coins[i].Position.X,
            //    //            (int)game.Coins[i].Position.Y, game.Coins[i].Width, game.Coins[i].Height);
            //    //    }
            //    //}

            //    foreach (Coin coin in game.Level.Coins)
            //    {
            //        if (game.Level.Ship.CheckDistance(coin))
            //        {
            //            AnimateImage(game.Level.coinImage);
            //            ImageAnimator.UpdateFrames();
            //            //graphics.DrawRectangle(System.Drawing.Pens.Blue, (int)coin.Position.X, (int)coin.Position.Y, coin.Width, coin.Height);
            //            graphics.DrawImage(coin.Bitmap, (int)coin.Position.X,
            //                (int)coin.Position.Y, coin.Width, coin.Height);
            //        }
            //    }

            //    // Draw NPCs
            //    foreach (Enemy enemy in game.Level.Enemies)
            //    {
            //        // Check to allow to render and calculate collisions
            //        if (game.Level.Ship.CheckDistance(enemy) || enemy.GetType() == typeof(BulletBill))
            //        {
            //            graphics.DrawImage(enemy.FlipNPCImage(), (int)enemy.Position.X,
            //                                (int)enemy.Position.Y, enemy.Width, enemy.Height);
            //        }
            //    }
            //    // Draw Mario
            //    graphics.DrawImage(game.Level.Ship.Bitmap, (int)game.Level.Ship.Position.X,
            //            (int)game.Level.Ship.Position.Y, game.Level.Ship.Width, game.Level.Ship.Height);
            //}

            ////if (buffer != null)
            ////{
            ////    bufferGrph = Graphics.FromImage(newBuffer);
            ////    bufferGrph.DrawImageUnscaled(buffer, Point.Empty);
            ////}
            ////buffer = newBuffer;
            ////painting = false;
        }

        private void PaintMe()
        {
            //g.DrawImageUnscaled(buffer, Point.Empty);

            //if (game.Level.Ship.Flipped)
            //{
            //    game.Level.Ship.Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            //    game.Level.Ship.Flipped = false;
            //}

            //painting = false;
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            //painting = true;
            //if (game != null)
            //    e.Graphics.DrawImageUnscaled(buffer, Point.Empty);
            // Rendering graphics from here stops flickering (used in conjunction with double buffering)
            graphics = e.Graphics;

            //graphics.Clear(Color.BlanchedAlmond);

            // Render graphics
            if (game != null && !game.Level.Ship.IsZoning && game.Level.Ship.IsAlive)
            {
                game.Level.Ship.FlipShipImage();
                graphics.DrawImage(game.Level.backgroundImage, new Point(0, 0));

                foreach (WarpPipe pipe in game.Level.WarpPipes)
                {
                    graphics.DrawImage(pipe.Bitmap, (int)pipe.Position.X,
                    (int)pipe.Position.Y, pipe.Width, pipe.Height);
                }

                foreach (Platform platform in game.Level.Platforms)
                {
                    graphics.DrawImage(platform.Bitmap, (int)platform.Position.X,
                    (int)platform.Position.Y, platform.Width, platform.Height);
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
                        //graphics.DrawRectangle(System.Drawing.Pens.Blue, (int)coin.Position.X, (int)coin.Position.Y, coin.Width, coin.Height);
                        graphics.DrawImage(game.Level.coinImage, (int)game.Level.Coins[i].Position.X,
                            (int)game.Level.Coins[i].Position.Y, game.Level.Coins[i].Width, game.Level.Coins[i].Height);
                    }
                }

                //foreach (Coin coin in game.Coins)
                //{
                //    if (game.Ship.CheckDistance(coin))
                //    {
                //        AnimateImage(coin.Bitmap);
                //        ImageAnimator.UpdateFrames();
                //        //graphics.DrawRectangle(System.Drawing.Pens.Blue, (int)coin.Position.X, (int)coin.Position.Y, coin.Width, coin.Height);
                //        graphics.DrawImage(coin.Bitmap, (int)coin.Position.X,
                //            (int)coin.Position.Y, coin.Width, coin.Height);
                //    }
                //}

                for (int i = 0; i < game.Level.Enemies.Count; i++)
                {
                    // Check to allow to render and calculate collisions
                    if (game.Level.Ship.CheckDistance(game.Level.Enemies[i]) || game.Level.Enemies[i].GetType() == typeof(BulletBill))
                    {
                        Enemy enemy = (Enemy)game.Level.Enemies[i];
                        graphics.DrawImage(enemy.FlipNPCImage(), (int)enemy.Position.X,
                                            (int)enemy.Position.Y, enemy.Width, enemy.Height);
                    }
                }
                // Draw NPCs
                //foreach (Enemy enemy in game.Enemies)
                //{
                //    // Check to allow to render and calculate collisions
                //    if (game.Ship.CheckDistance(enemy) || enemy.GetType() == typeof(BulletBill))
                //    {
                //        graphics.DrawImage(FlipNPCImage(enemy.Bitmap, (Enemy)enemy), (int)enemy.Position.X,
                //                            (int)enemy.Position.Y, enemy.Width, enemy.Height);
                //    }
                //}
                // Draw Mario
                graphics.DrawImage(game.Level.Ship.Bitmap, (int)game.Level.Ship.Position.X,
                        (int)game.Level.Ship.Position.Y, game.Level.Ship.Width, game.Level.Ship.Height);
            }
            else if (game != null)
            {
                graphics.Clear(Color.Black);
                if (game.Level.Ship.IsAlive == false)
                {
                    btnStart.Show();
                    AnimateImage(game.Level.gameOver);
                    ImageAnimator.UpdateFrames();
                    //graphics.DrawRectangle(System.Drawing.Pens.Blue, (int)coin.Position.X, (int)coin.Position.Y, coin.Width, coin.Height);
                    graphics.DrawImage(game.Level.gameOver, 0, 0, 700, 300);
                    Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    graphics.FillRectangle(brush, 560, 260, 140, 40);
                    brush.Dispose();
                }
            }
            //painting = false;
        }

        // Keypresses for moving
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (game != null)
            {
                if (keyData == Keys.Left || keyData == Keys.A)
                {
                    // Image faces left initially
                    if (game.Level.Ship.Direction != left)
                    {
                        game.Level.Ship.Flipped = true;
                        game.Level.Ship.Direction = left;
                    }
                    return true; //for the active control to see the keypress, return false
                }
                if (keyData == Keys.Right || keyData == Keys.D)
                {
                    if (game.Level.Ship.Direction != right)
                    {
                        game.Level.Ship.Flipped = true;
                        game.Level.Ship.Direction = right;
                    }
                    return true; //for the active control to see the keypress, return false
                }
                if (keyData == Keys.Space)
                {
                    return true;
                }
                if (keyData == Keys.S || keyData == Keys.Down)
                {
                    game.Level.Ship.IsDucking = true;
                    return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void KeyLoop()
        {
            DateTime timer = DateTime.Now;
            while (game.Level.Ship.IsAlive)
            {
                if (timer.AddMilliseconds(10) < DateTime.Now)
                {
                    if (m_filter.IsKeyPressed(Keys.Up) || m_filter.IsKeyPressed(Keys.W) || m_filter.IsKeyPressed(Keys.Space))
                    {
                        if (game.Level.Ship.IsGrounded == true && game.Level.Ship.IsJumping == false)
                            game.Level.Ship.Velocity.Y -= 6.3;
                        else if (game.Level.Ship.Velocity.Y < -1)
                            game.Level.Ship.Velocity.Y -= 0.04;
                        game.Level.Ship.IsJumping = true;
                    }
                    else
                    {
                        game.Level.Ship.IsJumping = false;
                    }
                    if ((m_filter.IsKeyPressed(Keys.Right) || m_filter.IsKeyPressed(Keys.D)) && game.Level.Ship.IsGrounded)
                        game.Level.Ship.Velocity.X += 0.2;
                    else if (m_filter.IsKeyPressed(Keys.Right) || m_filter.IsKeyPressed(Keys.D))
                        game.Level.Ship.Velocity.X += 0.1;
                    if ((m_filter.IsKeyPressed(Keys.Left) || m_filter.IsKeyPressed(Keys.A)) && game.Level.Ship.IsGrounded)
                        game.Level.Ship.Velocity.X -= 0.2;
                    if (m_filter.IsKeyPressed(Keys.Left) || m_filter.IsKeyPressed(Keys.A))
                        game.Level.Ship.Velocity.X -= 0.1;
                    if (m_filter.IsKeyPressed(Keys.S) || m_filter.IsKeyPressed(Keys.Down))
                        game.Level.Ship.IsDucking = true;
                    else
                        game.Level.Ship.IsDucking = false;
                    timer = DateTime.Now;
                }
            }
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // End threads on closing
            if (thread != null && thread.IsAlive == true)
                thread.Abort();
            if (keysThread != null && keysThread.IsAlive == true)
                keysThread.Abort();
            if (game != null)
                game.End();
        }
    }

    public class KeyMessageFilter : IMessageFilter
    {
        private Dictionary<Keys, bool> m_keyTable = new Dictionary<Keys, bool>();

        public Dictionary<Keys, bool> KeyTable
        {
            get { return m_keyTable; }
            private set { m_keyTable = value; }
        }

        public bool IsKeyPressed()
        {
            return m_keyPressed;
        }

        public bool IsKeyPressed(Keys k)
        {
            bool pressed = false;

            if (KeyTable.TryGetValue(k, out pressed))
            {
                return pressed;
            }

            return false;
        }

        private const int WM_KEYDOWN = 0x0100;

        private const int WM_KEYUP = 0x0101;

        private bool m_keyPressed = false;


        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN)
            {
                KeyTable[(Keys)m.WParam] = true;

                m_keyPressed = true;
            }

            if (m.Msg == WM_KEYUP)
            {
                KeyTable[(Keys)m.WParam] = false;

                m_keyPressed = false;
            }

            return false;
        }
    }
}
