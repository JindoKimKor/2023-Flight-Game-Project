using Final.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    public class PlayScene : GameScene
    {
        private SpriteBatch playSceneSpriteBatch;
        private Texture2D backgroundTexture;
        private Rectangle screenRectangleOne;
        private Rectangle screenRectangleTwo;
        private int backgroundTextureScrollSpeed = 3;

        private FighterAircraft fighterAircraft;
        private Texture2D fighterAircraftTexture;
        private static bool isStartingSequence = true;
        private Vector2 fighterAircraftStartingPosition;
        private float fighterAircraftFullyLoadedYPosition = Shared.stageSize.Y - 200;
        private Vector2 fighterAircraftCurrentPosition;
        private int fighterAircraftEntrySpeed;
        private MainGame mainGame;

        public static bool IsStartingSequence { get => isStartingSequence; set => isStartingSequence = value; }

        public PlayScene(Game game) : base(game)
        {
            mainGame = (MainGame)game;
            playSceneSpriteBatch = mainGame._spriteBatch;
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            fighterAircraftTexture = mainGame.Content.Load<Texture2D>("images/fighterAircraft");
            screenRectangleOne = new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            screenRectangleTwo = new Rectangle(0, -(int)Shared.stageSize.Y, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            fighterAircraftStartingPosition = new Vector2(Shared.stageSize.X / 2, Shared.stageSize.Y);
            fighterAircraftEntrySpeed = 2;

            fighterAircraft = new FighterAircraft(mainGame, playSceneSpriteBatch, fighterAircraftTexture, fighterAircraftStartingPosition, fighterAircraftEntrySpeed);
            fighterAircraftCurrentPosition = fighterAircraftStartingPosition;
            ComponentList.Add(fighterAircraft);
            fighterAircraft.Show();
        }


        public override void Update(GameTime gameTime)
        {
            screenRectangleOne.Y += backgroundTextureScrollSpeed;
            screenRectangleTwo.Y += backgroundTextureScrollSpeed;
            if (screenRectangleOne.Y >= (int)Shared.stageSize.Y)
            {
                screenRectangleOne.Y = 0;
            }
            if (screenRectangleTwo.Y >= 0)
            {
                screenRectangleTwo.Y = -(int)Shared.stageSize.Y;
            }

            //Control aircraft loading time
            if (IsStartingSequence)
            {
                fighterAircraftCurrentPosition.Y -= fighterAircraftEntrySpeed;

                if (fighterAircraftCurrentPosition.Y == fighterAircraftFullyLoadedYPosition)
                {
                    IsStartingSequence = false;
                }
                fighterAircraft.ChangeAnimationAndPosition(AircraftFrames.Idle, fighterAircraftCurrentPosition);
            }




            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            playSceneSpriteBatch.Begin();
            playSceneSpriteBatch.Draw(backgroundTexture, screenRectangleOne, Color.White);
            playSceneSpriteBatch.Draw(backgroundTexture, screenRectangleTwo, Color.White);

            playSceneSpriteBatch.End();
            base.Draw(gameTime);
        }



    }
}
