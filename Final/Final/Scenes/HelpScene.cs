using Final.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    internal class HelpScene : GameScene
    {
        private SpriteBatch helpSceneSpriteBatch;
        private SpriteFont titleFont;
        private Texture2D backgroundTexture;
        private string titleText = "HELP";
        private SpriteFont regularFont;
        private SpriteFont hilightFont;
        private Texture2D transparentBackground;

        private Texture2D WASDTexture;
        private Texture2D SpaceBarTexture;

        private Texture2D bulletTexture;
        private Vector2 bulletFrameDimension;
        private List<Rectangle> bulletAnimationFrames;
        private const int BULLET_ROWS = 4;
        private Vector2 bulletPosition;
        private int currentBulletFrameIndex = 0;
        private double bulletFrameInterval = 40;
        private double bulletElapsedTime = 0;

        private Texture2D helicopterTexture;
        private Vector2 helicopterFrameDimension;
        private List<Rectangle> helicopterAnimationFrames;
        private const int HELICOPTER_ROWS = 4;
        private Vector2 helicopterPosition;
        private int currentHelicopterFrameIndex = 0;
        private double helicopterFrameInterval = 40;
        private double helicopterElapsedTime = 0;

        public HelpScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            helpSceneSpriteBatch = mainGame._spriteBatch;
            regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
            hilightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            transparentBackground = new Texture2D(GraphicsDevice,1, 1);
            transparentBackground.SetData(new[] { Color.Black });
            WASDTexture = mainGame.Content.Load<Texture2D>("images/wasd (1)");
            SpaceBarTexture = mainGame.Content.Load<Texture2D>("images/spacebar");

            bulletTexture = mainGame.Content.Load<Texture2D>("images/bossHelicopterBasicBullet");
            bulletFrameDimension = new Vector2(bulletTexture.Width / BULLET_ROWS, bulletTexture.Height-1);
            bulletAnimationFrames = new List<Rectangle>();
            bulletPosition = new Vector2(135,525); 
            
            for(int r = 0; r<BULLET_ROWS; r++){
                int x = r * (int)bulletFrameDimension.X;
                bulletAnimationFrames.Add(new Rectangle(x, 0, (int)bulletFrameDimension.X, (int)bulletFrameDimension.Y));
            }

            helicopterTexture = mainGame.Content.Load<Texture2D>("images/firstStageBossHelicopter");
            helicopterFrameDimension = new Vector2(helicopterTexture.Width / HELICOPTER_ROWS, helicopterTexture.Height);
            helicopterAnimationFrames = new List<Rectangle>();
            helicopterPosition = new Vector2(105, 570);

            for(int r = 0; r<HELICOPTER_ROWS; r++)
            {
                int x = r * (int)helicopterFrameDimension.X;
                helicopterAnimationFrames.Add(new Rectangle(x, 0, (int)helicopterFrameDimension.X, (int)helicopterFrameDimension.Y));
            }

        }
        public override void Update(GameTime gameTime)
        {
            bulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if(bulletElapsedTime >= bulletFrameInterval)
            {
                currentBulletFrameIndex++;
                if(currentBulletFrameIndex >= BULLET_ROWS)
                {
                    currentBulletFrameIndex = 0;
                }
                bulletElapsedTime= 0;
            }

            helicopterElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (helicopterElapsedTime >= helicopterFrameInterval)
            {
                currentHelicopterFrameIndex++;
                if (currentHelicopterFrameIndex >= HELICOPTER_ROWS)
                {
                    currentHelicopterFrameIndex = 0;
                }
                helicopterElapsedTime = 0;
            }

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            helpSceneSpriteBatch.Begin();
            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);

            helpSceneSpriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
            helpSceneSpriteBatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f); 

            helpSceneSpriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            helpSceneSpriteBatch.DrawString(regularFont, "Control the airplane \nusing the WASD keys", new Vector2(270, 300), Color.PaleVioletRed);
            helpSceneSpriteBatch.DrawString(regularFont, "Fire bullets using \nthe space bar", new Vector2(270, 400), Color.PaleVioletRed);
            helpSceneSpriteBatch.DrawString(regularFont, "Dodge the bullets \nfired by enemies", new Vector2(270, 500), Color.PaleVioletRed);
            helpSceneSpriteBatch.DrawString(regularFont, "Defeat the enemy planes", new Vector2(270, 600), Color.PaleVioletRed);
            helpSceneSpriteBatch.DrawString(regularFont, "Total score will be the count of defeated", new Vector2(50, 700), Color.PaleVioletRed);
            helpSceneSpriteBatch.DrawString(regularFont, "enemy planes and total time", new Vector2(130, 740), Color.PaleVioletRed);

            helpSceneSpriteBatch.Draw(WASDTexture, new Rectangle(70, 260, 150, 150), Color.White);
            helpSceneSpriteBatch.Draw(SpaceBarTexture, new Rectangle(70, 360, 150, 150), Color.White);

            helpSceneSpriteBatch.Draw(bulletTexture, bulletPosition, bulletAnimationFrames[currentBulletFrameIndex], Color.White, 0f, Vector2.Zero, 0.1f, SpriteEffects.None, 0f);
            helpSceneSpriteBatch.Draw(helicopterTexture, helicopterPosition, helicopterAnimationFrames[currentHelicopterFrameIndex], Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            helpSceneSpriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
