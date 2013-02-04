using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace RPG
{
    public class State
    {

        public SizeF Area;
        public GameWorld World;
     
     
        private Dictionary<string, Tile> _tiles = new Dictionary<string, Tile>();

        private static Font _font = new Font("Arial", 24);
        private static Brush _brush = new SolidBrush(Color.White);
        private static Random _random = new Random();



        public State(SizeF gameArea)
        {
            Area = gameArea;

            //Load in all the tile definitions
            readTileDefinitions(@"gamedata\tileProperties.csv");
        }        

        public void Draw(Graphics graphics)
        {
            World.Draw(graphics);                  

        }

        public void Update(double gameTime, double elapsedTime)
        {
            World.Update(gameTime, elapsedTime);
        }


        public void Initialize()
        {  
            //Create all the main gameobjects
            World = new GameWorld(this, _tiles, @"gamedata\map.txt");            
        }

        //Each line contains a comma delimited tile definition that the tile constructor understands
        private void readTileDefinitions(string tileDescriptionFile)
        {
            using (StreamReader stream = new StreamReader(tileDescriptionFile))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    //separate out the elements of the 
                    string[] elements = line.Split(',');

                    //And make the tile.
                    Tile tile = new Tile(elements);
                    _tiles.Add(tile.Shortcut, tile);
                }
            }
        }


        public void KeyDown(Keys keys)
        {
           
            World.KeyDown(keys);
           
        }
    }
  }

