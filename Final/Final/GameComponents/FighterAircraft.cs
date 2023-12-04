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
    public enum AircraftFrames
    {
        Idle = 12,
        MoveLeftSlow = 11,
        MoveLeftFast = 10,
        MoveRightSlow = 13,
        MoveRightFast = 14,
        MoveUpSlow = 7,
        MoveUpFast = 2,
        MoveDownSlow = 17,
        MoveDownFast = 22,
        MoveNorthwestSlow = 6,
        MoveNorthwestFast = 0,
        MoveNortheastSlow = 8,
        MoveNortheastFast = 4,
        MoveSouthwestSlow = 16,
        MoveSouthwestFast = 20,
        MoveSoutheastSlow = 19,
        MoveSoutheastFast = 24,
    }

    public class FighterAircraft : DrawableGameComponent
    {
        private SpriteBatch fighterAircraftSpriteBatch;
        private Texture2D fighterAircraftTexture;
        private Vector2 frameDimension;
        private List<Rectangle> animationFrames;
        private AircraftFrames currentFrame;
        private Vector2 textureOrigin;

        //frame index
        private const int ROWS = 5;
        private const int COLS = 5;


        public FighterAircraft(Game game, SpriteBatch playSceneSpriteBatch, Texture2D fighterAircraftTexture, Vector2 startingPosition) : base(game)
        {
            
            fighterAircraftSpriteBatch = playSceneSpriteBatch;
            this.fighterAircraftTexture = fighterAircraftTexture;
            frameDimension = new Vector2(fighterAircraftTexture.Width / ROWS, fighterAircraftTexture.Height / COLS);
            textureOrigin = new Vector2(frameDimension.X / 2, frameDimension.Y / 2);
            PlayScene.FighterAircraftCurrentPosition = startingPosition;
            currentFrame = AircraftFrames.Idle;
            InitializeAnimationFrames();
        }

        private void InitializeAnimationFrames()
        {
            animationFrames = new List<Rectangle>();
            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    int x = c * (int)frameDimension.X;
                    int y = r * (int)frameDimension.Y;

                    Rectangle frameRectangle = new Rectangle(x, y, (int)frameDimension.X, (int)frameDimension.Y);

                    animationFrames.Add(frameRectangle);
                }
            }
        }

        public void Hide()
        {
            this.Enabled = false;
            this.Visible = false;
        }

        public void Show()
        {
            this.Enabled = true;
            this.Visible = true;
        }

        public void ChangeAirCraftPositionAndAnimationWithSpeed(AircraftFrames newFrame, Vector2 newPosition)
        {
            float screenEdgeMinimumX = (frameDimension.X / 2);
            float screenEdgeMinimumY = (frameDimension.Y / 2);
            float screenEdgeMaxX = Shared.stageSize.X - (frameDimension.X / 2);
            float screenEdgeMaxY = Shared.stageSize.Y - (frameDimension.Y / 2);


            currentFrame = newFrame;
            //To keep the aircraft within the screen
            if (!PlayScene.IsStartingSequence)
            {
                PlayScene.FighterAircraftCurrentPosition = new Vector2(
                    newPosition.X <= screenEdgeMinimumX ? screenEdgeMinimumX : newPosition.X >= screenEdgeMaxX ? screenEdgeMaxX : newPosition.X,
                    newPosition.Y <= screenEdgeMinimumY ? screenEdgeMinimumY : newPosition.Y >= screenEdgeMaxY ? screenEdgeMaxY : newPosition.Y
                );
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            fighterAircraftSpriteBatch.Begin();
            fighterAircraftSpriteBatch.Draw(fighterAircraftTexture, PlayScene.FighterAircraftCurrentPosition, animationFrames[(int)currentFrame], Color.White, 0f, textureOrigin, 1.1f, SpriteEffects.None, 0f);
            fighterAircraftSpriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
