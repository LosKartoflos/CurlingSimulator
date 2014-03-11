using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CurlingSimulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int SCREENWIDTH = 800;
        public const int SCREENHEIGHT = 600; 
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D powerbarText; //image files

        Vector2 powerbarPos = new Vector2(50, 480);

        float powerbarRot;

        Vector2 powerbarCenter;
        int powerbarHeight, powerbarWidth;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferHeight = SCREENHEIGHT;
            graphics.PreferredBackBufferWidth = SCREENWIDTH;

            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 

        // Set the 3D model to draw.
        CStone[] m_stones;

        // Sets the Aspect Ratio (scales the 3D to 2D projection 
        float aspectRatio;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            powerbarText = Content.Load<Texture2D>("powerbar_full");

            powerbarHeight = powerbarText.Height;
            powerbarWidth = powerbarText.Width;

            powerbarCenter = new Vector2(powerbarWidth / 2, powerbarHeight / 2);

            // TODO: use this.Content to load your game content here

            //Loads the 3D Model
            m_stones = new CStone[6];
            modelPosition = new Vector3[6];
            for (int i = 0; i < 6; ++i)
            {
                m_stones[i] = new CStone(Content.Load<Model>("Models\\Curlingstein"), 0, 0, 0);
                modelPosition[i] = Vector3.Zero;
            }

            //sets the Aspect Ratio
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //Makes the Modelrotating
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
            MathHelper.ToRadians(0.1f);

            KeyboardState keyboard = Keyboard.GetState();

            // Richtungen
            Vector3 left = new Vector3(-1, 0, 0);
            Vector3 right = new Vector3(1, 0, 0);
            Vector3 forward = new Vector3(0, 0, -1);
            Vector3 backward = new Vector3(0, 0, 1);

            // Tastatur anwenden
            if (keyboard.IsKeyDown(Keys.Left)) modelPosition[0] += left;
            if (keyboard.IsKeyDown(Keys.Right)) modelPosition[0] += right;
            if (keyboard.IsKeyDown(Keys.Up)) modelPosition[0] += forward;
            if (keyboard.IsKeyDown(Keys.Down)) modelPosition[0] += backward;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        // Set the position of the model and set the rotation.
        Vector3[] modelPosition;
        float modelRotation = 0.0f;

        // Set the position of the camera 
        Vector3 cameraPosition = new Vector3(0.0f, 50.0f, 100.0f);
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);//start drawing 2D IMages
            spriteBatch.Draw(powerbarText, powerbarPos, null, Color.White, powerbarRot, powerbarCenter, 0.05f, SpriteEffects.None, 0.0f);
            base.Draw(gameTime);
            spriteBatch.End();

            for (int i = 0; i < 6; i++)
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[m_stones[i].getModel().Bones.Count];
                m_stones[i].getModel().CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in m_stones[i].getModel().Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] *
                            Matrix.CreateRotationY(modelRotation)
                            * Matrix.CreateTranslation(modelPosition[i]);
                        effect.View = Matrix.CreateLookAt(cameraPosition,
                            Vector3.Zero, Vector3.Up);
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.ToRadians(45.0f), aspectRatio,
                            1.0f, 10000.0f);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }


            base.Draw(gameTime);
        }
    }
}
