using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final.Scenes
{
    /// <summary>
    /// About Scene
    /// </summary>
    public class CreditScene : GameScene
    {
        // Rendering components
        private SpriteBatch spritebatch;

        // Fonts
        private SpriteFont titleFont;
        private SpriteFont regularFont;
        private SpriteFont hilightFont;

        // Textures
        private Texture2D backgroundTexture;
        private Texture2D transparentBackground;

        // Text for credits
        private string titleText = "2023";
        private string createdBy = "Created By";
        private string creator1 = "Jindo Kim";
        private string creator2 = "Sangkwon Kim";

        /// <summary>
        /// About Scene Constructor
        /// </summary>
        /// <param name="game"></param>
        public CreditScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            spritebatch = mainGame.SpriteBatch;

            LoadFonts();
            void LoadFonts()
            {
                regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
                hilightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
                titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            }
            LoadTextures();
            void LoadTextures()
            {
                backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
                transparentBackground = new Texture2D(GraphicsDevice, 1, 1);
                transparentBackground.SetData(new[] { Color.Black });
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spritebatch.Begin();
            DrawBackground();
            void DrawBackground()
            {
                spritebatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
                spritebatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);
            }
            DrawTitle();
            void DrawTitle()
            {
                Vector2 titleSize = titleFont.MeasureString(titleText);
                Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);
                spritebatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);
            }
            DrawCredits();
            void DrawCredits()
            {
                Vector2 subTitleSize = hilightFont.MeasureString(createdBy);
                Vector2 SubtitlePosition = new Vector2((Shared.stageSize.X - subTitleSize.X) / 2, 300);
                spritebatch.DrawString(hilightFont, createdBy, SubtitlePosition, Color.DarkViolet);

                Vector2 creator1Size = regularFont.MeasureString(creator1);
                Vector2 creator2Size = regularFont.MeasureString(creator2);
                Vector2 creator1Position = new Vector2((Shared.stageSize.X - creator1Size.X) / 2, 400);
                Vector2 creator2Position = new Vector2((Shared.stageSize.X - creator2Size.X) / 2, 450);
                spritebatch.DrawString(regularFont, creator1, creator1Position, Color.PaleVioletRed);
                spritebatch.DrawString(regularFont, creator2, creator2Position, Color.PaleVioletRed);
            }
            spritebatch.End();
            base.Draw(gameTime);
        }







    }
}
