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
    public class SmallHelicopterBullet : DrawableGameComponent
    {
        private MainGame mainGame;
        private SpriteBatch smallHelicopterBullet;
        private SmallHelicopter smallHelicopter;
        private Texture2D bulletTexture;
        private Vector2 frameDimension;
        private List<Rectangle> animationFrames;
        private const int BULLET_COLS = 4;
        private int currentIndex = 0;
        private Vector2 currentPosition;
        private Vector2 direction;
        private Vector2 textureOrigin;
        private float bulletSpeed = 4.5f;

        public SmallHelicopterBullet(Game game, SpriteBatch playSceneSpriteBatch, SmallHelicopter smallHelicopter) : base(game)
        {
            mainGame = (MainGame)game;
            smallHelicopterBullet = playSceneSpriteBatch;
            this.smallHelicopter = smallHelicopter;
            bulletTexture = mainGame.Content.Load<Texture2D>("images/smallHelicopterBullet");
            frameDimension = new Vector2(bulletTexture.Width / BULLET_COLS, bulletTexture.Height);
            animationFrames = new List<Rectangle>();
            for (int c = 0; c < BULLET_COLS; c++)
            {
                int x = c * (int)frameDimension.X;

                animationFrames.Add(new Rectangle(x, 0, (int)frameDimension.X, (int)frameDimension.Y));
            }
            currentPosition = smallHelicopter.CurrentPosition;
            textureOrigin = new Vector2(frameDimension.X / 2, frameDimension.Y / 2);
            direction = Vector2.Normalize(PlayScene.FighterAircraftCurrentPosition - currentPosition);
        }

        private double elapsedTime = 0;
        private double frameInterval = 100;
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= frameInterval)
            {
                currentIndex++;
                elapsedTime = 0;
            }
            if (currentIndex >= BULLET_COLS - 1)
            {
                currentIndex = 0;
            }
            currentPosition += direction * bulletSpeed;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            smallHelicopterBullet.Begin();
            smallHelicopterBullet.Draw(bulletTexture, currentPosition, animationFrames[currentIndex], Color.White, 0f, textureOrigin, 0.2f, SpriteEffects.None, 0f);
            smallHelicopterBullet.End();

            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(frameDimension.X * 0.2f);
            int scaledHeight = (int)(frameDimension.Y * 0.2f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
