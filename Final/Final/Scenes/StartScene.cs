using Final.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final.Scenes
{
    /// <summary>
    /// The central hub of the scene, providing a main menu
    /// </summary>
    public class StartScene : GameScene
    {
        private MenuComponent menuComponent;
        private SpriteBatch spriteBatch;
        private SpriteFont titleFont;
        private SpriteFont regularFont;
        private SpriteFont highlightFont;
        private Texture2D backgroundTexture;
        private string titleText = "2023";


        public MenuComponent MenuComponent { get => menuComponent; set => menuComponent = value; }

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="game"></param>
        public StartScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            spriteBatch = mainGame.SpriteBatch;

            // Load fonts
            LoadFonts();
            void LoadFonts()
            {
                regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
                highlightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
                titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            }

            // Load background texture
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");

            // Initialize menu component
            InitializeMenuComponent();
            void InitializeMenuComponent()
            {
                string[] menuItems = { "Start Game", "Help", "Leaderboard", "Credit", "Exit" };
                menuComponent = new MenuComponent(game, spriteBatch, regularFont, highlightFont, menuItems);
                ComponentList.Add(menuComponent);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Game.GraphicsDevice.Viewport.Width - titleSize.X) / 2, 100);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
            spriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}
