using Final.GameObjects.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Final.GameObjects.Weapons
{
    public delegate void RemovePassedMinYCoordinateBullet(AircraftBasicBullet bullet);
    /// <summary>
    /// Main Character bullet Class
    /// </summary>
    public class AircraftBasicBullet : DrawableGameComponent
    {
        // SpriteBatch and texture for rendering the basic bullet
        private SpriteBatch spriteBatch;
        private Texture2D bulletTexture;

        // Animation frames and dimensions for the bullet
        private Vector2 bulletFrameSize;
        private List<Rectangle> animationFrames;
        private const int BASIC_BULLET_ROWS = 6;

        // Position and movement properties of the bullet
        private Vector2 currentPosition;
        private Vector2 originTexture;
        private float maxBulletYCoordinate;
        private const float INITIAL_Y_OFFSET = 30f;
        private const float BULLET_SPEED = 12f;
        private string bulletDirection; // Direction of bullet movement: "left", "center", "right"

        // Animation control for the bullet
        private int currentFrameIndex = 0;
        private double elapsedTime = 0;
        private double frameInterval = 50;

        // Main game reference for accessing shared resources
        private MainGame mainGame;

        private const float HORIZONTAL_MOVEMENT_FACTOR = 0.3f;
        private const float VERTICAL_MOVEMENT_FACTOR = 0.7f;

        // Delegate for removing bullet when it passes a certain Y coordinate
        public RemovePassedMinYCoordinateBullet RemoveBulletDelegate { get; set; }

        /// <summary>
        /// One Bullet Mode Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playSceneAircraftSpriteBatch"></param>
        public AircraftBasicBullet(Game game, SpriteBatch playSceneAircraftSpriteBatch) : base(game)
        {
            InitializeBullet(game, playSceneAircraftSpriteBatch, "");
        }
        /// <summary>
        /// Three Bullet Mode Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playSceneAircraftSpriteBatch"></param>
        /// <param name="direction"></param>
        public AircraftBasicBullet(Game game, SpriteBatch playSceneAircraftSpriteBatch, string direction) : base(game)
        {
            InitializeBullet(game, playSceneAircraftSpriteBatch, direction);
        }

        // Initializes the basic bullet with common setup
        private void InitializeBullet(Game game, SpriteBatch spriteBatch, string direction)
        {
            mainGame = (MainGame)game;
            this.spriteBatch = spriteBatch;
            currentPosition = FighterAircraft.AircraftCurrentPosition - new Vector2(0, INITIAL_Y_OFFSET);
            bulletTexture = mainGame.Content.Load<Texture2D>("images/aircraftBasicAttackBullet");
            bulletFrameSize = new Vector2(bulletTexture.Width / BASIC_BULLET_ROWS, bulletTexture.Height);
            maxBulletYCoordinate = -bulletTexture.Height;
            originTexture = new Vector2(bulletFrameSize.X / 2, bulletFrameSize.Y / 2);
            animationFrames = GenerateAnimationFrames();
            bulletDirection = direction;

            // Generates animation frames for the bullet
            List<Rectangle> GenerateAnimationFrames()
            {
                List<Rectangle> frames = new List<Rectangle>();
                for (int c = 0; c < BASIC_BULLET_ROWS; c++)
                {
                    int x = c * (int)bulletFrameSize.X;
                    frames.Add(new Rectangle(x, 0, (int)bulletFrameSize.X, (int)bulletFrameSize.Y));
                }
                return frames;
            }

        }

        public override void Update(GameTime gameTime)
        {

            UpdateAnimationFrame();
            UpdateBulletPosition();
            CheckBulletPositionAndRemove();

            void UpdateAnimationFrame()
            {
                elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime >= frameInterval)
                {
                    // Setting the index
                    currentFrameIndex = (currentFrameIndex + 1) % BASIC_BULLET_ROWS;
                    elapsedTime = 0;
                }
            }
            void UpdateBulletPosition()
            {
                switch (bulletDirection)
                {
                    case "left":
                        currentPosition.X -= BULLET_SPEED * HORIZONTAL_MOVEMENT_FACTOR;
                        currentPosition.Y -= BULLET_SPEED * VERTICAL_MOVEMENT_FACTOR;
                        break;
                    case "center":
                        currentPosition.Y -= BULLET_SPEED;
                        break;
                    case "right":
                        currentPosition.X += BULLET_SPEED * HORIZONTAL_MOVEMENT_FACTOR;
                        currentPosition.Y -= BULLET_SPEED * VERTICAL_MOVEMENT_FACTOR;
                        break;
                    default:
                        currentPosition.Y -= BULLET_SPEED;
                        break;
                }
            }
            void CheckBulletPositionAndRemove()
            {
                if (currentPosition.Y <= maxBulletYCoordinate)
                {
                    //It got called only if It's not null
                    RemoveBulletDelegate?.Invoke(this);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(bulletTexture, currentPosition, animationFrames[currentFrameIndex], Color.White, 0f, originTexture, 0.2f, SpriteEffects.None, 0f);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(bulletFrameSize.X * 0.2f);
            int scaledHeight = (int)(bulletFrameSize.Y * 0.2f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
