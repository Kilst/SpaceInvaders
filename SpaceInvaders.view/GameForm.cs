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
using SpaceInvaders.service;
using System.Timers;

// (DateTime.Now is REALLY EXPENSIVE)
// (DateTime.Now is REALLY EXPENSIVE)
// (DateTime.Now is REALLY EXPENSIVE)
// (DateTime.Now is REALLY EXPENSIVE)
// (DateTime.Now is REALLY EXPENSIVE)

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

        GameService game;
        DateTime time;
        DrawBuffer drawBuffer;
        Graphics g;
        System.Timers.Timer gameTimer;
        System.Timers.Timer keypressTimer;
        private bool newLevel = false;

        private const int left = 100;
        private const int right = 101;
        private const int up = 102;
        private const int down = 103;

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Text = "Reset";
        Start:
            if (gameTimer == null)
            {
                btnStart.Hide();
                gameTimer = null;
                keypressTimer = null;
                time = new DateTime();
                g = this.CreateGraphics();
                game = new GameService();
                drawBuffer = new DrawBuffer(game.Level);
                Thread.Sleep(1500);
                gameTimer = new System.Timers.Timer(25);
                gameTimer.Elapsed += new ElapsedEventHandler(GameTimedEvent);
                gameTimer.Enabled = true;
                keypressTimer = new System.Timers.Timer(1);
                keypressTimer.Elapsed += new ElapsedEventHandler(KeypressTimedEvent);
                keypressTimer.Enabled = true;
                //thread = new Thread(new ThreadStart(GameLoop));
                //thread.Start();
                //keysThread = new Thread(new ThreadStart(KeyLoop));
                //keysThread.Start();
            }
            else
            {
                gameTimer.Enabled = false;
                gameTimer.Elapsed -= new ElapsedEventHandler(GameTimedEvent);
                gameTimer = null;
                keypressTimer.Enabled = false;
                keypressTimer.Elapsed -= new ElapsedEventHandler(KeypressTimedEvent);
                game.SetPlayerAlive(false);
                time = DateTime.Now;
                while (time.AddSeconds(2) > DateTime.Now)
                {
                    // Needed to let the game end
                }
                game.Level.Dispose();
                game = null;
                //game.Level.Ship.Direction = left;
                m_filter = null;
                m_filter = new KeyMessageFilter();
                Application.AddMessageFilter(m_filter);
                goto Start;
            }
        }

        // Timer
        public void GameTimedEvent(object sender, ElapsedEventArgs e)
        {
            game.UpdateLevel();
            if (game.Zoning == true)
            {
                // Needed otherwise timer ticks and calls g.DrawUnscaledImage
                // and we get an object in use due to the new drawBuffer instantation.
                drawBuffer = new DrawBuffer(game.Level);
                Thread.Sleep(1500);
                game.Zoning = false;
            }
            if (game.IsPlayerAlive() == true && game.Zoning != true)
                game.PhysicsUpdate();

            if (drawBuffer.painting == false && game.Zoning != true)
            {
                Benchmark.Start();
                g.DrawImageUnscaled(drawBuffer.Draw(game), Point.Empty);
                Benchmark.End();
                Console.WriteLine("Total Time to Draw Scene: {0}", Benchmark.GetSeconds());
                drawBuffer.painting = false;
            }
        }

        // Thread (DateTime.Now is REALLY EXPENSIVE)
        public void GameLoop()
        {
            bool newLevel = false;
            while (game != null && drawBuffer != null)
            {
                // Timer for logic (40FPS)
                if (time.AddMilliseconds(25) < DateTime.Now)
                {
                    if (game.IsPlayerAlive() == true)
                    {
                        Benchmark.Start();
                        game.PhysicsUpdate();
                        Benchmark.End();
                        Console.WriteLine("Total Time to Complete: " + Benchmark.GetSeconds());
                    }
                    time = DateTime.Now;
                    if (game.Zoning == true)
                    {
                        Thread.Sleep(2000);
                        newLevel = game.UpdateLevel();
                        drawBuffer = new DrawBuffer(game.Level);
                    }

                    // Calls GameForm_Paint
                    Invalidate();
                    Thread.Sleep(10);
                }
            }
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {

            if (game != null && drawBuffer != null)
            {
                //Benchmark.Start();
                if (drawBuffer.painting == false)
                {
                    if (game.IsPlayerAlive() && !btnStart.Visible)
                        btnStart.Show();
                    //e.Graphics.DrawImageUnscaled(drawBuffer.Draw(game), Point.Empty);
                    //drawBuffer.painting = false;
                    // Rendering graphics from here stops flickering (used in conjunction with double buffering)
                }
                //Benchmark.End();
                //Console.WriteLine("Total Time to Complete: " + Benchmark.GetSeconds());
            }

        }

        // Keypresses for moving
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (game != null)
            {
                if (keyData == Keys.Left || keyData == Keys.A)
                {
                    return true; //for the active control to see the keypress, return false
                }
                if (keyData == Keys.Right || keyData == Keys.D)
                {
                    return true; //for the active control to see the keypress, return false
                }
                if (keyData == Keys.Space)
                {
                    return true;
                }
                if (keyData == Keys.S || keyData == Keys.Down)
                {
                    return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        // Timer
        public void KeypressTimedEvent(object sender, ElapsedEventArgs e)
        {
            game.MovingCheck();

            if (m_filter.IsKeyPressed(Keys.Up) || m_filter.IsKeyPressed(Keys.W) || m_filter.IsKeyPressed(Keys.Space))
                game.AddPlayerVelocity(up);
            if ((m_filter.IsKeyPressed(Keys.Right) || m_filter.IsKeyPressed(Keys.D)))
                game.AddPlayerVelocity(right);
            if ((m_filter.IsKeyPressed(Keys.Left) || m_filter.IsKeyPressed(Keys.A)))
                game.AddPlayerVelocity(left);
            if (m_filter.IsKeyPressed(Keys.S) || m_filter.IsKeyPressed(Keys.Down))
                game.SetPlayerDucking(true);
            else
                game.SetPlayerDucking(false);
        }

        // Thread (DateTime.Now is REALLY EXPENSIVE)
        private void KeyLoop()
        {
            DateTime timer = DateTime.Now;
            while (game.IsPlayerAlive())
            {
                if (timer.AddMilliseconds(10) < DateTime.Now)
                {
                    game.MovingCheck();

                    if (m_filter.IsKeyPressed(Keys.Up) || m_filter.IsKeyPressed(Keys.W) || m_filter.IsKeyPressed(Keys.Space))
                    {
                        game.AddPlayerVelocity(up);
                    }
                    if ((m_filter.IsKeyPressed(Keys.Right) || m_filter.IsKeyPressed(Keys.D)))
                    {
                        game.AddPlayerVelocity(right);
                    }
                    if ((m_filter.IsKeyPressed(Keys.Left) || m_filter.IsKeyPressed(Keys.A)))
                    {
                        game.AddPlayerVelocity(left);

                    }
                    if (m_filter.IsKeyPressed(Keys.S) || m_filter.IsKeyPressed(Keys.Down))
                    {
                        game.SetPlayerDucking(true);
                    }

                    timer = DateTime.Now;
                }
            }
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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