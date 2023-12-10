using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Final
{
    /// <summary>
    /// Game Controller Class
    /// </summary>
    public class MainGame : Game
    {
        // Graphics and rendering components
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        // Game scenes
        private StartScene startScene;
        private PlayScene playScene;
        private HelpScene helpScene;
        private CreditScene creditScene;
        private FinishScene finishScene;
        private LeaderBoardScene leaderBoardScene;

        // Audio components
        private Song backgroundMusic;
        private SoundEffect playSceneBackgroundSound;
        private SoundEffectInstance soundInstance;

        // Game state and control
        private int selectedIndex = 0;

        public SpriteBatch SpriteBatch { get => spriteBatch; set => spriteBatch = value; }

        /// <summary>
        /// Default Constructor to set basic up
        /// </summary>
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
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            LoadScenes();
            void LoadScenes()
            {
                startScene = new StartScene(this);
                Components.Add(startScene);

                helpScene = new HelpScene(this);
                Components.Add(helpScene);

                playScene = new PlayScene(this);
                Components.Add(playScene);
                playScene.EndGameEventHandler += EndPlayMode;

                creditScene = new CreditScene(this);
                Components.Add(creditScene);

                finishScene = new FinishScene(this);
                Components.Add(finishScene);
                finishScene.FinishSceneCompleted += ReturnToStartScene;

                leaderBoardScene = new LeaderBoardScene(this);
                Components.Add(leaderBoardScene);
            }
            LoadAudioContent();
            void LoadAudioContent()
            {
                backgroundMusic = Content.Load<Song>("sounds/backgroundMusic");
                MediaPlayer.IsRepeating = true;

                playSceneBackgroundSound = Content.Load<SoundEffect>("sounds/playSceneBackgroundSound");
                soundInstance = playSceneBackgroundSound.CreateInstance();
            }
            //Show initial scene
            startScene.Show();

        }

        protected override void Update(GameTime gameTime)
        {
            CheckForReturnToStartToScene();

            // Manage scene transitions and game state
            AccessScenesOnMenu();

            // Manage audio playback
            ManageAudioPlayback();

            base.Update(gameTime);
        }

        /// <summary>
        /// To get Start Scene
        /// </summary>
        private void CheckForReturnToStartToScene()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Esc key for returning to the start scene from any other scene
            if (playScene.Enabled || helpScene.Enabled || creditScene.Enabled || finishScene.Enabled || leaderBoardScene.Enabled)
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    ReturnToStartScene();
                }
            }
        }
        /// <summary>
        /// Access a selected scene 
        /// </summary>
        private void AccessScenesOnMenu()
        {
            if (startScene.Enabled)
            {
                selectedIndex = startScene.MenuComponent.SelectedIndex;
                switch (selectedIndex)
                {
                    case 0://playScene
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            TransitionToScene(playScene);
                            soundInstance.Play();
                        }
                        break;
                    case 1://helpScene
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            TransitionToScene(helpScene);
                        }
                        break;
                    case 2://leaderBoardScene
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            leaderBoardScene.LoadTopScores();
                            TransitionToScene(leaderBoardScene);
                        }
                        break;
                    case 3://creditScene
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            TransitionToScene(creditScene);
                        }
                        break;
                    case 4://Exit
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            Exit();
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Transtion to the selected scene
        /// </summary>
        /// <param name="scene"></param>
        private void TransitionToScene(GameScene scene)
        {
            startScene.Hide();
            scene.Show();
        }
        /// <summary>
        /// Manage start scene background music
        /// </summary>
        private void ManageAudioPlayback()
        {
            if (startScene.Enabled && MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(backgroundMusic);
            }
            else if (!startScene.Enabled && MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Called when one game is completed for new game play scene
        /// </summary>
        private void EndPlayMode()
        {
            playScene.Hide();
            finishScene.Show();
            Components.Remove(playScene);
            playScene = new PlayScene(this);
            playScene.EndGameEventHandler += EndPlayMode;
            Components.Add(playScene);
        }

        /// <summary>
        /// To restart a new game
        /// </summary>
        private void ReturnToStartScene()
        {
            HideAllScenes();
            startScene.Show();
            startScene.MenuComponent.SelectedIndex = -1;
            soundInstance.Stop();
        }

        /// <summary>
        /// Hides all game scenes by setting their visibility and enabled state to false. This is used for resetting the game environment.
        /// </summary>
        private void HideAllScenes()
        {
            foreach (GameComponent gameComponent in Components)
            {
                if (gameComponent is GameScene)
                {
                    GameScene gameScene = (GameScene)gameComponent;
                    gameScene.Hide();
                }
            }
        }
    }
}