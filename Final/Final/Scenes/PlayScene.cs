using Final.GameObjects;
using Final.GameObjects.Characters;
using Final.GameObjects.Mechanics;
using Final.GameObjects.UIs;
using Final.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    public delegate void EndGameEventHandlerDelegate();
    /// <summary>
    /// Play Scene Class
    /// </summary>
    public class PlayScene : GameScene
    {
        // SpriteBatch and texture for rendering
        private SpriteBatch spriteBatch;
        private Texture2D backgroundTexture;
        private Rectangle topBackgroundRectangle;
        private Rectangle bottomBackgroundRectangle;

        // Background scrolling speed
        private int backgroundTextureScrollSpeed = 3;

        // Game objects
        private FighterAircraft fighterAircraft;
        private BossHelicopter bossHelicopter;
        private CollisionManager collisionManager;
        private SmallHelicopter smallHelicopter;
        private GameBoard gameBoard;

        // Lists and collections
        private static List<SmallHelicopter> smallHelicopterList;

        // Textures and UI elements
        private Texture2D singleBulletModeTexture;
        private Rectangle oneBulletModeRectangle;
        private Texture2D tripleBulletModeTexture;
        private Rectangle threeBulletModeRectangle;
        private bool isSingleBulletMode = true;
        private float iconScale = 0.2f;

        // Main game reference and time tracking
        private MainGame mainGame;
        private TimeSpan totalTimeElapsed;
        private static string timeString;


        private static int numberOfDestoryedSmallHelicopter;
        private static int numberOfGotHit;

        public static int NumberOfDestoryedSmallHelicopter { get => numberOfDestoryedSmallHelicopter; set => numberOfDestoryedSmallHelicopter = value; }
        public static int NumberOfGotHit { get => numberOfGotHit; set => numberOfGotHit = value; }
        // Properties
        public static List<SmallHelicopter> SmallHelicopterList { get => smallHelicopterList; set => smallHelicopterList = value; }
        public static string TimeString { get => timeString; set => timeString = value; }
        public BossHelicopter BossHelicopter { get => bossHelicopter; set => bossHelicopter = value; }

        // Events
        public event EndGameEventHandlerDelegate EndGameEventHandler;

        /// <summary>
        /// PlayScene Constructor
        /// </summary>
        /// <param name="game"></param>
        public PlayScene(Game game) : base(game)
        {
            // Initialize main game reference and sprite batch
            mainGame = (MainGame)game;
            spriteBatch = mainGame.SpriteBatch;

            // Load textures and set rectangles for background
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            topBackgroundRectangle = new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            bottomBackgroundRectangle = new Rectangle(0, -(int)Shared.stageSize.Y, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);

            // Initialize game objects
            fighterAircraft = new FighterAircraft(mainGame, spriteBatch);
            ComponentList.Add(fighterAircraft);
            bossHelicopter = new BossHelicopter(mainGame, spriteBatch);
            ComponentList.Add(bossHelicopter);
            collisionManager = new CollisionManager(mainGame, this, bossHelicopter, fighterAircraft);
            ComponentList.Add(collisionManager);
            smallHelicopterList = new List<SmallHelicopter>();

            // Load textures and set rectangles for UI elements
            singleBulletModeTexture = mainGame.Content.Load<Texture2D>("images/oneBulletMode");
            oneBulletModeRectangle = new Rectangle((int)Shared.stageSize.X - 120, (int)Shared.stageSize.Y - 250, (int)(singleBulletModeTexture.Width * iconScale), (int)(singleBulletModeTexture.Height * iconScale));
            
        }

        // Aircraft bullet management
        private double lastBulletTime = 0;
        private double aircraftBulletCooldown = 200;

        // Boss bullet management
        private double bossBulletElapsedTime;
        private double bossBulletGenerationTime;
        private double bossBulletStopTime;
        private bool isBossBulletActive;
        private double bossBulletGeneratingSequence = 800;

        // Small helicopter management
        private double smallHelicopterSpawningElapsedTime;
        private double smallHelicopterSpawnInterval = 1500;

        // End game animation and timing
        private double destroyAnimationElapsedTime;
        private double endGameAnimationDuration = 1000;
        private double closeGameElapsedTime = 0;
        private double closeGameInterval = 10000;

        public override void Update(GameTime gameTime)
        {
            ScrollBackground();

            HandleInputAndBulletGeneration(gameTime);

            BossBulletManagement(gameTime);

            GenerateSmallHelicopters(gameTime);

            TrackPlayTime(gameTime);

            CheckEndGameConditions(gameTime);

            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //drawing background
            spriteBatch.Draw(backgroundTexture, topBackgroundRectangle, Color.White);
            spriteBatch.Draw(backgroundTexture, bottomBackgroundRectangle, Color.White);
            //drawing single or triple bullet mode icon
            spriteBatch.Draw(singleBulletModeTexture, oneBulletModeRectangle, Color.White);
            
            if (bossHelicopter.CurrentStage != BossHelicopter.BossStage.firstStage)
            {
                tripleBulletModeTexture = mainGame.Content.Load<Texture2D>("images/threeBulletMode");
                threeBulletModeRectangle = new Rectangle((int)Shared.stageSize.X - 120, (int)Shared.stageSize.Y - 150, (int)(tripleBulletModeTexture.Width * iconScale), (int)(tripleBulletModeTexture.Height * iconScale));
                spriteBatch.Draw(tripleBulletModeTexture, threeBulletModeRectangle, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// To create an illusion of movement in the game by scrolling the background vertically
        /// </summary>
        private void ScrollBackground()
        {
            topBackgroundRectangle.Y += backgroundTextureScrollSpeed;
            bottomBackgroundRectangle.Y += backgroundTextureScrollSpeed;
            if (topBackgroundRectangle.Y >= (int)Shared.stageSize.Y)
            {
                topBackgroundRectangle.Y = 0;
            }
            if (bottomBackgroundRectangle.Y >= 0)
            {
                bottomBackgroundRectangle.Y = -(int)Shared.stageSize.Y;
            }
        }

        /// <summary>
        /// Main Character bullet mode, generating and destroying bullets
        /// </summary>
        /// <param name="gameTime"></param>
        private void HandleInputAndBulletGeneration(GameTime gameTime)
        {
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            KeyboardState keyboardState = Keyboard.GetState();

            MouseState mouseState = Mouse.GetState();

            //one or three bullet Mode
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Point mousePosition = new Point(mouseState.X, mouseState.Y);
                if (oneBulletModeRectangle.Contains(mousePosition))
                {
                    isSingleBulletMode = true;
                }
                else if (threeBulletModeRectangle.Contains(mousePosition))
                {
                    isSingleBulletMode = false;
                }
            }

            //generating aircraft basic bullet
            if (keyboardState.IsKeyDown(Keys.Space) && !FighterAircraft.IsInEntrySequence)
            {
                if (currentTime - lastBulletTime > aircraftBulletCooldown)
                {
                    if (isSingleBulletMode)
                    {
                        AircraftBasicBullet aircraftBasicBullet = new AircraftBasicBullet(mainGame, spriteBatch);
                        aircraftBasicBullet.RemoveBulletDelegate = RemoveAircraftBullet;
                        ComponentList.Add(aircraftBasicBullet);
                    }
                    else
                    {
                        AircraftBasicBullet leftAircraftBasicBullet = new AircraftBasicBullet(mainGame, spriteBatch, "left");
                        AircraftBasicBullet centerAircraftBasicBullet = new AircraftBasicBullet(mainGame, spriteBatch, "center");
                        AircraftBasicBullet rightAircraftBasicBullet = new AircraftBasicBullet(mainGame, spriteBatch, "right");
                        leftAircraftBasicBullet.RemoveBulletDelegate = RemoveAircraftBullet;
                        centerAircraftBasicBullet.RemoveBulletDelegate = RemoveAircraftBullet;
                        rightAircraftBasicBullet.RemoveBulletDelegate = RemoveAircraftBullet;
                        ComponentList.Add(leftAircraftBasicBullet);
                        ComponentList.Add(centerAircraftBasicBullet);
                        ComponentList.Add(rightAircraftBasicBullet);
                    }
                    lastBulletTime = currentTime;
                }
                void RemoveAircraftBullet(AircraftBasicBullet aircraftBasicBullet)
                {
                    ComponentList.Remove(aircraftBasicBullet);
                    aircraftBasicBullet.Dispose();
                }
            }
        }

        /// <summary>
        /// Generating and Destroy Boss bullets depends on boss's stage level
        /// </summary>
        /// <param name="gameTime"></param>
        private void BossBulletManagement(GameTime gameTime)
        {
            //boss generating bullet time control logic
            bossBulletGenerationTime = isBossBulletActive == false ? bossBulletGenerationTime += gameTime.ElapsedGameTime.TotalMilliseconds : 0;

            if (bossBulletGenerationTime > 3000)
            {
                isBossBulletActive = true;
                // Initialize game board
                gameBoard = new GameBoard(mainGame, spriteBatch);
                ComponentList.Add(gameBoard);
            }
            if (bossBulletStopTime > 3000)
            {
                isBossBulletActive = false;
                bossBulletStopTime = 0;
            }

            //generating boss basic bullet
            if (isBossBulletActive)
            {
                bossBulletStopTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                //generating bullet sequence logic
                bossBulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (bossHelicopter.CurrentStage == BossHelicopter.BossStage.firstStage)
                {
                    if (bossBulletElapsedTime > bossBulletGeneratingSequence && !bossHelicopter.IsStartSequence)
                    {
                        BossHelicopterBasicBullet bossBasicBullet = new BossHelicopterBasicBullet(mainGame, spriteBatch);
                        bossBasicBullet.RemoveBossBulletDelegate = RemoveBossBasictBullet;
                        ComponentList.Add(bossBasicBullet);
                        bossBulletElapsedTime = 0;
                    }
                }
                else if (bossHelicopter.CurrentStage == BossHelicopter.BossStage.secondStage)
                {
                    if (bossBulletElapsedTime > bossBulletGeneratingSequence && !bossHelicopter.IsStartSequence)
                    {
                        BossHelicopterBasicBullet leftBossBasicBullet = new BossHelicopterBasicBullet(mainGame, spriteBatch, "left");
                        BossHelicopterBasicBullet centerBossBasicBullet = new BossHelicopterBasicBullet(mainGame, spriteBatch, "center");
                        BossHelicopterBasicBullet rightBossBasicBullet = new BossHelicopterBasicBullet(mainGame, spriteBatch, "right");
                        leftBossBasicBullet.RemoveBossBulletDelegate = RemoveBossBasictBullet;
                        centerBossBasicBullet.RemoveBossBulletDelegate = RemoveBossBasictBullet;
                        rightBossBasicBullet.RemoveBossBulletDelegate = RemoveBossBasictBullet;
                        ComponentList.Add(leftBossBasicBullet);
                        ComponentList.Add(centerBossBasicBullet);
                        ComponentList.Add(rightBossBasicBullet);
                        bossBulletElapsedTime = 0;
                    }
                }

                void RemoveBossBasictBullet(BossHelicopterBasicBullet bossHelicopterBasicBullet)
                {
                    ComponentList.Remove(bossHelicopterBasicBullet);
                    bossHelicopterBasicBullet.Dispose();
                }
            }
        }

        /// <summary>
        /// Generating and destroying Small Helicopter as an enemy
        /// </summary>
        /// <param name="gameTime"></param>
        private void GenerateSmallHelicopters(GameTime gameTime)
        {
            // generating small helicopter
            if (!FighterAircraft.IsInEntrySequence)
            {
                smallHelicopterSpawningElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (smallHelicopterSpawningElapsedTime > smallHelicopterSpawnInterval)
                {
                    smallHelicopter = new SmallHelicopter(mainGame, spriteBatch, this);
                    ComponentList.Add(smallHelicopter);

                    smallHelicopterSpawningElapsedTime = bossHelicopter.CurrentStage == BossHelicopter.BossStage.firstStage ? 0 : 1000;

                    smallHelicopter.RemovePassedOrExpolosed += RemoveSmallHelicopter;
                    void RemoveSmallHelicopter(SmallHelicopter smallHelicopter)
                    {
                        ComponentList.Remove(smallHelicopter);
                        SmallHelicopterList.Remove(smallHelicopter);
                        smallHelicopter.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// To Track play time used in GameBoard Class
        /// </summary>
        /// <param name="gameTime"></param>
        private void TrackPlayTime(GameTime gameTime)
        {
            //tracking play time
            if (Enabled == true && Visible == true)
            {
                totalTimeElapsed += gameTime.ElapsedGameTime;
                TimeString = $"{totalTimeElapsed.Minutes:D2}:{totalTimeElapsed.Seconds:D2}";
            }
        }
        /// <summary>
        /// Check Boss Stage level and Ending game initial point
        /// </summary>
        /// <param name="gameTime"></param>
        private void CheckEndGameConditions(GameTime gameTime)
        {
            //End Game initial point
            if (bossHelicopter.CurrentStage == BossHelicopter.BossStage.destroyed)
            {
                destroyAnimationElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (destroyAnimationElapsedTime >= endGameAnimationDuration)
                {
                    DestroyBossAnimation destoryAnimation = new DestroyBossAnimation(mainGame, spriteBatch);
                    ComponentList.Add(destoryAnimation);
                    destroyAnimationElapsedTime = 0;
                }
                closeGameElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (closeGameElapsedTime >= closeGameInterval)
                {
                    EndGameEventHandler?.Invoke();
                }
            }
        }
    }
}
