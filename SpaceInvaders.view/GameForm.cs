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

        Thread thread;
        Thread keysThread;
        GameService game;
        DateTime time;
        DrawBuffer drawBuffer;

        const int left = 100;
        const int right = 101;

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Text = "Reset";
        Start:
            if (thread == null)
            {
                btnStart.Hide();
                
                time = new DateTime();
                game = new GameService();
                drawBuffer = new DrawBuffer(game.Level);
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
                game = null;
                //game.Level.Ship.Direction = left;
                thread = null;
                m_filter = null;
                keysThread = null;
                m_filter = new KeyMessageFilter();
                Application.AddMessageFilter(m_filter);
                goto Start;
            }
        }

        public void GameLoop()
        {
            Invalidate();
            bool newLevel = false;
            while (game.Level.Ship.IsAlive == true)
            {
                // Timer for logic (40FPS)
                Thread.Sleep(25);
                game.PhysicsUpdate();
                Invalidate();
                if (game.Level.Ship.IsZoning == true)
                {
                    Thread.Sleep(2000);
                    newLevel = game.UpdateLevel();
                    drawBuffer = new DrawBuffer(game.Level);
                }
                // Calls GameForm_Paint
                //Invalidate();
                //foreach (Enemy enemy in game.Level.Enemies)
                //{
                //    Invalidate(new Rectangle(new Point((int)enemy.Position.X - enemy.Width, (int)enemy.Position.Y), new Size(enemy.Width * 3, enemy.Height)));
                //}
                //foreach (Coin enemy in game.Level.Coins)
                //{
                //    Invalidate(new Rectangle(new Point((int)enemy.Position.X - enemy.Width, (int)enemy.Position.Y), new Size(enemy.Width * 3, enemy.Height)));
                //}
            }

            // GameOver Loop
            //DateTime timer = DateTime.Now;
            while (game != null && drawBuffer != null)
            {
                Thread.Sleep(25);
                    Invalidate();
            }
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            if (game != null && drawBuffer != null)
            {
                if (drawBuffer.painting == false)
                {
                    if (game.Level.Ship.IsAlive == false && !btnStart.Visible)
                        btnStart.Show();
                    Benchmark.Start();
                    e.Graphics.DrawImageUnscaled(drawBuffer.Draw(game), Point.Empty);
                    Benchmark.End();
                    Console.WriteLine("Time to Draw Scene: " + Benchmark.GetSeconds());
                    // Rendering graphics from here stops flickering (used in conjunction with double buffering)
                }
            }
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
                        game.Level.Ship.Direction = left;
                    }
                    return true; //for the active control to see the keypress, return false
                }
                if (keyData == Keys.Right || keyData == Keys.D)
                {
                    if (game.Level.Ship.Direction != right)
                    {
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
            while (game.Level.Ship.IsAlive)
            {
                Thread.Sleep(10);
                    if (game.Level.Ship.Velocity.X < 0.5 || game.Level.Ship.Velocity.X > -0.5)
                    {
                        game.Level.Ship.IsMoving = false;
                        game.Level.Ship.IsDucking = false;
                    }
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
                    if ((m_filter.IsKeyPressed(Keys.Right) || m_filter.IsKeyPressed(Keys.D)))
                    {
                        if (game.Level.Ship.IsGrounded)
                            game.Level.Ship.Velocity.X += 0.2;
                        else
                            game.Level.Ship.Velocity.X += 0.1;
                        game.Level.Ship.IsMoving = true;
                    }
                    if ((m_filter.IsKeyPressed(Keys.Left) || m_filter.IsKeyPressed(Keys.A)))
                    {
                        if(game.Level.Ship.IsGrounded)
                            game.Level.Ship.Velocity.X -= 0.2;
                        else
                            game.Level.Ship.Velocity.X -= 0.1;
                        game.Level.Ship.IsMoving = true;

                    }
                    if (m_filter.IsKeyPressed(Keys.S) || m_filter.IsKeyPressed(Keys.Down))
                    {
                        game.Level.Ship.IsDucking = true;
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
                game.Dispose();
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
