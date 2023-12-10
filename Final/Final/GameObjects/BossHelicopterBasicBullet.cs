using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Final.GameComponents
{
    public delegate void RemovePassedMaxYCoordinateBossBullet(BossHelicopterBasicBullet bullet);

    /// <summary>
    /// Boss Helicopter Bullet Class
    /// </summary>
    public class BossHelicopterBasicBullet : DrawableGameComponent
    {
        // Main game reference
        private MainGame mainGame;

        // SpriteBatch and texture for bullet rendering
        private SpriteBatch spriteBatch;
        private Texture2D bulletTexture;
        private Texture2D secondStageBulletTexture;

        // Animation frames and dimensions
        private Vector2 bulletFrameSize;
        private Vector2 secondStageBulletFrameSize;
        private List<Rectangle> bulletAnimationFrames;
        private List<Rectangle> secondStageBulletAnimationFrames;
        private const int BULLET_COLS = 4;
        private const int SPECIAL_BULLET_COLS = 4;

        // Bullet position, direction, and origin
        private Vector2 currentPosition;
        private Vector2 direction;
        private Vector2 bulletTextureOrigin;
        private Vector2 secondStageBulletTextureOrigin;

        // Bullet movement properties
        private float maxYPosition;
        private float bulletSpeed = 6.0f;
        private float rotationAngle = 0f;
        private const float rotationSpeed = 8f;

        // Animation control
        private int frameIndex = 0;
        private double elapsedTimeFrameChange = 0;
        private double frameChangeInterval = 40;
        private string startingPosition = "";

        // Delegate for removing the bullet when it passes a certain Y coordinate
        public RemovePassedMaxYCoordinateBossBullet RemoveBossBulletDelegate { get; set; }

        /// <summary>
        /// Boss first stage level bullet constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playSceneSpriteBatch"></param>
        public BossHelicopterBasicBullet(Game game, SpriteBatch playSceneSpriteBatch)
    : base(game)
        {
            // Set main game and spriteBatch references
            mainGame = (MainGame)game;
            this.spriteBatch = playSceneSpriteBatch;
            // Initializes shared bullet properties
            InitializeBullet();
            // Sets initial position based on boss helicopter position
            SetInitialPosition(BossHelicopter.BossCurrentPosition);

            void InitializeBullet()
            {
                // Load texture and set frame size
                bulletTexture = mainGame.Content.Load<Texture2D>("images/bossHelicopterBasicBullet");
                bulletFrameSize = new Vector2(bulletTexture.Width / BULLET_COLS, bulletTexture.Height);
                bulletTextureOrigin = new Vector2(bulletFrameSize.X / 2, bulletFrameSize.Y / 2);
                maxYPosition = Shared.stageSize.Y;

                // Initialize bullet animation frames
                bulletAnimationFrames = new List<Rectangle>();
                for (int c = 0; c < BULLET_COLS; c++)
                {
                    int x = c * (int)bulletFrameSize.X;
                    bulletAnimationFrames.Add(new Rectangle(x, 0, (int)bulletFrameSize.X, (int)bulletFrameSize.Y));
                }
            }
            void SetInitialPosition(Vector2 bossPosition)
            {
                currentPosition = bossPosition;
                direction = Vector2.Normalize(FighterAircraft.AircraftCurrentPosition - currentPosition);
            }
        }

        /// <summary>
        /// Boss second stage level bullet constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playSceneSpriteBatch"></param>
        /// <param name="startingPosition">spawning position</param>
        public BossHelicopterBasicBullet(Game game, SpriteBatch playSceneSpriteBatch, string startingPosition)
            : base(game)
        {
            // Set main game and spriteBatch references
            mainGame = (MainGame)game;
            this.spriteBatch = playSceneSpriteBatch;
            InitializeBullet();
            this.startingPosition = startingPosition;
            SetPositionBasedOnStartingPoint();

            // Initializes shared bullet properties
            void InitializeBullet()
            {
                // Load texture and set frame size
                secondStageBulletTexture = mainGame.Content.Load<Texture2D>("images/bossSpecialAttack");
                secondStageBulletFrameSize = new Vector2(secondStageBulletTexture.Width / SPECIAL_BULLET_COLS, secondStageBulletTexture.Height);
                secondStageBulletTextureOrigin = new Vector2(secondStageBulletFrameSize.X / 2, secondStageBulletFrameSize.Y / 2);
                maxYPosition = Shared.stageSize.Y;

                // Initialize bullet animation frames
                secondStageBulletAnimationFrames = new List<Rectangle>();
                for (int c = 0; c < SPECIAL_BULLET_COLS; c++)
                {
                    int x = c * (int)secondStageBulletFrameSize.X;
                    secondStageBulletAnimationFrames.Add(new Rectangle(x, 0, (int)secondStageBulletFrameSize.X, (int)secondStageBulletFrameSize.Y));
                }
            }
            // Sets position based on starting point ("left", "right", or default)
            void SetPositionBasedOnStartingPoint()
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
        }







        public override void Update(GameTime gameTime)
        {
            UpdateAnimationFrame(gameTime);
            MoveBullet(gameTime);
            CheckBulletRemoval();
            base.Update(gameTime);

            void UpdateAnimationFrame(GameTime gameTime)
            {
                elapsedTimeFrameChange += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTimeFrameChange >= frameChangeInterval)
                {
                    frameIndex = (frameIndex + 1) % BULLET_COLS;
                    elapsedTimeFrameChange = 0;
                }
            }

            void MoveBullet(GameTime gameTime)
            {
                if (startingPosition == "")
                {
                    currentPosition += direction * bulletSpeed;
                }
                else
                {
                    currentPosition.Y += bulletSpeed * 2;
                    rotationAngle += rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    rotationAngle = rotationAngle % MathHelper.TwoPi;
                }

            }

            void CheckBulletRemoval()
            {
                if (currentPosition.Y >= maxYPosition)
                {
                    RemoveBossBulletDelegate?.Invoke(this);
                }
            }
        }

        

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (startingPosition == "center")
            {
                spriteBatch.Draw(secondStageBulletTexture, currentPosition, secondStageBulletAnimationFrames[frameIndex], Color.White, rotationAngle, secondStageBulletTextureOrigin, 0.2f, SpriteEffects.None, 0f);
            }
            else if (startingPosition == "left" || startingPosition == "right")
            {
                spriteBatch.Draw(secondStageBulletTexture, currentPosition, secondStageBulletAnimationFrames[frameIndex], Color.White, rotationAngle, bulletTextureOrigin, 0.2f, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(bulletTexture, currentPosition, bulletAnimationFrames[frameIndex], Color.White, 0f, bulletTextureOrigin, 0.07f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// To get boss helicopter bullet hit box
        /// </summary>
        /// <returns>Boss Bullet Texture's Frame Positions and Size</returns>
        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(bulletFrameSize.X * 0.1f);
            int scaledHeight = (int)(bulletFrameSize.Y * 0.1f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
