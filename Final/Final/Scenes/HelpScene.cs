using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    internal class HelpScene : GameScene
    {
        private SpriteBatch helpSceneSpriteBatch;
        private Texture2D tex;
        public HelpScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            helpSceneSpriteBatch = mainGame._spriteBatch;


        }
        public override void Draw(GameTime gameTime)
        {
            helpSceneSpriteBatch.Begin();
            helpSceneSpriteBatch.Draw(tex, Vector2.Zero, Color.White);
            helpSceneSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
