using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Final
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;


        private StartScene startScene;
        private PlayScene playScene;
        private HelpScene helpScene;

        private Song backgroundMusic;


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

            startScene = new StartScene(this);
            this.Components.Add(startScene);
            helpScene = new HelpScene(this);
            this.Components.Add(helpScene);
            playScene = new PlayScene(this);
            this.Components.Add(playScene);

            backgroundMusic = this.Content.Load<Song>("sounds/backgroundMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);

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

                if(MediaPlayer.State != MediaState.Playing)
                {
                    MediaPlayer.Play(backgroundMusic);
                }
            }
            else
            {
                if(MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Stop();
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
            base.Draw(gameTime);
        }
    }
}