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

        //frame index
        private const int ROWS = 5;
        private const int COLS = 5;


        public FighterAircraft(Game game, SpriteBatch playSceneSpriteBatch, Texture2D fighterAircraftTexture, Vector2 startingPosition, int entrySpeed) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            fighterAircraftSpriteBatch = playSceneSpriteBatch;
            this.fighterAircraftTexture = fighterAircraftTexture;
            frameDimension = new Vector2(fighterAircraftTexture.Width / ROWS, fighterAircraftTexture.Height / COLS);
            PlayScene.FighterAircraftCurrentPosition = startingPosition;
            currentFrame = AircraftFrames.Idle;
            InitializeAnimationFrames();
        }

        private void InitializeAnimationFrames()
        {
            animationFrames = new List<Rectangle>();
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    int x = j * (int)frameDimension.X;
                    int y = i * (int)frameDimension.Y;

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
            currentFrame = newFrame;
            PlayScene.FighterAircraftCurrentPosition = new Vector2(
                newPosition.X <= 0 ? 0 : newPosition.X >= (Shared.stageSize.X - frameDimension.X) ? (Shared.stageSize.X - frameDimension.X) : newPosition.X,
                newPosition.Y <= 0 ? 0 : newPosition.Y >= (Shared.stageSize.Y - frameDimension.Y) ? (Shared.stageSize.Y - frameDimension.Y) : newPosition.Y
            );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            fighterAircraftSpriteBatch.Begin();
            fighterAircraftSpriteBatch.Draw(fighterAircraftTexture, PlayScene.FighterAircraftCurrentPosition, animationFrames[(int)currentFrame], Color.White);
            fighterAircraftSpriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
