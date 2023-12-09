﻿using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.GameComponents
{
    public enum AircraftFrames
    {
        Idle = 12,
        MoveLeftSlow = 11,
        MoveLeftFast = 10,
        MoveRightSlow = 13,
        MoveRightFast = 14,
        MoveUpSlow = 7,
        MoveUpFast = 2,
        MoveDownSlow = 17,
        MoveDownFast = 22,
        MoveNorthwestSlow = 6,
        MoveNorthwestFast = 0,
        MoveNortheastSlow = 8,
        MoveNortheastFast = 4,
        MoveSouthwestSlow = 16,
        MoveSouthwestFast = 20,
        MoveSoutheastSlow = 19,
        MoveSoutheastFast = 24,
    }

    public class FighterAircraft : DrawableGameComponent
    {
        // SpriteBatch and texture for rendering
        private SpriteBatch spriteBatch;
        private Texture2D aircraftTexture;
        private Texture2D attackAnimationTexture;

        // Animation frames and dimensions
        private Vector2 aircraftFrameSize;
        private Vector2 attackAnimationFrameSize;
        private List<Rectangle> animationFrames;
        private List<Rectangle> fireAnimationFrames;
        private AircraftFrames currentFrame;

        // Position and movement properties
        private static Vector2 aircraftCurrentPosition;
        private Vector2 textureOrigin;
        private Vector2 attackAnimationPosition;

        // Animation control
        private int attackAnimationFrameIndex;
        private double attackAnimationElapsedTime;
        private double attackAnimationFrameInterval = 100;

        // Constants for animation and texture
        private const int AIRCRAFT_TEXTURE_ROWS = 5;
        private const int AIRCRAFT_TEXTURE_COLS = 5;
        private const int FIRE_TEXTURE_ROWS = 5;

        // Aircraft movement and entry
        private int aircraftEntrySpeed = 2;
        private float aircraftFullyLoadedYPosition = Shared.stageSize.Y - 250;
        private static bool isInEntrySequence;

        // Aircraft direction and speed mappings
        private Dictionary<AircraftFrames, Vector2> aircraftDirectionsWithSpeed;
        private Dictionary<AircraftFrames, double> directionDurations;

        // Hit effect properties
        private bool isGotHit;
        private int hitCounter;
        private double hitEffectDuration = 0.005;

        // Main game reference
        private MainGame mainGame;

        public bool IsGotHit { get => isGotHit; set => isGotHit = value; }
        public int HitCounter { get => hitCounter; set => hitCounter = value; }

        public static bool IsInEntrySequence { get => isInEntrySequence; set => isInEntrySequence = value; }

        public static Vector2 AircraftCurrentPosition { get => aircraftCurrentPosition; set => aircraftCurrentPosition = value; }

        public FighterAircraft(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {
            // Main game reference and sprite batch initialization
            mainGame = (MainGame)game;
            spriteBatch = playSceneSpriteBatch;

            // Load textures and initialize dimensions
            LoadTexturesAndInitializeDimensions();

            // Initialize aircraft position and state
            InitializeAircraftPositionAndState();

            // Initialize animation frames for aircraft and fire
            InitializeAnimationFrames();

            // Initialize directions and speeds for aircraft movement
            InitializeAircraftDirectionsWithSpeed();
        }

        private void LoadTexturesAndInitializeDimensions()
        {
            aircraftTexture = mainGame.Content.Load<Texture2D>("images/fighterAircraft");
            aircraftFrameSize = new Vector2(aircraftTexture.Width / AIRCRAFT_TEXTURE_ROWS, aircraftTexture.Height / AIRCRAFT_TEXTURE_COLS);
            textureOrigin = new Vector2(aircraftFrameSize.X / 2, aircraftFrameSize.Y / 2);

            attackAnimationTexture = mainGame.Content.Load<Texture2D>("images/aircraftAttackAnimation");
            attackAnimationFrameSize = new Vector2(attackAnimationTexture.Width, attackAnimationTexture.Height / FIRE_TEXTURE_ROWS);
            attackAnimationFrameIndex = (FIRE_TEXTURE_ROWS - 1);
        }

        private void InitializeAircraftPositionAndState()
        {
            AircraftCurrentPosition = new Vector2(Shared.stageSize.X / 2, Shared.stageSize.Y);
            currentFrame = AircraftFrames.Idle;
            isInEntrySequence = true;
        }

        private void InitializeAnimationFrames()
        {
            // Initialize frames for aircraft movement
            animationFrames = new List<Rectangle>();
            for (int r = 0; r < AIRCRAFT_TEXTURE_ROWS; r++)
            {
                for (int c = 0; c < AIRCRAFT_TEXTURE_COLS; c++)
                {
                    int x = c * (int)aircraftFrameSize.X;
                    int y = r * (int)aircraftFrameSize.Y;
                    animationFrames.Add(new Rectangle(x, y, (int)aircraftFrameSize.X, (int)aircraftFrameSize.Y));
                }
            }

            // Initialize frames for fire animation
            fireAnimationFrames = new List<Rectangle>();
            for (int r = 0; r < FIRE_TEXTURE_ROWS; r++)
            {
                int y = r * (int)attackAnimationFrameSize.Y;
                fireAnimationFrames.Add(new Rectangle(0, y, (int)attackAnimationFrameSize.X, (int)attackAnimationFrameSize.Y));
            }
        }

        private void InitializeAircraftDirectionsWithSpeed()
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
        public override void Update(GameTime gameTime)
        {

            HandleAircraftEntry(gameTime);
            HandleAircraftMovement(gameTime);
            UpdateFireAnimation(gameTime);


            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            spriteBatch.Begin();

            if (IsGotHit)
            {
                hitEffectDuration -= gameTime.ElapsedGameTime.TotalSeconds;
                if (hitEffectDuration <= 0)
                {
                    IsGotHit = false;
                    hitCounter++;
                }
                spriteBatch.Draw(aircraftTexture, AircraftCurrentPosition, animationFrames[(int)currentFrame], Color.Red, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(aircraftTexture, AircraftCurrentPosition, animationFrames[(int)currentFrame], Color.White, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !IsInEntrySequence)
            {
                spriteBatch.Draw(attackAnimationTexture, attackAnimationPosition, fireAnimationFrames[attackAnimationFrameIndex], Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleAircraftEntry(GameTime gameTime)
        {
            //Control aircraft loading time
            if (IsInEntrySequence)
            {
                aircraftCurrentPosition.Y -= aircraftEntrySpeed;

                if (AircraftCurrentPosition.Y == aircraftFullyLoadedYPosition)
                {
                    IsInEntrySequence = false;
                }
                ChangeAirCraftPositionAndAnimationWithSpeed(AircraftFrames.Idle, AircraftCurrentPosition);
            }
        }

        private void HandleAircraftMovement(GameTime gameTime)
        {
            if (!IsInEntrySequence)
            {
                KeyboardState keyboardState = Keyboard.GetState();

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
                AircraftCurrentPosition += aircraftDirectionsWithSpeed[currentFrame];
                ChangeAirCraftPositionAndAnimationWithSpeed(currentFrame, AircraftCurrentPosition);
            }
        }

        private void UpdateFireAnimation(GameTime gameTime)
        {
            attackAnimationElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (attackAnimationElapsedTime >= attackAnimationFrameInterval)
            {
                attackAnimationFrameIndex--;
                if (attackAnimationFrameIndex <= 1)
                {
                    attackAnimationFrameIndex = (FIRE_TEXTURE_ROWS - 1);
                }
                attackAnimationElapsedTime = 0;
            }

            attackAnimationPosition = AircraftCurrentPosition;
            //adjust position according to its presentation
            attackAnimationPosition.X = attackAnimationPosition.X - 22f;
            attackAnimationPosition.Y = attackAnimationPosition.Y - 50f;
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

        public void ChangeAirCraftPositionAndAnimationWithSpeed(AircraftFrames newFrame, Vector2 newPosition)
        {
            float screenEdgeMinimumX = (aircraftFrameSize.X / 2);
            float screenEdgeMinimumY = (aircraftFrameSize.Y / 2);
            float screenEdgeMaxX = Shared.stageSize.X - (aircraftFrameSize.X / 2);
            float screenEdgeMaxY = Shared.stageSize.Y - (aircraftFrameSize.Y / 2);

            currentFrame = newFrame;
            //To keep the aircraft within the screen
            if (!IsInEntrySequence)
            {
                AircraftCurrentPosition = new Vector2(
                    newPosition.X <= screenEdgeMinimumX ? screenEdgeMinimumX : newPosition.X >= screenEdgeMaxX ? screenEdgeMaxX : newPosition.X,
                    newPosition.Y <= screenEdgeMinimumY ? screenEdgeMinimumY : newPosition.Y >= screenEdgeMaxY ? screenEdgeMaxY : newPosition.Y
                );
            }
        }



        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(aircraftFrameSize.X * 0.8f);
            int scaledHeight = (int)(aircraftFrameSize.Y * 0.8f);

            return new Rectangle((int)AircraftCurrentPosition.X, (int)AircraftCurrentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
