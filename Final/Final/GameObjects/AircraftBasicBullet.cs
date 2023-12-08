using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing.Imaging;


namespace Final.GameComponents
{
    public delegate void RemovePassedMinYCoordinateBullet(AircraftBasicBullet bullet);

    public class AircraftBasicBullet : DrawableGameComponent
    {
        private SpriteBatch basicBulletSpriteBatch;
        private Texture2D basicBulletTexture;
        private Vector2 currentPosition;

        private Vector2 originTexture;
        private float maxBulletYCoordinate;
        private const float movingSpeed = 30f;
        private string direction;
        private Vector2 bulletFrameDimension;
        private List<Rectangle> animationFrames;
        private const int BASIC_BULLET_ROWS = 6;
        private int currentFrameIndex = 0;


        private MainGame mainGame;

        public RemovePassedMinYCoordinateBullet RemoveBulletDelegate { get; set; }

        public AircraftBasicBullet(Game game, SpriteBatch playSceneAircraftSpriteBatch) : base(game)
        {
            mainGame = (MainGame)game;
            basicBulletSpriteBatch = playSceneAircraftSpriteBatch;
            currentPosition = PlayScene.FighterAircraftCurrentPosition;
            currentPosition.Y = currentPosition.Y - 30f;
            basicBulletTexture = mainGame.Content.Load<Texture2D>("images/aircraftBasicAttackBullet");
            bulletFrameDimension = new Vector2(basicBulletTexture.Width / BASIC_BULLET_ROWS, basicBulletTexture.Height);
            maxBulletYCoordinate = -basicBulletTexture.Height;
            originTexture = new Vector2(bulletFrameDimension.X / 2, bulletFrameDimension.Y / 2);
            animationFrames = new List<Rectangle>();

            for (int c = 0; c < BASIC_BULLET_ROWS; c++)
            {
                int x = c * (int)bulletFrameDimension.X;

                animationFrames.Add(new Rectangle(x, 0, (int)bulletFrameDimension.X, (int)bulletFrameDimension.Y));
            }
            direction = "";
        }
        public AircraftBasicBullet(Game game, SpriteBatch playSceneAircraftSpriteBatch, string direction) : base(game)
        {
            mainGame = (MainGame)game;
            basicBulletSpriteBatch = playSceneAircraftSpriteBatch;
            currentPosition = PlayScene.FighterAircraftCurrentPosition;
            currentPosition.Y = currentPosition.Y - 30f;
            basicBulletTexture = mainGame.Content.Load<Texture2D>("images/aircraftBasicAttackBullet");
            bulletFrameDimension = new Vector2(basicBulletTexture.Width / BASIC_BULLET_ROWS, basicBulletTexture.Height);
            maxBulletYCoordinate = -basicBulletTexture.Height;
            originTexture = new Vector2(bulletFrameDimension.X / 2, bulletFrameDimension.Y / 2);
            animationFrames = new List<Rectangle>();

            for (int c = 0; c < BASIC_BULLET_ROWS; c++)
            {
                int x = c * (int)bulletFrameDimension.X;

                animationFrames.Add(new Rectangle(x, 0, (int)bulletFrameDimension.X, (int)bulletFrameDimension.Y));
            }
            this.direction = direction;
        }



        private double elapsedTime = 0;
        private double frameInterval = 50;
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= frameInterval)
            {
                currentFrameIndex++;
                switch (direction)
                {
                    case "left":
                        currentPosition.X -= movingSpeed * 0.4f;
                        currentPosition.Y -= movingSpeed * 0.7f;
                        break;
                    case "center":
                        currentPosition.Y -= movingSpeed;
                        break;
                    case "right":
                        currentPosition.X += movingSpeed * 0.4f;
                        currentPosition.Y -= movingSpeed * 0.7f;
                        break;
                    default:
                        currentPosition.Y -= movingSpeed;
                        break;
                }
                if (currentFrameIndex >= BASIC_BULLET_ROWS)
                {
                    currentFrameIndex = 0;
                }
                elapsedTime = 0;
            }
            if (currentPosition.Y <= maxBulletYCoordinate)
            {
                //It got called only if It's not null
                RemoveBulletDelegate?.Invoke(this);
            }
            if (true)
            {

            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            basicBulletSpriteBatch.Begin();

            basicBulletSpriteBatch.Draw(basicBulletTexture, currentPosition, animationFrames[currentFrameIndex], Color.White, 0f, originTexture, 0.2f, SpriteEffects.None, 0f);

            basicBulletSpriteBatch.End();


            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(bulletFrameDimension.X * 0.2f);
            int scaledHeight = (int)(bulletFrameDimension.Y * 0.2f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
