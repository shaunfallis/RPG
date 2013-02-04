using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace RPG
{
    public partial class GameDisplay : Form
    {

        private Stopwatch _timer = new Stopwatch();
        private double _lastTime;
        private long _frameCounter;
        private State _gameState;


        public GameDisplay()
        {
            //Setup the form
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            //Startup the game state
            _gameState = new State(ClientSize);

            initialize();
        }


        private void initialize()
        {
            _gameState.Initialize();

            //Initialise and start the timer
            _lastTime = 0.0;
            _timer.Reset();
            _timer.Start();

        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            //Work out how long since we were last here in seconds
            double gameTime = _timer.ElapsedMilliseconds / 1000.0;
            double elapsedTime = gameTime - _lastTime;
            _lastTime = gameTime;
            _frameCounter++;


            //Perform any animation and updates
            _gameState.Update(gameTime, elapsedTime);


            //Draw everything
            _gameState.Draw(e.Graphics);


            //Force the next Paint()
            this.Invalidate();

        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            _gameState.KeyDown(e.KeyCode);
        }
        
    }
}
