using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RPG
{
    public abstract class GameObject
    {
        public abstract void Update(double gameTime, double elapsedTime);
        public abstract void Draw(Graphics graphics);
    }
}
