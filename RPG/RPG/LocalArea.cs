using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace RPG
{
    class LocalArea : GameObject
    {
        public const int OffsetX = 30;
        public const int OffsetY = 50;
        public const int MapSizeX = 10;
        public const int MapSizeY = 10;

        public MapTile[,] Map = new MapTile[MapSizeX, MapSizeY]; 
        private Rectangle _areaRectangle = new Rectangle(OffsetX, OffsetY, MapSizeX * Tile.TileSizeX, MapSizeY * Tile.TileSizeY);

        public string Name;

        
        
         public LocalArea(StreamReader stream, Dictionary<string, Tile> tiles)
        {
            string line;

            //1st line is the name
            Name = stream.ReadLine().ToLower();

          

            //Read in 10 lines of 10 characters each. Look up the tile and make the
            //matching sprite
            for (int j = 0; j < MapSizeY; j++)
            {
                //Get a line of map characters
                line = stream.ReadLine();

                for (int i = 0; i < MapSizeX; i++)
                {
                    MapTile mapTile = new MapTile();
                    Map[i, j] = mapTile;
                    mapTile.Tile = tiles[line[i].ToString()];
                    mapTile.SetSprite(i, j);
                }
            }

            //Read game objects until the blank line
            while (!stream.EndOfStream && (line = stream.ReadLine().Trim()) != "")
            {
                //Each line is an x,y coordinate and a tile shortcut
                //Look up the tile and construct the sprite
                string[] elements = line.Split(',');
                int x = Convert.ToInt32(elements[0]);
                int y = Convert.ToInt32(elements[1]);
                MapTile mapTile = Map[x, y];
                mapTile.ObjectTile = tiles[elements[2]];
                mapTile.SetObjectSprite(x, y);

                if (mapTile.ObjectTile.IsTransparent)
                {
                    mapTile.ObjectSprite.ColorKey = Color.FromArgb(75, 75, 75);
                }
            }

        }

         public override void Update(double gameTime, double elapsedTime)
        {
            //Update all the map tiles and any objects
            foreach (MapTile mapTile in Map)
            {
                mapTile.Sprite.Update(gameTime, elapsedTime);
                if (mapTile.ObjectSprite != null)
                {
                    if (mapTile.ObjectSprite.NumberOfFrames > 1)
                    {
                        mapTile.ObjectSprite.CurrentFrame = (int)((gameTime * 8.0) % (double)mapTile.ObjectSprite.NumberOfFrames);
                    }
                    mapTile.ObjectSprite.Update(gameTime, elapsedTime);
                }
            }
        }

        public override void  Draw(Graphics graphics)
        {
             foreach (MapTile mapTile in Map)
            {
                mapTile.Sprite.Draw(graphics);
                if (mapTile.ObjectSprite != null)
                {
                    mapTile.ObjectSprite.Draw(graphics);
                }
            } 
        }
    }
}
