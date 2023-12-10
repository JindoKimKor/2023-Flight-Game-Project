using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    /// <summary>
    /// Save Scores Class
    /// </summary>
    public class FinishScene : GameScene
    {
        // Rendering components
        private SpriteBatch finishSceneSpriteBatch;

        // Fonts for text
        private SpriteFont titleFont;
        private SpriteFont regularFont;
        private SpriteFont hilightFont;

        // Textures for scene visuals
        private Texture2D backgroundTexture;
        private Texture2D transparentBackground;

        // Text and titles
        private string titleText = "Game End";

        // Game state and control
        private double delayCounter;
        private bool flag = true;
        private string userInitialToSave = "";
        private int finalScore;
        private KeyboardState state;
        private KeyboardState oldState = Keyboard.GetState();

        // Event
        public event Action FinishSceneCompleted;

        /// <summary>
        /// Finish scene constructor
        /// </summary>
        /// <param name="game"></param>
        public FinishScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            LoadRenderingComponents();
            LoadFonts();
            LoadTextures();

            void LoadRenderingComponents()
            {
                finishSceneSpriteBatch = mainGame.SpriteBatch;
            }
            void LoadFonts()
            {
                regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
                hilightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
                titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            }
            void LoadTextures()
            {
                backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
                transparentBackground = new Texture2D(GraphicsDevice, 1, 1);
                transparentBackground.SetData(new[] { Color.Black });
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateGameState(gameTime);
            HandleInput();

            base.Update(gameTime);

            void UpdateGameState(GameTime gameTime)
            {
                finalScore = PlayScene.NumberOfDestoryedSmallHelicopter - PlayScene.NumberOfGotHit;
                delayCounter += gameTime.ElapsedGameTime.TotalSeconds;
                if (delayCounter >= 1.0)
                    delayCounter -= 1.0;
                flag = delayCounter < 0.7;
            }

            void HandleInput()
            {
                state = Keyboard.GetState();
                ProcessUserInput();
                oldState = state;
            }

            void ProcessUserInput()
            {
                Keys[] userInitialKeys = state.GetPressedKeys();
                if (userInitialKeys.Length > 0 && !oldState.IsKeyDown(userInitialKeys[0]))
                {
                    if (userInitialKeys[0] >= Keys.A && userInitialKeys[0] <= Keys.Z && userInitialToSave.Length < 3)
                    {
                        userInitialToSave += userInitialKeys[0].ToString();
                    }
                    else if (userInitialKeys[0] == Keys.Back && userInitialToSave.Length > 0)
                    {
                        userInitialToSave = userInitialToSave.Substring(0, userInitialToSave.Length - 1);
                    }
                    else if (userInitialKeys[0] == Keys.Enter && userInitialToSave.Length > 0)
                    {
                        SaveScore();
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            finishSceneSpriteBatch.Begin();

            DrawBackground();
            DrawTitle();
            DrawUserInput();
            DrawGameStatistics();

            finishSceneSpriteBatch.End();
            base.Draw(gameTime);

            void DrawBackground()
            {
                finishSceneSpriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
                finishSceneSpriteBatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);
            }

            void DrawTitle()
            {
                Vector2 titleSize = titleFont.MeasureString(titleText);
                Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);
                finishSceneSpriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);
            }

            void DrawUserInput()
            {
                string initialsText = "Please Enter Your Initials";
                Vector2 messageSize = regularFont.MeasureString(initialsText);
                Vector2 messagePosition = new Vector2((Shared.stageSize.X - messageSize.X) / 2, 280);
                Vector2 initialsSize = hilightFont.MeasureString(userInitialToSave);
                Vector2 initialsPosition = new Vector2((Shared.stageSize.X - initialsSize.X) / 2, 320);

                if (flag)
                {
                    finishSceneSpriteBatch.DrawString(regularFont, initialsText, messagePosition, Color.White);
                    finishSceneSpriteBatch.DrawString(hilightFont, userInitialToSave, initialsPosition, Color.White);
                }
            }

            void DrawGameStatistics()
            {
                string timeText = "Time: " + PlayScene.TimeString;
                string enemyKillText = "Enemy Kill: " + PlayScene.NumberOfDestoryedSmallHelicopter.ToString();
                string hitText = "Hitten: " + PlayScene.NumberOfGotHit.ToString();
                string totalScoreText = "Total Score: " + finalScore.ToString();

                // Positions for game statistics
                Vector2 timePosition = new Vector2(150, 450);
                Vector2 enemyKillPosition = new Vector2(150, 550);
                Vector2 hitPosition = new Vector2(150, 650);
                Vector2 totalScorePosition = new Vector2(150, 750);

                // Draw game statistics
                finishSceneSpriteBatch.DrawString(regularFont, timeText, timePosition, Color.PaleVioletRed);
                finishSceneSpriteBatch.DrawString(regularFont, enemyKillText, enemyKillPosition, Color.PaleVioletRed);
                finishSceneSpriteBatch.DrawString(regularFont, hitText, hitPosition, Color.PaleVioletRed);
                finishSceneSpriteBatch.DrawString(regularFont, totalScoreText, totalScorePosition, Color.PaleVioletRed);
            }
        }

        private void SaveScore()
        {
            string filePath = "scores.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(userInitialToSave);
                writer.WriteLine(finalScore);
            }
            FinishSceneCompleted?.Invoke();
        }
    }
}
