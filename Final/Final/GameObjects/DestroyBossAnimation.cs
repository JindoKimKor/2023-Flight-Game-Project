using Final.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Final.GameObjects
{
    /// <summary>
    /// Responsible for boss ending part animation
    /// </summary>
    public class DestroyBossAnimation : DrawableGameComponent
    {
        // SpriteBatch and Texture
        private SpriteBatch spriteBatch;
        private Texture2D destroyAnimationTexture;

        // Animation Fields
        private Vector2 destroyAnimationFrameDimension;
        private const int DESTROY_ANIMATION_COLS = 7;
        private List<Rectangle> destroyAnimationFrames;
        private int destroyAnimationFrameIndex = 0;
        private Vector2 destroyAnimationPosition;
        private Vector2 originTexture;

        // Timing Fields
        private float destroyingAnimationSpawningElapsedTime = 0;
        /// <summary>
        /// Destroy Boss Animation Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playSceneSpriteBatch"></param>
        public DestroyBossAnimation(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {

            MainGame mainGame = (MainGame)game;
            spriteBatch = playSceneSpriteBatch;

            InitializeFields();

            GenerateRandomAnimationPosition();

            void InitializeFields()
            {
                destroyAnimationTexture = mainGame.Content.Load<Texture2D>("images/destroyAnimation");
                destroyAnimationFrameDimension = new Vector2(destroyAnimationTexture.Width / DESTROY_ANIMATION_COLS, destroyAnimationTexture.Height);
                destroyAnimationFrames = new List<Rectangle>();

                for (int c = 0; c < DESTROY_ANIMATION_COLS; c++)
                {
                    int x = c * (int)destroyAnimationFrameDimension.X;
                    destroyAnimationFrames.Add(new Rectangle(x, 0, (int)destroyAnimationFrameDimension.X, (int)destroyAnimationFrameDimension.Y));
                }
                originTexture = new Vector2(destroyAnimationFrameDimension.X / 2, destroyAnimationFrameDimension.Y / 2);
            }

            void GenerateRandomAnimationPosition()
            {
                Random random = new Random();
                destroyAnimationPosition = new Vector2(
                    random.Next((int)BossHelicopter.BossCurrentPosition.X - 50, (int)BossHelicopter.BossCurrentPosition.X + 50),
                    random.Next((int)BossHelicopter.BossCurrentPosition.Y - 50, (int)BossHelicopter.BossCurrentPosition.Y + 50));
            }
        }


        public override void Update(GameTime gameTime)
        {
            UpdateAnimation();

            base.Update(gameTime);

            void UpdateAnimation()
            {
                destroyingAnimationSpawningElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (destroyingAnimationSpawningElapsedTime >= 0.3f && destroyAnimationFrameIndex < DESTROY_ANIMATION_COLS - 1)
                {
                    destroyAnimationFrameIndex++;
                    destroyingAnimationSpawningElapsedTime = 0f;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (destroyAnimationFrameIndex < DESTROY_ANIMATION_COLS - 1)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(destroyAnimationTexture, destroyAnimationPosition, destroyAnimationFrames[destroyAnimationFrameIndex], Color.White, 0f, originTexture, 1.2f, SpriteEffects.None, 0f);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
