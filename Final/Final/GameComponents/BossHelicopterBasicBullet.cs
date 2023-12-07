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
        private MainGame mainGame;

        private SpriteBatch bossBasicBulletSpriteBatch;
        private Texture2D basicBulletTexture;
        private Vector2 frameDimension;
        private List<Rectangle> animationFrames;
        private const int BASIC_BULLET_ROWS = 4;
        private Vector2 currentPosition;
        private Vector2 direction;
        private Vector2 originTexture;

        private float maxYCoordinate;
        private float bulletSpeed = 7.0f;
        private int currentFrameIndex = 0;

        public RemovePassedMaxYCoordinateBossBullet RemoveBossBulletDelegate { get; set; }

        public BossHelicopterBasicBullet(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {
            mainGame = (MainGame)game;
            bossBasicBulletSpriteBatch = playSceneSpriteBatch;
            basicBulletTexture = mainGame.Content.Load<Texture2D>("images/bossHelicopterBasicBullet");
            frameDimension = new Vector2(basicBulletTexture.Width / BASIC_BULLET_ROWS, basicBulletTexture.Height);
            animationFrames = new List<Rectangle>();
            currentPosition = BossHelicopter.BossHelicopterCurrentPosition;
            originTexture = new Vector2(frameDimension.X / 2, frameDimension.Y / 2);
            direction = Vector2.Normalize(PlayScene.FighterAircraftCurrentPosition - currentPosition);
            maxYCoordinate = Shared.stageSize.Y;

            for (int r = 0; r < BASIC_BULLET_ROWS; r++)
            {
                int x = r * (int)frameDimension.X;
                animationFrames.Add(new Rectangle(x, 0, (int)frameDimension.X, (int)frameDimension.Y));
            }
        }

        public BossHelicopterBasicBullet(Game game, SpriteBatch playSceneSpriteBatch, string startingPosition) : base(game)
        {
            mainGame = (MainGame)game;
            bossBasicBulletSpriteBatch = playSceneSpriteBatch;
            basicBulletTexture = mainGame.Content.Load<Texture2D>("images/bossHelicopterBasicBullet");
            frameDimension = new Vector2(basicBulletTexture.Width / BASIC_BULLET_ROWS, basicBulletTexture.Height);
            animationFrames = new List<Rectangle>();
            switch (startingPosition)
            {
                case "left":
                    currentPosition = BossHelicopter.BossHelicopterCurrentPosition - new Vector2(50f, 0);
                    break;
                case "right":
                    currentPosition = BossHelicopter.BossHelicopterCurrentPosition + new Vector2(50f, 0);
                    break;
                default:
                    currentPosition = BossHelicopter.BossHelicopterCurrentPosition;
                    break;
            }

            
            originTexture = new Vector2(frameDimension.X / 2, frameDimension.Y / 2);
            direction = Vector2.Normalize(PlayScene.FighterAircraftCurrentPosition - currentPosition);
            direction += direction;
            maxYCoordinate = Shared.stageSize.Y;

            for (int r = 0; r < BASIC_BULLET_ROWS; r++)
            {
                int x = r * (int)frameDimension.X;
                animationFrames.Add(new Rectangle(x, 0, (int)frameDimension.X, (int)frameDimension.Y));
            }
        }


        private double elapsedTime = 0;
        private double frameInterval = 40;
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= frameInterval)
            {
                currentFrameIndex++;
                currentPosition += direction * bulletSpeed;
                if (currentFrameIndex >= BASIC_BULLET_ROWS)
                {
                    currentFrameIndex = 0;
                }
                elapsedTime = 0;
            }
            if (currentPosition.Y >= maxYCoordinate)
            {
                //It got called only if It's not null
                RemoveBossBulletDelegate?.Invoke(this);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            bossBasicBulletSpriteBatch.Begin();

            bossBasicBulletSpriteBatch.Draw(basicBulletTexture, currentPosition, animationFrames[currentFrameIndex], Color.White, 0f, originTexture, 0.07f, SpriteEffects.None, 0f);


            bossBasicBulletSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
