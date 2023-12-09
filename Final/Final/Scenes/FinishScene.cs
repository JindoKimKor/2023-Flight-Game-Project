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
    public class FinishScene : GameScene
    {
        private SpriteBatch finishSceneSpriteBatch;
        private SpriteFont titleFont;
        private Texture2D backgroundTexture;
        private string titleText = "Game End";
        SpriteFont regularFont;
        SpriteFont hilightFont;
        private Texture2D transparentBackground;

        private double delayCounter;
        private bool flag = true;

        private string initials = "";
        private int finalScore;
        KeyboardState state;
        KeyboardState oldState = Keyboard.GetState();

        public event Action FinishSceneCompleted;

        public FinishScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            finishSceneSpriteBatch = mainGame._spriteBatch;
            regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
            hilightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            transparentBackground = new Texture2D(GraphicsDevice, 1, 1);
            transparentBackground.SetData(new[] { Color.Black });
            finalScore = PlayScene.NumberOfDestoryedSmallHelicopter - PlayScene.NumberOfGotHit;
        }
        public override void Update(GameTime gameTime)
        {
            delayCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (delayCounter >= 1.0)
            {
                delayCounter -= 1.0;
            }
            flag = delayCounter < 0.7;

            state = Keyboard.GetState();
            Keys[] keys = state.GetPressedKeys();
            if (keys.Length > 0 && !oldState.IsKeyDown(keys[0]))
            {
                if (keys[0] >= Keys.A && keys[0] <= Keys.Z && initials.Length < 3)
                {
                    initials += keys[0].ToString();
                }
                else if (keys[0] == Keys.Back && initials.Length > 0)
                {
                    initials = initials.Substring(0, initials.Length - 1);
                }
                else if (keys[0] == Keys.Enter && initials.Length > 0)
                {
                    SaveScore();
                }
            }
            oldState = state;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            finishSceneSpriteBatch.Begin();

            Vector2 titleSize = titleFont.MeasureString(titleText);
            string initialsText = "Please Enter Your Initials";
            Vector2 messageSize = regularFont.MeasureString(initialsText);

            Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);
            Vector2 messagePosition = new Vector2((Shared.stageSize.X - messageSize.X) / 2, 280);

            finishSceneSpriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
            finishSceneSpriteBatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);
            finishSceneSpriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            Vector2 initialsSize = hilightFont.MeasureString(initials);
            Vector2 initialsPosition = new Vector2((Shared.stageSize.X - initialsSize.X) / 2, 320);
            if (flag)
            {

                finishSceneSpriteBatch.DrawString(regularFont, initialsText, messagePosition, Color.White);
                finishSceneSpriteBatch.DrawString(hilightFont, initials, initialsPosition, Color.White);
            }


            finishSceneSpriteBatch.DrawString(regularFont, "Tiem: ", new Vector2(150, 450), Color.PaleVioletRed);
            finishSceneSpriteBatch.DrawString(hilightFont, PlayScene.TimeString, new Vector2(350, 440), Color.DarkViolet);

            finishSceneSpriteBatch.DrawString(regularFont, "Enemy Kill: ", new Vector2(150, 550), Color.PaleVioletRed);
            finishSceneSpriteBatch.DrawString(hilightFont, PlayScene.NumberOfDestoryedSmallHelicopter.ToString(), new Vector2(350, 540), Color.DarkViolet);

            finishSceneSpriteBatch.DrawString(regularFont, "Hitten: ", new Vector2(150, 650), Color.PaleVioletRed);
            finishSceneSpriteBatch.DrawString(hilightFont, PlayScene.NumberOfGotHit.ToString(), new Vector2(350, 640), Color.DarkViolet);

            finishSceneSpriteBatch.DrawString(regularFont, "Total Score: ", new Vector2(150, 800), Color.PaleVioletRed);
            if (flag)
            {
                finishSceneSpriteBatch.DrawString(hilightFont, (PlayScene.NumberOfDestoryedSmallHelicopter - PlayScene.NumberOfGotHit).ToString(), new Vector2(350, 790), Color.DarkViolet);
            }



            finishSceneSpriteBatch.End();
            base.Draw(gameTime);
        }
        private void SaveScore()
        {
            string filePath = "scores.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(initials);
                writer.WriteLine(PlayScene.NumberOfDestoryedSmallHelicopter - PlayScene.NumberOfGotHit);
            }
            FinishSceneCompleted?.Invoke();
        }
    }
}
