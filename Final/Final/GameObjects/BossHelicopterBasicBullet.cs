using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.GameComponents
{
    public delegate void RemovePassedMaxYCoordinateBossBullet(BossHelicopterBasicBullet bullet);

    public class BossHelicopterBasicBullet : DrawableGameComponent
    {
        // Main game reference
        private MainGame mainGame;

        // SpriteBatch and texture for bullet rendering
        private SpriteBatch spriteBatch;
        private Texture2D bulletTexture;

        // Animation frames and dimensions
        private Vector2 frameSize;
        private List<Rectangle> bulletAnimationFrames;
        private const int NUM_BULLET_ROWS = 4;

        // Bullet position, direction, and origin
        private Vector2 currentPosition;
        private Vector2 direction;
        private Vector2 textureOrigin;

        // Bullet movement properties
        private float maxYPosition;
        private float bulletSpeed = 6.0f;

        // Animation control
        private int frameIndex = 0;
        private double elapsedTimeFrameChange = 0;
        private double frameChangeInterval = 40;

        // Delegate for removing the bullet when it passes a certain Y coordinate
        public RemovePassedMaxYCoordinateBossBullet RemoveBossBulletDelegate { get; set; }

        public BossHelicopterBasicBullet(Game game, SpriteBatch playSceneSpriteBatch)
    : base(game)
        {
            InitializeBullet(game, playSceneSpriteBatch);
            SetInitialPosition(BossHelicopter.BossCurrentPosition);
        }

        public BossHelicopterBasicBullet(Game game, SpriteBatch playSceneSpriteBatch, string startingPosition)
            : base(game)
        {
            InitializeBullet(game, playSceneSpriteBatch);
            SetPositionBasedOnStartingPoint(startingPosition);
        }

        // Initializes shared bullet properties
        private void InitializeBullet(Game game, SpriteBatch spriteBatch)
        {
            // Set main game and spriteBatch references
            mainGame = (MainGame)game;
            this.spriteBatch = spriteBatch;

            // Load texture and set frame size
            bulletTexture = mainGame.Content.Load<Texture2D>("images/bossHelicopterBasicBullet");
            frameSize = new Vector2(bulletTexture.Width / NUM_BULLET_ROWS, bulletTexture.Height);
            textureOrigin = new Vector2(frameSize.X / 2, frameSize.Y / 2);
            maxYPosition = Shared.stageSize.Y;

            // Initialize bullet animation frames
            bulletAnimationFrames = new List<Rectangle>();
            for (int r = 0; r < NUM_BULLET_ROWS; r++)
            {
                int x = r * (int)frameSize.X;
                bulletAnimationFrames.Add(new Rectangle(x, 0, (int)frameSize.X, (int)frameSize.Y));
            }
        }

        // Sets initial position based on boss helicopter position
        private void SetInitialPosition(Vector2 bossPosition)
        {
            currentPosition = bossPosition;
            direction = Vector2.Normalize(FighterAircraft.AircraftCurrentPosition - currentPosition);
        }

        // Sets position based on starting point ("left", "right", or default)
        private void SetPositionBasedOnStartingPoint(string startingPosition)
        {
            switch (startingPosition)
            {
                case "left":
                    currentPosition = BossHelicopter.BossCurrentPosition - new Vector2(50f, 10f);
                    break;
                case "right":
                    currentPosition = BossHelicopter.BossCurrentPosition + new Vector2(50f, -10f);
                    break;
                default:
                    currentPosition = BossHelicopter.BossCurrentPosition;
                    break;
            }
            direction = Vector2.Normalize(FighterAircraft.AircraftCurrentPosition - currentPosition);
            direction += direction;
        }



        public override void Update(GameTime gameTime)
        {
            UpdateAnimationFrame(gameTime);
            MoveBullet();
            CheckBulletRemoval();
            base.Update(gameTime);
        }

        private void UpdateAnimationFrame(GameTime gameTime)
        {
            elapsedTimeFrameChange += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTimeFrameChange >= frameChangeInterval)
            {
                frameIndex = (frameIndex + 1) % NUM_BULLET_ROWS;
                elapsedTimeFrameChange = 0;
            }
        }

        private void MoveBullet()
        {
            currentPosition += direction * bulletSpeed;
        }

        private void CheckBulletRemoval()
        {
            if (currentPosition.Y >= maxYPosition)
            {
                RemoveBossBulletDelegate?.Invoke(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(bulletTexture, currentPosition, bulletAnimationFrames[frameIndex], Color.White, 0f, textureOrigin, 0.07f, SpriteEffects.None, 0f);


            spriteBatch.End();
            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(frameSize.X * 0.1f);
            int scaledHeight = (int)(frameSize.Y * 0.1f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
