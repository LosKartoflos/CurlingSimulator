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
    public class Main : Microsoft.Xna.Framework.Game
    {
        public const int SCREENWIDTH = 800;
        public const int SCREENHEIGHT = 600; 
        
        // Graphic draw stuff
        GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;
        float m_aspectRatio;

        // Previous Keyboard State
        KeyboardState m_keyboardState;

        // Previous moving state
        bool m_wasMoving;

        // Powerbar
        CPowerBar m_powerBar;
        bool m_moveSlider;

        // Models
        // Stones
        CStone[] m_stones;
        int m_idCurrentStone;
        float m_stoneRotation;
        Vector3 m_zeroPosition;

        // Floor
        Floor m_iceFloor;
        Vector3 m_iceFloorPos;
        float m_iceFloorRotation;

        // Camera
        Vector3 m_cameraPosition;
        Vector3 m_cameraPositionOffset;


        public Main()
        {
            m_graphics = new GraphicsDeviceManager(this);

            m_graphics.PreferredBackBufferHeight = SCREENHEIGHT;
            m_graphics.PreferredBackBufferWidth = SCREENWIDTH;

            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            m_keyboardState = Keyboard.GetState();
            m_cameraPositionOffset = new Vector3(0.0f, 50.0f, 100.0f);
            m_cameraPosition = m_cameraPositionOffset;
            m_iceFloorRotation = 1.60f;
            m_iceFloorPos = new Vector3(16, -3, 0);
            m_stoneRotation = 0.0f;
            m_idCurrentStone = 0;
            m_moveSlider = true;
            m_zeroPosition = new Vector3(0, 0, 0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            m_powerBar = new CPowerBar(Content.Load<Texture2D>("powerbar_full"), new Vector2(20, 450));
            m_powerBar.setSlider(Content.Load<Texture2D>("powerbar_slider"));

            // TODO: use this.Content to load your game content here

            m_stones = new CStone[6];
            for (int i = 0; i < 6; ++i)
            {
                m_stones[i] = new CStone(Content.Load<Model>("Models\\Curlingstein"), 0, 0, 0);
            }
                m_iceFloor = new Floor(Content.Load<Model>("Models\\EisFlaeche"), 0, 0, 0);

            //sets the Aspect Ratio
            m_aspectRatio = m_graphics.GraphicsDevice.Viewport.AspectRatio;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            bool bNoMoreStones = true;
            for (int i = 0; i < 6; ++i)
            {
                if (m_stones[i].getPosition() == m_zeroPosition || m_stones[i].getVx() != 0 || m_stones[i].getVy() != 0)
                {
                    bNoMoreStones = false;
                    break;
                }
            }

            if (bNoMoreStones)
            {
                for (int i = 0; i < 6; ++i)
                {
                    m_stones[i].setPosition(m_zeroPosition);
                }
            }

            //Makes the Modelrotating
            m_stoneRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
            MathHelper.ToRadians(0.1f);

            bool somethingMoving = false;
            for (int i = 0; i < 6; i++)
            {
                m_stones[i].setPosition(m_stones[i].getPosition() + new Vector3(0, 0, m_stones[i].getVy()));
                if (m_stones[i].getVy() != 0 || m_stones[i].getVx() != 0)
                {
                    somethingMoving = true;
                    break;
                }
            }
            if (!somethingMoving && m_wasMoving)
            {
                m_powerBar.setZero();
                m_moveSlider = true;
            }

            m_wasMoving = somethingMoving;

            bool spaceWasDown = m_keyboardState.IsKeyDown(Keys.Space);
            m_keyboardState = Keyboard.GetState();
            if (spaceWasDown && m_keyboardState.IsKeyUp(Keys.Space) && !somethingMoving)
            {
                m_idCurrentStone++;
                if (m_idCurrentStone == 6)
                    m_idCurrentStone = 0;
                float speed = m_powerBar.getValue() * -100;
                m_stones[m_idCurrentStone].setVy((int)speed);
                m_moveSlider = false;
            }
            for (int i = 0; i < 6; i++)
            {
                m_stones[i].setPosition(m_stones[i].getPosition() + new Vector3(0, 0, m_stones[i].getVy()));
                if (m_stones[i].getVy() != 0 || m_stones[i].getVx() != 0)
                {
                    m_stones[i].applyResistance();
                }
            }

            if (m_moveSlider)
                m_powerBar.update();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            m_spriteBatch.Begin(SpriteBlendMode.AlphaBlend);//start drawing 2D IMages
            m_powerBar.draw(m_spriteBatch);
            m_spriteBatch.End();

            DrawStone();

            DrawFloor();

            base.Draw(gameTime);
        }

        private void DrawStone()
        {
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
                            Matrix.CreateRotationY(m_stoneRotation)
                            * Matrix.CreateTranslation(m_stones[i].getPosition());
                        effect.View = Matrix.CreateLookAt(m_cameraPosition,
                            Vector3.Zero, Vector3.Up);
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.ToRadians(45.0f), m_aspectRatio,
                            1.0f, 10000.0f);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }

        private void DrawFloor()
        {

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[m_iceFloor.getModel().Bones.Count];
            m_iceFloor.getModel().CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in m_iceFloor.getModel().Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(3.0f, 3.0f, 3.0f); // a red light
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(m_iceFloorRotation)
                        * Matrix.CreateTranslation(m_iceFloorPos);
                    effect.View = Matrix.CreateLookAt(m_cameraPosition,
                        Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), m_aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
     
            }
        }
    }
}
