using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;

        private Texture2D backgroundTexture;

        private StartScene startScene;
        private PlayScene playScene;
        private HelpScene helpScene;


        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 1000;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Shared.stageSize = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            backgroundTexture = Content.Load<Texture2D>("images/background");

            startScene = new StartScene(this);
            this.Components.Add(startScene);
            helpScene = new HelpScene(this);
            this.Components.Add(helpScene);
            playScene = new PlayScene(this);
            this.Components.Add(playScene);

            startScene.Show();
        }
        private void HideAllScenes()
        {
            foreach (GameComponent gameComponent in Components)
            {
                if(gameComponent is GameScene)
                {
                    GameScene gameScene = (GameScene)gameComponent;
                    gameScene.Hide();
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here
            KeyboardState keyboardState = Keyboard.GetState();
            int selectedIndex = 0;

            if (startScene.Enabled)
            {
                selectedIndex = startScene.MenuComponent.SelectedIndex;
                if (selectedIndex == 0 && keyboardState.IsKeyDown(Keys.Enter))
                {
                    startScene.Hide();
                    playScene.Show();
                }
                else if (selectedIndex == 1 && keyboardState.IsKeyDown(Keys.Enter))
                {
                    startScene.Hide();
                    helpScene.Show();
                }
                else if (selectedIndex == 4 && keyboardState.IsKeyDown(Keys.Enter))
                {
                    Exit();
                }

            }
            if(playScene.Enabled || helpScene.Enabled)
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    HideAllScenes();
                    startScene.Show();
                }
            }
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}