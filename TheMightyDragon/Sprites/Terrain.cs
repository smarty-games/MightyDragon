using System;
using Microsoft.Xna.Framework.Graphics;
using Desktop.Sprites;
using Microsoft.Xna.Framework;

namespace Desktop.Sprites
{
    public class Terrain:Sprite
    {

        public Texture2D TerrainTexture;

        public Terrain(Texture2D terrain):base(terrain)
        {
            TerrainTexture = terrain;
        }
        public void Init(Vector2 position)
        {

            StepX = 32;
            StepY = 32;
            Position = position;

        }

        internal Terrain Duplicate()
        {
            return new Terrain(TerrainTexture);
        }
    }
}
