﻿using Final.GameComponents;
using Final.GameObjects;
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

    public class PlayScene : GameScene
    {
        // SpriteBatch and texture for rendering
        private SpriteBatch playSceneSpriteBatch;
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

        // Properties
        public static List<SmallHelicopter> SmallHelicopterList { get => smallHelicopterList; set => smallHelicopterList = value; }
        public static string TimeString { get => timeString; set => timeString = value; }
        public BossHelicopter BossHelicopter { get => bossHelicopter; set => bossHelicopter = value; }

        // Events
        public event EndGameEventHandlerDelegate EndGameEventHandler;


        public PlayScene(Game game) : base(game)
        {
            // Initialize main game reference and sprite batch
            mainGame = (MainGame)game;
            playSceneSpriteBatch = mainGame._spriteBatch;

            // Load textures and set rectangles for background
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            topBackgroundRectangle = new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            bottomBackgroundRectangle = new Rectangle(0, -(int)Shared.stageSize.Y, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);

            // Initialize game objects
            fighterAircraft = new FighterAircraft(mainGame, playSceneSpriteBatch);
            ComponentList.Add(fighterAircraft);
            bossHelicopter = new BossHelicopter(mainGame, playSceneSpriteBatch);
            ComponentList.Add(bossHelicopter);
            collisionManager = new CollisionManager(mainGame, this, bossHelicopter, fighterAircraft);
            ComponentList.Add(collisionManager);
            smallHelicopterList = new List<SmallHelicopter>();

            // Load textures and set rectangles for UI elements
            singleBulletModeTexture = mainGame.Content.Load<Texture2D>("images/oneBulletMode");
            oneBulletModeRectangle = new Rectangle((int)Shared.stageSize.X - 120, (int)Shared.stageSize.Y - 250, (int)(singleBulletModeTexture.Width * iconScale), (int)(singleBulletModeTexture.Height * iconScale));
            tripleBulletModeTexture = mainGame.Content.Load<Texture2D>("images/threeBulletMode");
            threeBulletModeRectangle = new Rectangle((int)Shared.stageSize.X - 120, (int)Shared.stageSize.Y - 150, (int)(tripleBulletModeTexture.Width * iconScale), (int)(tripleBulletModeTexture.Height * iconScale));

            // Initialize game board
            gameBoard = new GameBoard(mainGame, playSceneSpriteBatch);
            GameBoard.NumberOfDestoryedSmallHelicopter = 0;
            GameBoard.NumberOfGotHit = 0;
            ComponentList.Add(gameBoard);
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
        private double smallHelicopterElapsedTime;
        private double smallHelicopterSpawnInterval = 3000;

        // End game animation and timing
        private double destroyAnimationElapsedTime;
        private double endGameAnimationDuration = 1000;
        private double closeGameElapsedTime = 0;
        private double closeGameInterval = 5000;

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
            playSceneSpriteBatch.Begin();
            //drawing background
            playSceneSpriteBatch.Draw(backgroundTexture, topBackgroundRectangle, Color.White);
            playSceneSpriteBatch.Draw(backgroundTexture, bottomBackgroundRectangle, Color.White);
            //drawing single or triple bullet mode icon
            playSceneSpriteBatch.Draw(singleBulletModeTexture, oneBulletModeRectangle, Color.White);
            playSceneSpriteBatch.Draw(tripleBulletModeTexture, threeBulletModeRectangle, Color.White);

            playSceneSpriteBatch.End();
            base.Draw(gameTime);
        }

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
                        AircraftBasicBullet aircraftBasicBullet = new AircraftBasicBullet(mainGame, playSceneSpriteBatch);
                        aircraftBasicBullet.RemoveBulletDelegate = RemoveAircraftBullet;
                        ComponentList.Add(aircraftBasicBullet);
                    }
                    else
                    {
                        AircraftBasicBullet leftAircraftBasicBullet = new AircraftBasicBullet(mainGame, playSceneSpriteBatch, "left");
                        AircraftBasicBullet centerAircraftBasicBullet = new AircraftBasicBullet(mainGame, playSceneSpriteBatch, "center");
                        AircraftBasicBullet rightAircraftBasicBullet = new AircraftBasicBullet(mainGame, playSceneSpriteBatch, "right");
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
        private void BossBulletManagement(GameTime gameTime)
        {
            //boss generating bullet time control logic
            bossBulletGenerationTime = isBossBulletActive == false ? bossBulletGenerationTime += gameTime.ElapsedGameTime.TotalMilliseconds : 0;

            if (bossBulletGenerationTime > 3000)
            {
                isBossBulletActive = true;
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
                        BossHelicopterBasicBullet bossBasicBullet = new BossHelicopterBasicBullet(mainGame, playSceneSpriteBatch);
                        bossBasicBullet.RemoveBossBulletDelegate = RemoveBossBasictBullet;
                        ComponentList.Add(bossBasicBullet);
                        bossBulletElapsedTime = 0;
                    }
                }
                else if (bossHelicopter.CurrentStage == BossHelicopter.BossStage.secondStage)
                {
                    if (bossBulletElapsedTime > bossBulletGeneratingSequence && !bossHelicopter.IsStartSequence)
                    {
                        BossHelicopterBasicBullet leftBossBasicBullet = new BossHelicopterBasicBullet(mainGame, playSceneSpriteBatch, "left");
                        BossHelicopterBasicBullet centerBossBasicBullet = new BossHelicopterBasicBullet(mainGame, playSceneSpriteBatch, "center");
                        BossHelicopterBasicBullet rightBossBasicBullet = new BossHelicopterBasicBullet(mainGame, playSceneSpriteBatch, "right");
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

        private void GenerateSmallHelicopters(GameTime gameTime)
        {
            // generating small helicopter
            if (!FighterAircraft.IsInEntrySequence)
            {
                smallHelicopterElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (smallHelicopterElapsedTime > smallHelicopterSpawnInterval)
                {
                    smallHelicopter = new SmallHelicopter(mainGame, playSceneSpriteBatch, this);
                    ComponentList.Add(smallHelicopter);
                    smallHelicopterElapsedTime = 0;

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

        private void TrackPlayTime(GameTime gameTime)
        {
            //tracking play time
            if (Enabled == true && Visible == true)
            {
                totalTimeElapsed += gameTime.ElapsedGameTime;
                TimeString = $"{totalTimeElapsed.Minutes:D2}:{totalTimeElapsed.Seconds:D2}";
            }
        }

        private void CheckEndGameConditions(GameTime gameTime)
        {
            //End Game initial point
            if (bossHelicopter.CurrentStage == BossHelicopter.BossStage.destroyed)
            {
                destroyAnimationElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (destroyAnimationElapsedTime >= endGameAnimationDuration)
                {
                    DestoryAnimation destoryAnimation = new DestoryAnimation(mainGame, playSceneSpriteBatch);
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
