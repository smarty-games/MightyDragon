﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Desktop.Models;
using Desktop.Sprites;

namespace Desktop
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TheGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int ScreenWidth;
        int ScreenHeight;

        private List<Sprite> _sprites;

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
             ScreenHeight = GraphicsDevice.Viewport.Height;
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

      _sprites = new List<Sprite>()
      {
        new Sprite(playeraAnimations)
        {
          
          Input = new Input()
          {
            Up = Keys.W,
            Down = Keys.S,
            Left = Keys.A,
            Right = Keys.D,
          },
          MoveType = General.CharacterMoveType,
          Name = "me",
          PlayerType = General.ePlayerType.Player
        },
        new Sprite(new Dictionary<string, Animation>()
        {
          { "IdleUp", new Animation(Content.Load<Texture2D>("Dragon/IdleUp"), 3,0.2f) },
          { "IdleDown", new Animation(Content.Load<Texture2D>("Dragon/IdleDown"), 3,0.2f) },
          { "IdleLeft", new Animation(Content.Load<Texture2D>("Dragon/IdleLeft"), 3, 0.2f) },
          { "IdleRight", new Animation(Content.Load<Texture2D>("Dragon/IdleRight"), 3, 0.2f) },
          { "WalkUp", new Animation(Content.Load<Texture2D>("Dragon/WalkingUp"), 3,0.2f) },
          { "WalkDown", new Animation(Content.Load<Texture2D>("Dragon/WalkingDown"), 3,0.2f) },
          { "WalkLeft", new Animation(Content.Load<Texture2D>("Dragon/WalkingLeft"), 3, 0.2f) },
          { "WalkRight", new Animation(Content.Load<Texture2D>("Dragon/WalkingRight"), 3, 0.2f) },
          { "AttackUp", new Animation(Content.Load<Texture2D>("Dragon/AttackUp"), 3,0.2f) },
          { "AttackDown", new Animation(Content.Load<Texture2D>("Dragon/AttackDown"), 3,0.2f) },
          { "AttackLeft", new Animation(Content.Load<Texture2D>("Dragon/AttackLeft"), 3, 0.2f) },
          { "AttackRight", new Animation(Content.Load<Texture2D>("Dragon/AttackRight"), 3, 0.2f) }

        })
        {
          
          Input = new Input()
          {
            Up = Keys.Up,
            Down = Keys.Down,
            Left = Keys.Left,
            Right = Keys.Right,
          },
          MoveType = General.CharacterMoveType,
          Name = "dragon",
          PlayerType = General.ePlayerType.Dragon
        },
      };
            _sprites.ForEach(s => s.SetStartPosition());
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
      foreach (var sprite in _sprites)
        sprite.Update(gameTime, sprite);

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

      foreach (var sprite in _sprites)
        sprite.Draw(spriteBatch);

      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
