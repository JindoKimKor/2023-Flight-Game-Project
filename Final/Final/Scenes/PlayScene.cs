using Final.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    public class PlayScene : GameScene
    {
        private SpriteBatch playSceneSpriteBatch;
        private Texture2D backgroundTexture;
        private Rectangle upperScreenRectangle;
        private Rectangle bottomScreenRectangle;
        private int backgroundTextureScrollSpeed = 3;

        private FighterAircraft fighterAircraft;
        private BossHelicopter bossHelicopter;
        private CollisionManager collisionManager;
        private SmallHelicopter smallHelicopter;
        private static List<SmallHelicopter> smallHelicopterList;

        private Texture2D fighterAircraftTexture;
        private static bool isStartingSequence = true;
        private Vector2 fighterAircraftStartingPosition;
        private float fighterAircraftFullyLoadedYPosition = Shared.stageSize.Y - 250;
        private static Vector2 fighterAircraftCurrentPosition;
        private int fighterAircraftEntrySpeed = 2;
        private AircraftFrames currentFrame;

        private MainGame mainGame;

        private Dictionary<AircraftFrames, Vector2> aircraftDirectionsWithSpeed;

        private Dictionary<AircraftFrames, double> directionDurations;

        public static bool IsStartingSequence { get => isStartingSequence; set => isStartingSequence = value; }
        public static Vector2 FighterAircraftCurrentPosition { get => fighterAircraftCurrentPosition; set => fighterAircraftCurrentPosition = value; }
        public static List<SmallHelicopter> SmallHelicopterList { get => smallHelicopterList; set => smallHelicopterList = value; }

        public PlayScene(Game game) : base(game)
        {
            mainGame = (MainGame)game;
            playSceneSpriteBatch = mainGame._spriteBatch;
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            fighterAircraftTexture = mainGame.Content.Load<Texture2D>("images/fighterAircraft");
            upperScreenRectangle = new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            bottomScreenRectangle = new Rectangle(0, -(int)Shared.stageSize.Y, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            fighterAircraftStartingPosition = new Vector2(Shared.stageSize.X / 2, Shared.stageSize.Y);

            InitializeAircraftDirections();

            fighterAircraft = new FighterAircraft(mainGame, playSceneSpriteBatch, fighterAircraftTexture, fighterAircraftStartingPosition);
            FighterAircraftCurrentPosition = fighterAircraftStartingPosition;
            ComponentList.Add(fighterAircraft);
            //fighterAircraft.Show();

            bossHelicopter = new BossHelicopter(mainGame, playSceneSpriteBatch);
            ComponentList.Add(bossHelicopter);
            //bossHelicopter.Show();

            collisionManager = new CollisionManager(mainGame, this, bossHelicopter, fighterAircraft);
            ComponentList.Add(collisionManager);

            smallHelicopterList = new List<SmallHelicopter>();
        }
        private void InitializeAircraftDirections()
        {
            aircraftDirectionsWithSpeed = new Dictionary<AircraftFrames, Vector2>
            {
                { AircraftFrames.Idle, new Vector2(0, 0) },
                { AircraftFrames.MoveLeftSlow, new Vector2(-1 * 2, 0) },
                { AircraftFrames.MoveLeftFast, new Vector2(-2 * 2, 0) },
                { AircraftFrames.MoveRightSlow, new Vector2(1 * 2, 0) },
                { AircraftFrames.MoveRightFast, new Vector2(2 * 2, 0) },
                { AircraftFrames.MoveUpSlow, new Vector2(0, -1 * 2) },
                { AircraftFrames.MoveUpFast, new Vector2(0, -2 * 2) },
                { AircraftFrames.MoveDownSlow, new Vector2(0, 1 * 2) },
                { AircraftFrames.MoveDownFast, new Vector2(0, 2 * 2) },
                { AircraftFrames.MoveNorthwestSlow, new Vector2(-0.7071f * 2, -0.7071f * 2) },
                { AircraftFrames.MoveNorthwestFast, new Vector2(-1.4142f * 2, -1.4142f * 2) },
                { AircraftFrames.MoveNortheastSlow, new Vector2(0.7071f * 2, -0.7071f * 2) },
                { AircraftFrames.MoveNortheastFast, new Vector2(1.4142f * 2, -1.4142f * 2) },
                { AircraftFrames.MoveSouthwestSlow, new Vector2(-0.7071f * 2, 0.7071f * 2) },
                { AircraftFrames.MoveSouthwestFast, new Vector2(-1.4142f * 2, 1.4142f * 2) },
                { AircraftFrames.MoveSoutheastSlow, new Vector2(0.7071f * 2, 0.7071f * 2) },
                { AircraftFrames.MoveSoutheastFast, new Vector2(1.4142f * 2, 1.4142f * 2) }
            };

            directionDurations = new Dictionary<AircraftFrames, double>
            {
                { AircraftFrames.MoveLeftSlow, 0 },
                { AircraftFrames.MoveRightSlow, 0 },
                { AircraftFrames.MoveUpSlow, 0 },
                { AircraftFrames.MoveDownSlow, 0 },
                { AircraftFrames.MoveNorthwestSlow, 0 },
                { AircraftFrames.MoveNortheastSlow, 0 },
                { AircraftFrames.MoveSouthwestSlow, 0 },
                { AircraftFrames.MoveSoutheastSlow, 0 }
            };
        }

        private AircraftFrames DetermineCurrentFrameAndInitilizeOthers(AircraftFrames aircraftFrames)
        {
            foreach (AircraftFrames item in directionDurations.Keys)
            {
                if (item != aircraftFrames)
                {
                    directionDurations[item] = 0;
                }
            }
            if (directionDurations.ContainsKey(aircraftFrames) && directionDurations[aircraftFrames] >= 0.15)
            {
                switch (aircraftFrames)
                {
                    case AircraftFrames.MoveLeftSlow:
                        return AircraftFrames.MoveLeftFast;
                    case AircraftFrames.MoveRightSlow:
                        return AircraftFrames.MoveRightFast;
                    case AircraftFrames.MoveUpSlow:
                        return AircraftFrames.MoveUpFast;
                    case AircraftFrames.MoveDownSlow:
                        return AircraftFrames.MoveDownFast;
                    case AircraftFrames.MoveNorthwestSlow:
                        return AircraftFrames.MoveNorthwestFast;
                    case AircraftFrames.MoveNortheastSlow:
                        return AircraftFrames.MoveNortheastFast;
                    case AircraftFrames.MoveSouthwestSlow:
                        return AircraftFrames.MoveSouthwestFast;
                    case AircraftFrames.MoveSoutheastSlow:
                        return AircraftFrames.MoveSoutheastFast;
                    default:
                        return aircraftFrames;
                }
            }

            return aircraftFrames;
        }


        //aircraft bullet
        private double lastBulletTime = 0;
        private double aircraftBulletCooldown = 200;
        //boss bullet
        private double bossBulletElapsedTime;
        private double generatingBossBasicBulletElapsedTime;
        private double stoppingBossBasicBulletElapsedTime;
        private bool isGeneratingBossBulletTime;
        private double bossBulletGeneratingSequence = 800;
        //small helopcoter

        private double smallHelicopterElapsedTime;
        private double smallHelicopterGeneratingSequence = 3000;

        public override void Update(GameTime gameTime)
        {
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Space) && !PlayScene.IsStartingSequence)
            {
                if (currentTime - lastBulletTime > aircraftBulletCooldown)
                {
                    AircraftBasicBullet aircraftBasicBullet = new AircraftBasicBullet(mainGame, playSceneSpriteBatch);
                    aircraftBasicBullet.RemoveBulletDelegate = RemoveAircraftBullet;
                    ComponentList.Add(aircraftBasicBullet);

                    lastBulletTime = currentTime;
                }
                void RemoveAircraftBullet(AircraftBasicBullet aircraftBasicBullet)
                {
                    ComponentList.Remove(aircraftBasicBullet);
                    aircraftBasicBullet.Dispose();
                }
            }

            //generating bullet time control logic
            generatingBossBasicBulletElapsedTime = isGeneratingBossBulletTime == false ? generatingBossBasicBulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds : 0;

            if (generatingBossBasicBulletElapsedTime > 3000)
            {
                isGeneratingBossBulletTime = true;
            }
            if (stoppingBossBasicBulletElapsedTime > 3000)
            {
                isGeneratingBossBulletTime = false;
                stoppingBossBasicBulletElapsedTime = 0;
            }

            //generating boss basic bullet
            if (isGeneratingBossBulletTime)
            {
                stoppingBossBasicBulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                //generating bullet sequence logic
                bossBulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (bossBulletElapsedTime > bossBulletGeneratingSequence && !bossHelicopter.IsStartSequence)
                {
                    BossHelicopterBasicBullet aircraftBasicBullet = new BossHelicopterBasicBullet(mainGame, playSceneSpriteBatch);
                    aircraftBasicBullet.RemoveBossBulletDelegate = RemoveBossBasictBullet;
                    ComponentList.Add(aircraftBasicBullet);
                    bossBulletElapsedTime = 0;
                }
                void RemoveBossBasictBullet(BossHelicopterBasicBullet bossHelicopterBasicBullet)
                {
                    ComponentList.Remove(bossHelicopterBasicBullet);
                    bossHelicopterBasicBullet.Dispose();
                }
            }

            //Background scrolling
            upperScreenRectangle.Y += backgroundTextureScrollSpeed;
            bottomScreenRectangle.Y += backgroundTextureScrollSpeed;
            if (upperScreenRectangle.Y >= (int)Shared.stageSize.Y)
            {
                upperScreenRectangle.Y = 0;
            }
            if (bottomScreenRectangle.Y >= 0)
            {
                bottomScreenRectangle.Y = -(int)Shared.stageSize.Y;
            }

            //Control aircraft loading time
            if (IsStartingSequence)
            {
                fighterAircraftCurrentPosition.Y -= fighterAircraftEntrySpeed;

                if (FighterAircraftCurrentPosition.Y == fighterAircraftFullyLoadedYPosition)
                {
                    IsStartingSequence = false;
                }
                fighterAircraft.ChangeAirCraftPositionAndAnimationWithSpeed(AircraftFrames.Idle, FighterAircraftCurrentPosition);
            }
            // aircraft control logic
            else//!IsStartingSequence
            {
                currentFrame = AircraftFrames.Idle;

                bool left = keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A);
                bool right = keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D);
                bool up = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W);
                bool down = keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S);


                if (left && up)
                {
                    directionDurations[AircraftFrames.MoveNorthwestSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveNorthwestSlow);
                }
                else if (left && down)
                {
                    directionDurations[AircraftFrames.MoveSouthwestSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveSouthwestSlow);
                }
                else if (right && up)
                {
                    directionDurations[AircraftFrames.MoveNortheastSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveNortheastSlow);
                }
                else if (right && down)
                {
                    directionDurations[AircraftFrames.MoveSoutheastSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveSoutheastSlow);
                }
                else if (left)
                {
                    directionDurations[AircraftFrames.MoveLeftSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveLeftSlow);
                }
                else if (right)
                {
                    directionDurations[AircraftFrames.MoveRightSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveRightSlow);
                }
                else if (up)
                {
                    directionDurations[AircraftFrames.MoveUpSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveUpSlow);
                }
                else if (down)
                {
                    directionDurations[AircraftFrames.MoveDownSlow] += gameTime.ElapsedGameTime.TotalSeconds;
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.MoveDownSlow);
                }
                else
                {
                    currentFrame = DetermineCurrentFrameAndInitilizeOthers(AircraftFrames.Idle);
                }
                FighterAircraftCurrentPosition += aircraftDirectionsWithSpeed[currentFrame];
                fighterAircraft.ChangeAirCraftPositionAndAnimationWithSpeed(currentFrame, FighterAircraftCurrentPosition);

            }

            //small helicopter
            if (!IsStartingSequence)
            {
                smallHelicopterElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (smallHelicopterElapsedTime > smallHelicopterGeneratingSequence)
                {
                    smallHelicopter = new SmallHelicopter(mainGame, playSceneSpriteBatch, this);
                    ComponentList.Add(smallHelicopter);
                    smallHelicopterElapsedTime = 0;

                    smallHelicopter.RemovePassedOrExpolosed += RemoveAircraftBullet;
                    void RemoveAircraftBullet(SmallHelicopter smallHelicopter)
                    {
                        ComponentList.Remove(smallHelicopter);
                        SmallHelicopterList.Remove(smallHelicopter);
                        smallHelicopter.Dispose();
                    }
                }
            }

            base.Update(gameTime);

        }
        public override void Draw(GameTime gameTime)
        {
            playSceneSpriteBatch.Begin();
            playSceneSpriteBatch.Draw(backgroundTexture, upperScreenRectangle, Color.White);
            playSceneSpriteBatch.Draw(backgroundTexture, bottomScreenRectangle, Color.White);

            playSceneSpriteBatch.End();
            base.Draw(gameTime);
        }



    }
}
