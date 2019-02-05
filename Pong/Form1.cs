/*
 * Description:     A basic PONG simulator
 * Author:      Malcolm Wright     
 * Date:            Feb 5th, 2018
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush drawBrush = new SolidBrush(Color.White);
        Font drawFont = new Font("Courier New", 30);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        const int BALL_SPEED = 4;
        Rectangle ball;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 4;
        Rectangle p1, p2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                titleLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            p1.Width = p2.Width = 10;    //height for both paddles set the same
            p1.Height = p2.Height = 40;  //width for both paddles set the same

            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            // set Width and Height of ball
            ball.Width = ball.Height = 10;

            // set starting X position for ball to middle of screen, (use this.Width and ball.Width)
            ball.X = this.Width / 2 - ball.Width / 2;

            // set starting Y position for ball to middle of screen, (use this.Height and ball.Height)
            ball.Y = this.Height / 2 - ball.Height / 2;
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            // TODO create code to move ball either left or right based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight)
            {
                ball.X += BALL_SPEED;
            }
            else
            {
                ball.X -= BALL_SPEED;
            }

            // TODO create code move ball either down or up based on ballMoveDown and using BALL_SPEED
            if (ballMoveDown)
            {
                ball.Y += BALL_SPEED;
            }
            else
            {
                ball.Y -= BALL_SPEED;
            }

            #endregion

            #region update paddle positions

            // move paddles up if it isn't at the top
            if (aKeyDown == true && p1.Y > 0)
            { 
                p1.Y -= PADDLE_SPEED;
            }
            if (jKeyDown == true && p2.Y > 0)
            {
                p2.Y -= PADDLE_SPEED;
            }

            // move paddles down if it isn't at the bottom
            if (zKeyDown == true && p1.Y < this.Height - p1.Height)
            {
                p1.Y += PADDLE_SPEED;
            }
            if (mKeyDown == true && p2.Y < this.Height - p2.Height)
            {
                p2.Y += PADDLE_SPEED;
            }
            #endregion

            #region ball collision with top and bottom lines

            // check collision at top or bottom and play sound
            if (ball.Y + ball.Height >= this.Height || ball.Y < 0)
            {
                ballMoveDown = !ballMoveDown;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            if (p1.IntersectsWith(ball) || p2.IntersectsWith(ball))
            {
                ballMoveRight = !ballMoveRight;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                // play score sound
                scoreSound.Play();

                // update player 2 score
                player2Score++;

                // use if statement to check to see if player 2 has won the game. If true run 
                // GameOver method. Else change direction of ball and call SetParameters method.
                if (player2Score == gameWinScore)
                {
                    // player 2 has won
                    GameOver("Player 2");
                }
                else
                {
                    SetParameters();
                    ballMoveRight = false;
                }
            }
            if (ball.X >= this.Width - ball.Width)
            {
                // play score sound
                scoreSound.Play();

                // update player 2 score
                player1Score++;

                // use if statement to check to see if player 1 has won the game. If true run 
                // GameOver method. Else change direction of ball and call SetParameters method.
                if (player1Score == gameWinScore)
                {
                    // player 1 has won
                    GameOver("Player 1");
                }
                else
                {
                    SetParameters();
                    ballMoveRight = true;
                }
            }

            #endregion
            
            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;

            // --- stop the gameUpdateLoop
            gameUpdateLoop.Stop();

            // --- show a message on the startLabel to indicate a winner, (need to Refresh).
            startLabel.Text = winner + " wins!";
            startLabel.Visible = true;
            this.Refresh();

            // --- pause for two seconds 
            Thread.Sleep(2000);

            // --- use the startLabel to ask the user if they want to play again
            startLabel.Text = "Do you want to play again?\n" +
                "Space to play again\nN to quit";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // draw paddles using FillRectangle
            e.Graphics.FillRectangle(drawBrush, p1);
            e.Graphics.FillRectangle(drawBrush, p2);

            // draw ball using FillRectangle
            e.Graphics.FillEllipse(drawBrush, ball);
            
            if ()
            // draw scores to the screen using DrawString
            e.Graphics.DrawString("" + player1Score, drawFont, drawBrush, 0, 0);
            e.Graphics.DrawString("" + player2Score, drawFont, drawBrush, this.Width - 40, 0);
        }

    }
}
