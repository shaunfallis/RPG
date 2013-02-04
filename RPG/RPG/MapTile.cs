using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RPG
{
    public class MapTile
    {
        public Tile Tile;
        public Sprite Sprite;
        public Sprite ObjectSprite;
        public Tile ObjectTile;
        public int ObjectHealth; //A copy of the health of the tile so we remember how damage monsters are

        public void SetSprite(int x, int y)
        {
            //Update the sprite
            Sprite = new Sprite(null, LocalArea.OffsetX + x * Tile.TileSizeX,
                                      LocalArea.OffsetY + y * Tile.TileSizeY,
                                      Tile.Bitmap, Tile.Rectangle,
                                      Tile.NumberOfFrames);
        }

        public void SetObjectSprite(int x, int y)
        {
            //Update the sprite
            ObjectSprite = new Sprite(null, LocalArea.OffsetX + x * Tile.TileSizeX,
                                      LocalArea.OffsetY + y * Tile.TileSizeY,
                                      ObjectTile.Bitmap, ObjectTile.Rectangle,
                                      ObjectTile.NumberOfFrames);
            if (ObjectTile.IsTransparent)
            {
                ObjectSprite.ColorKey = Color.FromArgb(75, 75, 75);
            }
        }
    }
}
