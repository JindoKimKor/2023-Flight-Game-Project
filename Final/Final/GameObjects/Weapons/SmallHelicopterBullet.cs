using Final.GameObjects.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Final.GameObjects.Weapons
{
    public class SmallHelicopterBullet : DrawableGameComponent
    {
        private MainGame mainGame;
        // SpriteBatch and texture for rendering the bullet
        private SpriteBatch bulletSpriteBatch;
        private Texture2D bulletTexture;

        // Animation frames and dimensions
        private Vector2 bulletFrameSize;
        private List<Rectangle> bulletAnimationFrames;
        private const int BULLET_COLS = 4;

        // Current state of the bullet
        private int frameIndex = 0;
        private Vector2 currentPosition;
        private Vector2 bulletDirection;
        private Vector2 textureOrigin;
        private const float BULLET_SPEED = 4.5f;

        // Animation control
        private double frameElapsedTime = 0;
        private double frameInterval = 100;

        public SmallHelicopterBullet(Game game, SpriteBatch playSceneSpriteBatch, SmallHelicopter smallHelicopter) : base(game)
        {
            mainGame = (MainGame)game;
            bulletSpriteBatch = playSceneSpriteBatch;
            bulletTexture = mainGame.Content.Load<Texture2D>("images/smallHelicopterBullet");

            InitializeAnimationFrames();

            currentPosition = smallHelicopter.CurrentPosition;
            textureOrigin = new Vector2(bulletFrameSize.X / 2, bulletFrameSize.Y / 2);
            bulletDirection = Vector2.Normalize(FighterAircraft.AircraftCurrentPosition - currentPosition);

            void InitializeAnimationFrames()
            {
                bulletFrameSize = new Vector2(bulletTexture.Width / BULLET_COLS, bulletTexture.Height);
                bulletAnimationFrames = new List<Rectangle>();
                for (int c = 0; c < BULLET_COLS; c++)
                {
                    int x = c * (int)bulletFrameSize.X;
                    bulletAnimationFrames.Add(new Rectangle(x, 0, (int)bulletFrameSize.X, (int)bulletFrameSize.Y));
                }
            }
        }


        public override void Update(GameTime gameTime)
        {
            UpdateAnimationFrame();
            UpdateBulletPosition();

            base.Update(gameTime);

            void UpdateAnimationFrame()
            {
                frameElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (frameElapsedTime >= frameInterval)
                {
                    frameIndex++;
                    frameElapsedTime = 0;
                    if (frameIndex >= BULLET_COLS - 1)
                    {
                        frameIndex = 0;
                    }
                }
            }

            void UpdateBulletPosition()
            {
                currentPosition += bulletDirection * BULLET_SPEED;
            }
        }




        public override void Draw(GameTime gameTime)
        {
            bulletSpriteBatch.Begin();
            bulletSpriteBatch.Draw(bulletTexture, currentPosition, bulletAnimationFrames[frameIndex], Color.White, 0f, textureOrigin, 0.2f, SpriteEffects.None, 0f);
            bulletSpriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// To get small helicopter bullet hit box
        /// </summary>
        /// <returns>small helicopter bullet Texture's Frame Positions and Size</returns>
        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(bulletFrameSize.X * 0.2f);
            int scaledHeight = (int)(bulletFrameSize.Y * 0.2f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
