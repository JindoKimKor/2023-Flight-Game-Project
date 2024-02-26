using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Final.GameObjects.Characters
{
    /// <summary>
    /// Boss Helicopter Class
    /// </summary>
    public class BossHelicopter : DrawableGameComponent
    {
        public enum BossStage
        {
            firstStage,
            secondStage,
            destroyed
        }

        private MainGame mainGame;
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
        private int secondStageHealth = 20;
        private int maxHealth = 40;
        private bool isHit;
        private int hitCount;
        public BossStage CurrentStage;

        public bool IsStartSequence { get => isStartSequence; set => isStartSequence = value; }
        public static Vector2 BossCurrentPosition { get => bossCurrentPosition; set => bossCurrentPosition = value; }
        public bool IsHit { get => isHit; set => isHit = value; }

        // Properties for helicopter movement and behavior and destruction
        private double timerNewXCoordinate = 0;
        private double frameInterval = 1000;
        private float newXCoordinate;
        private Random random = new Random();
        private double timerShake = 0;
        private double intervalShake = 50;
        private SoundEffect destructionSound;
        private double timerDestruction = 0;
        private double intervalDestructionSound = 1200;


        // Hit effect timer
        private double timerHitEffect = 0.005;

        /// <summary>
        /// Boss Helicopter Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playSceneSpriteBatch"></param>
        public BossHelicopter(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {
            mainGame = (MainGame)game;
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
            destructionSound = mainGame.Content.Load<SoundEffect>("sounds/destroyedSound");

        }

        public override void Update(GameTime gameTime)
        {

            HandleEntrySequence();

            AdjustXPositionBasedOnAircraft();

            ShakeIfDestroyed();

            ChangeStageBasedOnHealth();

            base.Update(gameTime);

            void HandleEntrySequence()
            {
                Action<int> changeStartSequence = (x) => { if (BossCurrentPosition.Y >= x) IsStartSequence = false; };

                currentAnimationFrameIndex = currentAnimationFrameIndex == animationFramesAlive.Count() - 1 ? 0 : ++currentAnimationFrameIndex;
                bossCurrentPosition.Y = IsStartSequence == true ? BossCurrentPosition.Y + ENTRY_SPEED : BossCurrentPosition.Y;

                changeStartSequence(FINAL_Y_ENTRY_POSITION);
            }

            void AdjustXPositionBasedOnAircraft()
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
            void ShakeIfDestroyed()
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

                    timerDestruction += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timerDestruction >= intervalDestructionSound)
                    {
                        destructionSound.Play();
                        timerDestruction = 0;
                    }
                }
            }
            void ChangeStageBasedOnHealth()
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

                if (CurrentStage == BossStage.firstStage)
                {
                    spriteBatch.Draw(textureFirstStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.Red, 0f, originTexture, 0.91f, SpriteEffects.None, 0f);
                }
                else if (CurrentStage == BossStage.secondStage)
                {
                    spriteBatch.Draw(textureSecondStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.Red, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }
            }
            else
            {
                if (CurrentStage == BossStage.firstStage)
                {
                    spriteBatch.Draw(textureFirstStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.White, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }
                else if (CurrentStage == BossStage.secondStage)
                {
                    spriteBatch.Draw(textureSecondStage, BossCurrentPosition, animationFramesAlive[currentAnimationFrameIndex], Color.White, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }
                else if (CurrentStage == BossStage.destroyed)
                {
                    IsHit = !IsHit;//flash effect
                    spriteBatch.Draw(textureDestroyedStage, BossCurrentPosition, new Rectangle(0, 0, textureDestroyedStage.Width, textureDestroyedStage.Height), Color.White, 0f, originTexture, 0.9f, SpriteEffects.None, 0f);
                }

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// To get boss helicopter hit box
        /// </summary>
        /// <returns>Boss Texture's Frame Positions and Size</returns>
        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(frameDimensionAlive.X * 0.9f);
            int scaledHeight = (int)(frameDimensionAlive.Y * 0.9f);

            return new Rectangle((int)BossCurrentPosition.X, (int)BossCurrentPosition.Y, scaledWidth, scaledHeight);
        }

    }
}
