using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class StartScene : GameScene
    {
        private MenuComponent menu;
        private SpriteBatch sb;
        private SpriteFont titleFont;
        private string titleText = "2023";
        

        public MenuComponent Menu { get => menu; set => menu = value; }

        public StartScene(Game game) : base(game)
        {
            Game1 g = (Game1)game;
            this.sb = g._spriteBatch;
            SpriteFont regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
            SpriteFont hilightFont = game.Content.Load<SpriteFont>("fonts/HilightFont"); 
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");


            string[] menuItems = { "Start Game", "Help", "Leaderboard", "Credit", "Option", "Exit" };
            menu = new MenuComponent(game, sb, regularFont, hilightFont, menuItems);
            this.Components.Add(menu);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            sb.Begin();

            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Game.GraphicsDevice.Viewport.Width - titleSize.X) / 2, 100); 

            sb.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            sb.End();
        }
    }
}
