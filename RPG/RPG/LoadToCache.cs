using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RPG
{
    class LoadToCache
    {
        private static Dictionary<string, Bitmap> _bitmaps = new Dictionary<string, Bitmap>();

        public Bitmap this[string filename]
        {
            get
            {
                //Load bitmap to cache if not already loaded
                if (!_bitmaps.ContainsKey(filename))
                {
                    _bitmaps.Add(filename, new Bitmap(filename));
                }
                return _bitmaps[filename];
            }
        }
    }
}
