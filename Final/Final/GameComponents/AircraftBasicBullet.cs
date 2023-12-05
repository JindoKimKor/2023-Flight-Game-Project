using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.GameComponents
{
    public delegate void RemovePassedMaxYCoordinateBullet(AircraftBasicBullet bullet);

    public class AircraftBasicBullet : DrawableGameComponent
    {
        private SpriteBatch basicBulletSpriteBatch;
        private Texture2D basicBulletTexture;
        private Vector2 currentPosition;

        private Vector2 originTexture;
        private float maxBulletYCoordinate;
        private const float movingSpeed = 10f;

        private Vector2 bulletFrameDimension;
        private List<Rectangle> animationFrames;
        private const int BASIC_ATTACK_ROWS = 6;
        private int currentFrameIndex = 0;


        private MainGame mainGame;

        public RemovePassedMaxYCoordinateBullet RemoveBulletDelegate { get; set; }

        public AircraftBasicBullet(Game game, SpriteBatch playSceneAircraftSpriteBatch) : base(game)
        {
            mainGame = (MainGame)game;
            basicBulletSpriteBatch = playSceneAircraftSpriteBatch;
            currentPosition = PlayScene.FighterAircraftCurrentPosition;
            currentPosition.Y = currentPosition.Y - 30f;
            basicBulletTexture = mainGame.Content.Load<Texture2D>("images/aircraftBasicAttackBullet");
            bulletFrameDimension = new Vector2(basicBulletTexture.Width / BASIC_ATTACK_ROWS, basicBulletTexture.Height);
            maxBulletYCoordinate = -basicBulletTexture.Height;
            originTexture = new Vector2(bulletFrameDimension.X / 2, bulletFrameDimension.Y / 2);
            animationFrames = new List<Rectangle>();

            for (int c = 0; c < BASIC_ATTACK_ROWS; c++)
            {
                int x = c * (int)bulletFrameDimension.X;

                animationFrames.Add(new Rectangle(x, 0, (int)bulletFrameDimension.X, (int)bulletFrameDimension.Y));
            }
        }
        private double elapsedTime = 0;
        private double frameInterval = 50;
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= frameInterval)
            {
                currentFrameIndex++;
                currentPosition.Y = currentPosition.Y - movingSpeed;
                if (currentFrameIndex >= BASIC_ATTACK_ROWS)
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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            basicBulletSpriteBatch.Begin();

            basicBulletSpriteBatch.Draw(basicBulletTexture, currentPosition, animationFrames[currentFrameIndex], Color.White, 0f, originTexture, 0.2f, SpriteEffects.None, 0f);

            basicBulletSpriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
