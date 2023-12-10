using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Final.Scenes
{
    /// <summary>
    /// Help Scene
    /// </summary>
    internal class HelpScene : GameScene
    {
        // Rendering components
        private SpriteBatch spriteBatch;

        // Fonts for text
        private SpriteFont titleFont;
        private SpriteFont regularFont;

        // Textures for scene visuals
        private Texture2D backgroundTexture;
        private Texture2D transparentBackground;
        private Texture2D wasdTexture;
        private Texture2D spaceBarTexture;

        // Bullet animation fields
        private Texture2D bulletTexture;
        private Vector2 bulletFrameDimension;
        private List<Rectangle> bulletAnimationFrames;
        private const int BULLET_ROWS = 4;
        private Vector2 bulletPosition;
        private int currentBulletFrameIndex = 0;
        private double bulletFrameInterval = 40;
        private double bulletElapsedTime = 0;

        // Helicopter animation fields
        private Texture2D helicopterTexture;
        private Vector2 helicopterFrameDimension;
        private List<Rectangle> helicopterAnimationFrames;
        private const int HELICOPTER_ROWS = 4;
        private Vector2 helicopterPosition;
        private int currentHelicopterFrameIndex = 0;
        private double helicopterFrameInterval = 40;
        private double helicopterElapsedTime = 0;

        private const string TITLE_TEXT = "HELP";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public HelpScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            spriteBatch = mainGame.SpriteBatch;
            LoadFonts();
            void LoadFonts()
            {
                regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
                titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            }
            LoadTextures();
            void LoadTextures()
            {
                backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
                transparentBackground = new Texture2D(GraphicsDevice, 1, 1);
                transparentBackground.SetData(new[] { Color.Black });
                wasdTexture = mainGame.Content.Load<Texture2D>("images/wasdKeyboardIcon");
                spaceBarTexture = mainGame.Content.Load<Texture2D>("images/spacebar");
            }
            InitializeBulletAnimation();
            void InitializeBulletAnimation()
            {
                bulletTexture = game.Content.Load<Texture2D>("images/bossHelicopterBasicBullet");
                bulletFrameDimension = new Vector2(bulletTexture.Width / BULLET_ROWS, bulletTexture.Height - 1);
                bulletAnimationFrames = new List<Rectangle>();
                bulletPosition = new Vector2(135, 525);

                for (int r = 0; r < BULLET_ROWS; r++)
                {
                    int x = r * (int)bulletFrameDimension.X;
                    bulletAnimationFrames.Add(new Rectangle(x, 0, (int)bulletFrameDimension.X, (int)bulletFrameDimension.Y));
                }
            }
            InitializeHelicopterAnimation();
            void InitializeHelicopterAnimation()
            {
                helicopterTexture = game.Content.Load<Texture2D>("images/firstStageBossHelicopter");
                helicopterFrameDimension = new Vector2(helicopterTexture.Width / HELICOPTER_ROWS, helicopterTexture.Height);
                helicopterAnimationFrames = new List<Rectangle>();
                helicopterPosition = new Vector2(105, 570);

                for (int r = 0; r < HELICOPTER_ROWS; r++)
                {
                    int x = r * (int)helicopterFrameDimension.X;
                    helicopterAnimationFrames.Add(new Rectangle(x, 0, (int)helicopterFrameDimension.X, (int)helicopterFrameDimension.Y));
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            UpdateBulletAnimation();
            void UpdateBulletAnimation()
            {
                bulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (bulletElapsedTime >= bulletFrameInterval)
                {
                    currentBulletFrameIndex++;
                    if (currentBulletFrameIndex >= BULLET_ROWS)
                    {
                        currentBulletFrameIndex = 0;
                    }
                    bulletElapsedTime = 0;
                }
            }
            UpdateHelicopterAnimation();
            void UpdateHelicopterAnimation()
            {
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
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            DrawBackground();
            void DrawBackground()
            {
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
                spriteBatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);
            }
            DrawTitle();
            void DrawTitle()
            {
                Vector2 titleSize = titleFont.MeasureString(TITLE_TEXT);
                Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);
                spriteBatch.DrawString(titleFont, TITLE_TEXT, titlePosition, Color.BlueViolet);
            }
            DrawInstructions();
            void DrawInstructions()
            {
                // Text instructions
                spriteBatch.DrawString(regularFont, "Control the fighter \nusing the WASD keys", new Vector2(270, 300), Color.PaleVioletRed);
                spriteBatch.DrawString(regularFont, "Fire bullets using \nthe space bar", new Vector2(270, 400), Color.PaleVioletRed);
                spriteBatch.DrawString(regularFont, "Dodge the bullets \nfired by enemies", new Vector2(270, 500), Color.PaleVioletRed);
                spriteBatch.DrawString(regularFont, "Defeat the enemy planes", new Vector2(270, 600), Color.PaleVioletRed);
                spriteBatch.DrawString(regularFont, "Total score will be the count of defeated", new Vector2(50, 700), Color.PaleVioletRed);
                spriteBatch.DrawString(regularFont, "enemy planes deducted by hitten count", new Vector2(53, 740), Color.PaleVioletRed);
                spriteBatch.DrawString(regularFont, "The game will end when the boss dead", new Vector2(53, 780), Color.PaleVioletRed);
            }
            DrawControls();
            void DrawControls()
            {
                // Control images (WASD keys and Spacebar)
                spriteBatch.Draw(wasdTexture, new Rectangle(70, 260, 150, 150), Color.White);
                spriteBatch.Draw(spaceBarTexture, new Rectangle(70, 360, 150, 150), Color.White);
            }
            DrawAnimations();
            void DrawAnimations()
            {
                // Bullet and Helicopter animations
                spriteBatch.Draw(bulletTexture, bulletPosition, bulletAnimationFrames[currentBulletFrameIndex], Color.White, 0f, Vector2.Zero, 0.1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(helicopterTexture, helicopterPosition, helicopterAnimationFrames[currentHelicopterFrameIndex], Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
