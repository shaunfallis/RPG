using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RPG
{
    public class GameWorld : GameObject
    {
        private const string _startArea = "start";

        private Dictionary<string, LocalArea> _world = new Dictionary<string, LocalArea>();
        private LocalArea _currentArea;
        private Dictionary<string, Tile> _tiles;        
        private Point _heroPosition;
        private Sprite _heroSprite;
        private bool _heroSpriteAnimating;
        private bool _heroSpriteFighting;        
        private double _startFightTime = -1.0;
        private PointF _heroDestination;
        private HeroDirection _direction;
        private State _gameState;
        private bool _inside = false;

        private static Font _font = new Font("Arial", 18);
        private static Brush _whiteBrush = new SolidBrush(Color.White);
        private static Brush _blackBrush = new SolidBrush(Color.Red);
        private static Random _random = new Random();



         public GameWorld(State gameState, Dictionary<string, Tile> tiles, string mapFile)
        {
            _gameState = gameState;
            _tiles = tiles;

            //Read in the map file
            readMapfile(mapFile);

            //Find the start point
            _currentArea = _world[_startArea];

            //Create and position the hero character
            _heroPosition = new Point(3, 3);
            _heroSprite = new Sprite(null, _heroPosition.X * Tile.TileSizeX + LocalArea.OffsetX,
                                            _heroPosition.Y * Tile.TileSizeY + LocalArea.OffsetY,
                                            _tiles["sha"].Bitmap, _tiles["sha"].Rectangle, _tiles["sha"].NumberOfFrames);
            _heroSprite.Flip = true;
            _heroSprite.ColorKey = Color.FromArgb(75, 75, 75);
        }

         private void readMapfile(string mapFile)
         {
             using (StreamReader stream = new StreamReader(mapFile))
             {
                 while (!stream.EndOfStream)
                 {
                     //Each area constructor will consume just one area
                     LocalArea area = new LocalArea(stream, _tiles);
                     _world.Add(area.Name, area);
                 }
             }
         }

        public override void Draw(Graphics graphics)
        {
            _currentArea.Draw(graphics);
            _heroSprite.Draw(graphics);
        }

        public override void Update(double gameTime, double elapsedTime)
        {
            //We only actually update the current area the rest all 'sleep'
            _currentArea.Update(gameTime, elapsedTime);

            _heroSprite.Update(gameTime, elapsedTime);

            //If the hero is moving we need to check if we are there yet
            if (_heroSpriteAnimating)
            {
                if (checkDestination())
                {
                    //We have arrived. Stop moving and animating
                    _heroSprite.Location = _heroDestination;
                    _heroSprite.Velocity = PointF.Empty;
                    _heroSpriteAnimating = false;

                  
                }
            }

            //The hero gets animated when moving or fighting
            if (_heroSpriteAnimating || _heroSpriteFighting)
            {
                _heroSprite.CurrentFrame = (int)((gameTime * 8.0) % _heroSprite.NumberOfFrames);
            }
            else
            {
                //Otherwise use frame 0
                _heroSprite.CurrentFrame = 0;
            }

            //If we are fighting then keep animating for a period of time
            if (_heroSpriteFighting)
            {
                if (_startFightTime < 0)
                {
                    _startFightTime = gameTime;
                }
                else
                {
                    if (gameTime - _startFightTime > 1.0)
                    {
                        _heroSpriteFighting = false;
                    }
                }
            }

        }

        private bool checkDestination()
        {
            //Depending on the direction we are moving we check different bounds of the destination
            switch (_direction)
            {
                case HeroDirection.Right:
                    return (_heroSprite.Location.X >= _heroDestination.X);
                case HeroDirection.Left:
                    return (_heroSprite.Location.X <= _heroDestination.X);
                case HeroDirection.Up:
                    return (_heroSprite.Location.Y <= _heroDestination.Y);
                case HeroDirection.Down:
                    return (_heroSprite.Location.Y >= _heroDestination.Y);
            }

            throw new ArgumentException("Direction is not set correctly");
        }

        public void KeyDown(Keys key)
        {
            //Ignore keypresses while we are animating or fighting
            if (!_heroSpriteAnimating)
            {
                switch (key)
                {
                    case Keys.Right:
                        //Are we at the edge of the map?
                        if (_heroPosition.X < LocalArea.MapSizeX - 1)
                        {
                            //Can we move to the next tile or not (blocking tile or monster)
                            if (checkNextTile(_currentArea.Map[_heroPosition.X + 1, _heroPosition.Y], _heroPosition.X + 1, _heroPosition.Y))
                            {
                                _heroSprite.Velocity = new PointF(100, 0);
                                _heroSprite.Flip = true;
                                _heroSpriteAnimating = true;
                                _heroSprite.Vertical = false;
                                _direction = HeroDirection.Right;
                                _heroPosition.X++;
                                setDestination();
                            }
                        }
                        break;

                    case Keys.Left:
                        //Are we at the edge of the map?
                        if (_heroPosition.X > 0) 
                        {
                            //Can we move to the next tile or not (blocking tile or monster)
                            if (checkNextTile(_currentArea.Map[_heroPosition.X - 1, _heroPosition.Y], _heroPosition.X - 1, _heroPosition.Y))
                            {
                                _heroSprite.Velocity = new PointF(-100, 0);
                                _heroSprite.Flip = false;
                                _heroSpriteAnimating = true;
                                _heroSprite.Vertical = false;
                                _direction = HeroDirection.Left;
                                _heroPosition.X--;
                                setDestination();
                            }
                        }                       
                        break;

                    case Keys.Up:
                        //Are we at the edge of the map?
                        if (_heroPosition.Y > 0)
                        {
                            //Can we move to the next tile or not (blocking tile or monster)
                            if (checkNextTile(_currentArea.Map[_heroPosition.X, _heroPosition.Y - 1], _heroPosition.X, _heroPosition.Y - 1))
                            {
                                _heroSprite.Velocity = new PointF(0, -100);
                                _heroSprite.Flip = true;
                                _heroSpriteAnimating = true;
                                _heroSprite.Vertical = true;
                                _direction = HeroDirection.Up;
                                _heroPosition.Y--;
                                setDestination();
                            }
                        }                       
                        break;

                    case Keys.Down:
                        //Are we at the edge of the map?
                        if (_heroPosition.Y < LocalArea.MapSizeY - 1)
                        {
                            //Can we move to the next tile or not (blocking tile or monster)
                            if (checkNextTile(_currentArea.Map[_heroPosition.X, _heroPosition.Y + 1], _heroPosition.X, _heroPosition.Y + 1))
                            {
                                _heroSprite.Velocity = new PointF(0, 100);
                                _heroSprite.Flip = false;
                                _heroSpriteAnimating = true;
                                _heroSprite.Vertical = true;
                                _direction = HeroDirection.Down;
                                _heroPosition.Y++;
                                setDestination();
                            }
                        }                        
                        break;                    
                }
            }
        }        

        private bool checkNextTile(MapTile mapTile, int x, int y)
        {
            checkDoors(mapTile, x, y);
            checkIndoors(mapTile, x, y);
            //If the next tile is a blocker then we can't move
            if (mapTile.Tile.IsBlock) return false;

            return true;
        }
        private void checkDoors(MapTile mapTile, int x, int y)
        {
            if (mapTile.Tile.Category == "door" && mapTile.Tile.IsBlock == true)
            {
                mapTile.Tile = _tiles["G"];
                mapTile.SetSprite(x, y);
                mapTile.Tile.IsBlock = false; 
            }
        }
        private void checkIndoors(MapTile mapTile, int x, int y)
        {           
            if (mapTile.Tile.Category == "inside" &! _inside)
            {
                indoors();
            }
            else if (mapTile.Tile.Category != "inside" && _inside)
            {
                outdoors();
            }
        }

        private void setDestination()
        {
            //Calculate the eventual sprite destination based on the area grid coordinates
            _heroDestination = new PointF(_heroPosition.X * Tile.TileSizeX + LocalArea.OffsetX,
                                            _heroPosition.Y * Tile.TileSizeY + LocalArea.OffsetY);
        }

        private void indoors()
        {
            _inside = true;
            
            int y = 0;
            int x = 0;
            foreach (MapTile tile in _currentArea.Map)
            {
                tile.Tile = _tiles[tile.Tile.Shortcut.ToString().ToLower()];
                tile.SetSprite(x, y);
                y++;
                if (y == 0 && x == 0) x++;

                if (y > LocalArea.MapSizeX - 1) y = 0;
                if (y == 0) x++;

            }         
        }

        private void outdoors()
        {
            _inside = false;
            int y = 0;
            int x = 0;
            foreach (MapTile tile in _currentArea.Map)
            {
                tile.Tile = _tiles[tile.Tile.Shortcut.ToString().ToUpper()];
                tile.SetSprite(x, y);
                y++;
                if (y == 0 && x == 0) x++;

                if (y > LocalArea.MapSizeX - 1) y = 0;
                if (y == 0) x++;

            }         
        }

        private enum HeroDirection
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}
