﻿using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Final.GameComponents
{

    public class BossHelicopter : DrawableGameComponent
    {
        public enum BossStage
        {
            firstStage,
            secondStage,
            destroyed
        }
        // SpriteBatch for rendering the boss helicopter
        private SpriteBatch spriteBatch;

        // Textures for different stages of the boss helicopter
        private Texture2D textureFirstStage;
        private Texture2D textureSecondStage;
        private Texture2D textureDestroyedStage;

        // Animation frame properties for the alive boss
        private Vector2 frameDimensionAlive;
        private List<Rectangle> animationFramesAlive;
        private const int ALIVE_BOSS_COLS = 4;
        private static Vector2 bossCurrentPosition;
        private Vector2 originTexture;
        private int currentAnimationFrameIndex = -1;
        private bool isStartSequence;
        private const float ENTRY_SPEED = 0.7f;
        private const int FINAL_Y_ENTRY_POSITION = 100;

        // Health and hit properties
        private bool isHit;
        private int maxHealth = 10;
        private int secondStageHealth = 5;
        private int hitCount;
        public BossStage CurrentStage;

        public bool IsStartSequence { get => isStartSequence; set => isStartSequence = value; }
        public static Vector2 BossCurrentPosition { get => bossCurrentPosition; set => bossCurrentPosition = value; }
        public bool IsHit { get => isHit; set => isHit = value; }

        // Properties for helicopter movement and behavior
        private double timerNewXCoordinate = 0;
        private double frameInterval = 1000;
        private float newXCoordinate;
        private Random random = new Random();
        private double timerShake = 0;
        private double intervalShake = 50;

        // Hit effect timer
        private double timerHitEffect = 0.005;

        public BossHelicopter(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {

            // Setting up the SpriteBatch
            spriteBatch = playSceneSpriteBatch;

            // Setting the initial position of the boss helicopter
            BossCurrentPosition = new Vector2(Shared.stageSize.X / 2, -frameDimensionAlive.Y);

            // Loading textures for each stage of the boss
            LoadTextures();

            // Setting up the animation frames for the boss helicopter
            InitializeAnimationFrames();

            // Setting the current stage of the boss
            CurrentStage = BossStage.firstStage;
            void LoadTextures()
            {
                textureFirstStage = game.Content.Load<Texture2D>("images/firstStageBossHelicopter");
                textureSecondStage = game.Content.Load<Texture2D>("images/secondStageBossHelicopter");
                textureDestroyedStage = game.Content.Load<Texture2D>("images/deadBossHelicopter");
                frameDimensionAlive = new Vector2(textureFirstStage.Width / ALIVE_BOSS_COLS, textureFirstStage.Height);
                originTexture = new Vector2(frameDimensionAlive.X / 2, frameDimensionAlive.Y / 2);
            }
            void InitializeAnimationFrames()
            {
                IsStartSequence = true;
                animationFramesAlive = new List<Rectangle>();

                for (int c = 0; c < ALIVE_BOSS_COLS; c++)
                {
                    int x = c * (int)frameDimensionAlive.X;
                    animationFramesAlive.Add(new Rectangle(x, 0, (int)frameDimensionAlive.X, (int)frameDimensionAlive.Y));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {

            HandleEntrySequence();

            AdjustXPositionBasedOnAircraft(gameTime);

            ShakeIfDestroyed(gameTime);

            ChangeStageBasedOnHealth();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();
            if (IsHit)
            {
                timerHitEffect -= gameTime.ElapsedGameTime.TotalSeconds;
                if (timerHitEffect <= 0)
                {
                    IsHit = false;
                    hitCount++;
                }
                if (hitCount >= secondStageHealth && hitCount < maxHealth)
                {
                    spriteBatch.Draw(textureSecondStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.Red, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }

                else if (hitCount >= 0 && hitCount < secondStageHealth)
                {
                    spriteBatch.Draw(textureFirstStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.Red, 0f, originTexture, 0.91f, SpriteEffects.None, 0f);
                }

            }
            else
            {
                if (hitCount >= secondStageHealth && hitCount < maxHealth)
                {
                    spriteBatch.Draw(textureSecondStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.White, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }
                else if (hitCount >= 0 && hitCount < secondStageHealth)
                {
                    spriteBatch.Draw(textureFirstStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.White, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }
                else if (hitCount >= maxHealth)
                {
                    IsHit = !IsHit;//flash effect
                    spriteBatch.Draw(textureDestroyedStage, BossCurrentPosition, new Rectangle(0, 0, textureDestroyedStage.Width, textureDestroyedStage.Height), Color.White, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void HandleEntrySequence()
        {
            Action<int> changeStartSequence = (x) => { if (BossCurrentPosition.Y >= x) IsStartSequence = false; };

            currentAnimationFrameIndex = currentAnimationFrameIndex == animationFramesAlive.Count() - 1 ? 0 : ++currentAnimationFrameIndex;
            bossCurrentPosition.Y = IsStartSequence == true ? BossCurrentPosition.Y + ENTRY_SPEED : BossCurrentPosition.Y;

            changeStartSequence(FINAL_Y_ENTRY_POSITION);
        }

        private void AdjustXPositionBasedOnAircraft(GameTime gameTime)
        {
            timerNewXCoordinate += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timerNewXCoordinate >= frameInterval && (CurrentStage == BossStage.firstStage || CurrentStage == BossStage.secondStage))
            {
                Vector2 FighterAircraftCurrentPosition = FighterAircraft.AircraftCurrentPosition;
                newXCoordinate = FighterAircraftCurrentPosition.X;
                timerNewXCoordinate = 0;
            }

            if (bossCurrentPosition.X > newXCoordinate)
            {
                bossCurrentPosition.X--;
            }
            else if (bossCurrentPosition.X < newXCoordinate)
            {
                bossCurrentPosition.X++;
            }
        }

        private void ShakeIfDestroyed(GameTime gameTime)
        {
            if (CurrentStage == BossStage.destroyed)
            {
                timerShake += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timerShake >= intervalShake)
                {
                    bossCurrentPosition.X += random.Next(-4, 6);
                    bossCurrentPosition.Y += random.Next(-4, 6);

                    timerShake = 0;
                }
            }
        }

        private void ChangeStageBasedOnHealth()
        {
            if (hitCount >= secondStageHealth && hitCount < maxHealth)
            {
                CurrentStage = BossStage.secondStage;
            }
            else if (hitCount >= maxHealth)
            {
                CurrentStage = BossStage.destroyed;
            }
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(frameDimensionAlive.X * 0.9f);
            int scaledHeight = (int)(frameDimensionAlive.Y * 0.9f);

            return new Rectangle((int)BossCurrentPosition.X, (int)BossCurrentPosition.Y, scaledWidth, scaledHeight);
        }

    }
}
