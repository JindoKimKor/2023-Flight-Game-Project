using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        private double delay = 1;
        private double delayCounter;
        private bool flag = true;
        
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
        }
        public override void Update(GameTime gameTime)
        {
            delayCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (delayCounter >= delay)
            {
                flag = !flag;
                delayCounter = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            finishSceneSpriteBatch.Begin();
            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);
            
            finishSceneSpriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
            finishSceneSpriteBatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);
            finishSceneSpriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            finishSceneSpriteBatch.DrawString(regularFont, "Tiem: ", new Vector2(150, 400), Color.PaleVioletRed);
            finishSceneSpriteBatch.DrawString(hilightFont, PlayScene.TimeString, new Vector2(350, 390), Color.DarkViolet);

            finishSceneSpriteBatch.DrawString(regularFont, "Enemy Kill: ", new Vector2(150, 500), Color.PaleVioletRed);
            finishSceneSpriteBatch.DrawString(hilightFont, PlayScene.NumberOfDestoryedSmallHelicopter.ToString(), new Vector2(350, 490), Color.DarkViolet);

            finishSceneSpriteBatch.DrawString(regularFont, "Hitten: ", new Vector2(150, 600), Color.PaleVioletRed);
            finishSceneSpriteBatch.DrawString(hilightFont, PlayScene.NumberOfGotHit.ToString(), new Vector2(350, 590), Color.DarkViolet);
            
            finishSceneSpriteBatch.DrawString(regularFont, "Total Score: ", new Vector2(150, 800), Color.PaleVioletRed);
            if (flag)
            {
                finishSceneSpriteBatch.DrawString(hilightFont, (PlayScene.NumberOfDestoryedSmallHelicopter - PlayScene.NumberOfGotHit).ToString(), new Vector2(350, 790), Color.DarkViolet); 
            }



            finishSceneSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
