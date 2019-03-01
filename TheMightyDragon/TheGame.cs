﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Desktop.Models;
using Desktop.Sprites;
using System;

namespace Desktop
{

    /// <summary> 
    /// -------------------------------------GAME IDEAS -----------------------------------------------------
    ///         Closed terrains will grow into MOUTAINS (resized textures of a moutain)
    /// -------------------------------------IMAGINE IF -----------------------------------------------------
    /// </summary>
    public class TheGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
         
        public int[][] GroundMap = new int[General.TilesVertically][] { new int[General.TilesHorizontaly] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1 },
                new int[General.TilesHorizontaly] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };
        // matrix copycat
        public int[][] PlayerMap;
        public Dictionary<string, Sprite> Sprites;
        private Texture2D Path;


        // 
        // ----------------------------- use GIT DESKTOP for git, ex. new repo ------------------------------------------------
        //
        public TheGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ScreenManager.Width = GraphicsDevice.Viewport.Width;
            ScreenManager.Height = GraphicsDevice.Viewport.Height;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // NOTE: I no-longer use this reference as it affects different objects if being used multiple times!
            var playeraAnimations = new Dictionary<string, Animation>()
              {
                { "WalkUp", new Animation(Content.Load<Texture2D>("Player/WalkingUp"), 3,0.1f) },
                { "WalkDown", new Animation(Content.Load<Texture2D>("Player/WalkingDown"), 3, 0.1f) },
                { "WalkLeft", new Animation(Content.Load<Texture2D>("Player/WalkingLeft"), 3,0.1f) },
                { "WalkRight", new Animation(Content.Load<Texture2D>("Player/WalkingRight"), 3,0.1f) },
                { "IdleUp", new Animation(Content.Load<Texture2D>("Player/IdleUp"), 3,0.1f) },
                { "IdleDown", new Animation(Content.Load<Texture2D>("Player/IdleDown"), 3, 0.1f) },
                { "IdleLeft", new Animation(Content.Load<Texture2D>("Player/IdleLeft"), 3,0.1f) },
                { "IdleRight", new Animation(Content.Load<Texture2D>("Player/IdleRight"), 3,0.1f) },
              };
            var dragonAnimations = new Dictionary<string, Animation>() {
                { "IdleUp", new Animation(Content.Load<Texture2D>("Dragon/IdleUp"), 3, 0.2f) },
                { "IdleDown", new Animation(Content.Load<Texture2D>("Dragon/IdleDown"), 3, 0.2f) },
                { "IdleLeft", new Animation(Content.Load<Texture2D>("Dragon/IdleLeft"), 3, 0.2f) },
                { "IdleRight", new Animation(Content.Load<Texture2D>("Dragon/IdleRight"), 3, 0.2f) },
                { "WalkUp", new Animation(Content.Load<Texture2D>("Dragon/WalkingUp"), 3, 0.2f) },
                { "WalkDown", new Animation(Content.Load<Texture2D>("Dragon/WalkingDown"), 3, 0.2f) },
                { "WalkLeft", new Animation(Content.Load<Texture2D>("Dragon/WalkingLeft"), 3, 0.2f) },
                { "WalkRight", new Animation(Content.Load<Texture2D>("Dragon/WalkingRight"), 3, 0.2f) },
                { "AttackUp", new Animation(Content.Load<Texture2D>("Dragon/AttackUp"), 3, 0.2f) },
                { "AttackDown", new Animation(Content.Load<Texture2D>("Dragon/AttackDown"), 3, 0.2f) },
                { "AttackLeft", new Animation(Content.Load<Texture2D>("Dragon/AttackLeft"), 3, 0.2f) },
                { "AttackRight", new Animation(Content.Load<Texture2D>("Dragon/AttackRight"), 3, 0.2f) }
                };

            Sprites = new Dictionary<string, Sprite>()
              {
                {"player", new Player(playeraAnimations,this)
                {
                  Input = new Input()
                  {
                    Up = Keys.W,
                    Down = Keys.S,
                    Left = Keys.A,
                    Right = Keys.D,
                  },
                  MoveType = General.eMoveType.ToTile,
                  Name = "me",
                }
                },
                {"dragon",
                new Dragon(dragonAnimations,this)
                {
                  Input = new Input()
                  {
                    Up = Keys.Up,
                    Down = Keys.Down,
                    Left = Keys.Left,
                    Right = Keys.Right,
                  },
                  MoveType = General.eMoveType.ToColission,
                  Name = "dragon",
                }
                }
            };
            Path = Content.Load<Texture2D>("Terrain/Path");
            ((Player)Sprites["player"]).Init();
            ((Dragon)Sprites["dragon"]).Init();

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            foreach (var source in Sprites.Values)
            {
                if (!(source is Terrain)) source.Update(gameTime, source);
            }
           base.Update(gameTime);
        }
   
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
                DrawMap();
                DrawCharacters();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawCharacters()
        {
            // than draw characters
            foreach (Sprite sprite in Sprites.Values)
            {
                if (!(sprite is Terrain))
                {
                    sprite.Draw(spriteBatch);
                }
            }
        }

        protected void DrawMap()
        {
            // draw map first - Terrain
            for (int line = 0; line < General.TilesVertically; line++)
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    if (GroundMap[line][col] == 1)
                    {

                        string mapSpriteKey = "tile" + line.ToString() + col.ToString();
                        Terrain mapSprite;
                        // try to fetch an existing terrain sprite
                        if (Sprites.ContainsKey(mapSpriteKey))
                        {
                            mapSprite = (Terrain)Sprites[mapSpriteKey];
                        }

                        else // create new sprite with terrain texture if not found for in GroundMap matrix
                        {

                            mapSprite = new Terrain(Path)
                            {
                                Position = new Vector2(col * General.TileSize, line * General.TileSize)
                            };
                            Sprites.Add(mapSpriteKey, mapSprite);
                        }

                        mapSprite.Draw(spriteBatch);
                    }
                }
        }
    }
}
