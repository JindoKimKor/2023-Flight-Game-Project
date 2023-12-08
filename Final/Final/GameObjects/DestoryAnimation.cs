using Final.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.GameObjects
{
    public class DestoryAnimation : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D destroyAnimationTexture;
        private Vector2 destroyAnimationFrameDimension;
        private const int DESTROY_ANIMATION_COLS = 7;
        private List<Rectangle> destroyAnimationFrames;
        private int destroyAnimationFrameIndex = 0;
        private Vector2 destroyAnimationPosition;
        private Vector2 originTexture;

        public DestoryAnimation(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            spriteBatch = playSceneSpriteBatch;
            destroyAnimationTexture = mainGame.Content.Load<Texture2D>("images/destroyAnimation");
            destroyAnimationFrameDimension = new Vector2(destroyAnimationTexture.Width / DESTROY_ANIMATION_COLS, destroyAnimationTexture.Height);
            destroyAnimationFrames = new List<Rectangle>();
            for (int c = 0; c < DESTROY_ANIMATION_COLS; c++)
            {
                int x = c * (int)destroyAnimationFrameDimension.X;
                destroyAnimationFrames.Add(new Rectangle(x, 0, (int)destroyAnimationFrameDimension.X, (int)destroyAnimationFrameDimension.Y));
            }
            originTexture = new Vector2(destroyAnimationFrameDimension.X / 2, destroyAnimationFrameDimension.Y / 2);
            Random random = new Random();
            destroyAnimationPosition = new Vector2(
                random.Next((int)BossHelicopter.BossHelicopterCurrentPosition.X - 50, (int)BossHelicopter.BossHelicopterCurrentPosition.X + 50),
                random.Next((int)BossHelicopter.BossHelicopterCurrentPosition.Y - 50, (int)BossHelicopter.BossHelicopterCurrentPosition.Y + 50));
        }

        private float destroyeGeneratingElapsedTime = 0;

        public override void Update(GameTime gameTime)
        {
            destroyeGeneratingElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (destroyeGeneratingElapsedTime >= 0.3f && destroyAnimationFrameIndex < DESTROY_ANIMATION_COLS - 1)
            {
                destroyAnimationFrameIndex++;
                destroyeGeneratingElapsedTime = 0f;
            }

            base.Update(gameTime);
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
