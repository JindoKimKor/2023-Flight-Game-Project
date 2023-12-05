using Final.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Final.Scenes
{
    public class PlayScene : GameScene
    {
        private SpriteBatch playSceneSpriteBatch;
        private Texture2D backgroundTexture;
        private Rectangle screenRectangleOne;
        private Rectangle screenRectangleTwo;
        private int backgroundTextureScrollSpeed = 3;

        private FighterAircraft fighterAircraft;
        private BossHelicopter bossHelicopter;

        private Texture2D fighterAircraftTexture;
        private static bool isStartingSequence = true;
        private Vector2 fighterAircraftStartingPosition;
        private float fighterAircraftFullyLoadedYPosition = Shared.stageSize.Y - 250;
        private static Vector2 fighterAircraftCurrentPosition;
        private int fighterAircraftEntrySpeed = 2;
        private AircraftFrames currentFrame;

        private MainGame mainGame;

        Dictionary<AircraftFrames, Vector2> aircraftDirectionsWithSpeed;
        private Dictionary<AircraftFrames, double> directionDurations;


        public static bool IsStartingSequence { get => isStartingSequence; set => isStartingSequence = value; }
        public static Vector2 FighterAircraftCurrentPosition { get => fighterAircraftCurrentPosition; set => fighterAircraftCurrentPosition = value; }

        public PlayScene(Game game) : base(game)
        {
            mainGame = (MainGame)game;
            playSceneSpriteBatch = mainGame._spriteBatch;
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            fighterAircraftTexture = mainGame.Content.Load<Texture2D>("images/fighterAircraft");
            screenRectangleOne = new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            screenRectangleTwo = new Rectangle(0, -(int)Shared.stageSize.Y, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            fighterAircraftStartingPosition = new Vector2(Shared.stageSize.X / 2, Shared.stageSize.Y);

            InitializeAircraftDirections();

            fighterAircraft = new FighterAircraft(mainGame, playSceneSpriteBatch, fighterAircraftTexture, fighterAircraftStartingPosition);
            FighterAircraftCurrentPosition = fighterAircraftStartingPosition;
            ComponentList.Add(fighterAircraft);
            fighterAircraft.Show();

            bossHelicopter = new BossHelicopter(mainGame, playSceneSpriteBatch);
            ComponentList.Add(bossHelicopter);
            bossHelicopter.Show();

        }
        private void InitializeAircraftDirections()
        {
            aircraftDirectionsWithSpeed = new Dictionary<AircraftFrames, Vector2>
            {
                { AircraftFrames.Idle, new Vector2(0, 0) },
                { AircraftFrames.MoveLeftSlow, new Vector2(-2, 0) },
                { AircraftFrames.MoveLeftFast, new Vector2(-4, 0) },
                { AircraftFrames.MoveRightSlow, new Vector2(2, 0) },
                { AircraftFrames.MoveRightFast, new Vector2(4, 0) },
                { AircraftFrames.MoveUpSlow, new Vector2(0, -2) },
                { AircraftFrames.MoveUpFast, new Vector2(0, -4) },
                { AircraftFrames.MoveDownSlow, new Vector2(0, 2) },
                { AircraftFrames.MoveDownFast, new Vector2(0, 4) },
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

        private double lastBulletTime = 0;
        private double bulletCooldown = 200;

        public override void Update(GameTime gameTime)
        {
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Space) && !PlayScene.IsStartingSequence)
            {
                if (currentTime - lastBulletTime > bulletCooldown)
                {
                    AircraftBasicBullet aircraftBasicBullet = new AircraftBasicBullet(mainGame, playSceneSpriteBatch);
                    aircraftBasicBullet.RemoveBulletDelegate = RemoveBullet;
                    ComponentList.Add(aircraftBasicBullet);

                    lastBulletTime = currentTime;
                }
            }
            void RemoveBullet(AircraftBasicBullet bullet)
            {
                bullet.Dispose();
                ComponentList.Remove(bullet);
            }

            screenRectangleOne.Y += backgroundTextureScrollSpeed;
            screenRectangleTwo.Y += backgroundTextureScrollSpeed;
            if (screenRectangleOne.Y >= (int)Shared.stageSize.Y)
            {
                screenRectangleOne.Y = 0;
            }
            if (screenRectangleTwo.Y >= 0)
            {
                screenRectangleTwo.Y = -(int)Shared.stageSize.Y;
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
            base.Update(gameTime);

        }
        public override void Draw(GameTime gameTime)
        {
            playSceneSpriteBatch.Begin();
            playSceneSpriteBatch.Draw(backgroundTexture, screenRectangleOne, Color.White);
            playSceneSpriteBatch.Draw(backgroundTexture, screenRectangleTwo, Color.White);

            playSceneSpriteBatch.End();
            base.Draw(gameTime);
        }



    }
}
