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
        GameTime m_previousGameTime;

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
        float m_stoneScale;
        Vector3 m_zeroPosition;

        // Floor
        Floor m_iceFloor;
        Vector3 m_iceFloorPos;
        float m_iceFloorScale;
        float m_iceFloorRotation;

        // Camera
        Vector3 m_cameraPosition;
        Vector3 m_cameraPositionOffset;
        Vector3 m_cameraLookAt;

        //Soundeffekte
        SoundEffect soundAnfang;
        SoundEffect soundMitte;
        SoundEffect soundEnde;
        SoundEffect soundBlow;
        SoundEffect soundCrowdBoo;
        SoundEffect soundCrowdJubel;
        SoundEffect soundSoundTrackAction;
        SoundEffect soundSoundTrackSports;
        SoundEffect soundHit;
        SoundEffect soundSubmarine;
        SoundEffect soundIce;

        SoundEffectInstance soundMitteLoop;
        SoundEffectInstance soundTrackSportsLoop;


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
            m_cameraPositionOffset = new Vector3(0.0f, 200.0f, 600.0f);
            m_cameraPosition = m_cameraPositionOffset;
            m_iceFloorRotation = 0.0f;
            m_iceFloorPos = new Vector3(0, -3, -20);
            m_iceFloorScale = 300;
            m_stoneRotation = 0.0f;
            m_stoneScale = 6;
            m_idCurrentStone = 0;
            m_moveSlider = true;
            m_zeroPosition = new Vector3(0, 0, 0);
            m_previousGameTime = new GameTime();


            m_cameraLookAt = new Vector3(0, 0, 0);
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
                m_iceFloor = new Floor(Content.Load<Model>("Models\\EisFlaeche2"), 0, 0, 0);
            //sets the Aspect Ratio
            m_aspectRatio = m_graphics.GraphicsDevice.Viewport.AspectRatio;

            // Load Sound Files
            soundAnfang = Content.Load<SoundEffect>("Sounds\\01_Anfang");
            soundMitte = Content.Load<SoundEffect>("Sounds\\02_Mitte");
            soundEnde = Content.Load<SoundEffect>("Sounds\\03_Ende");
            soundBlow = Content.Load<SoundEffect>("Sounds\\04_Blow");
            soundCrowdBoo = Content.Load<SoundEffect>("Sounds\\05_CrowdBoo");
            soundCrowdJubel = Content.Load<SoundEffect>("Sounds\\06_CrowdJubel");
            soundSoundTrackAction = Content.Load<SoundEffect>("Sounds\\07_SoundTrackAction");
            soundSoundTrackSports = Content.Load<SoundEffect>("Sounds\\08_SoundTrackSports");
            soundHit = Content.Load<SoundEffect>("Sounds\\09_Hit");
            soundSubmarine = Content.Load<SoundEffect>("Sounds\\10_Submarine");
            soundIce = Content.Load<SoundEffect>("Sounds\\11_Ice");

            // Loop Sound "07_SoundTrackSport" (Hintergrundmusik)
            soundTrackSportsLoop = soundSoundTrackSports.CreateInstance();
            soundTrackSportsLoop.IsLooped = true;
            soundTrackSportsLoop.Play();
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

            // Check if any stone is moving
            bool somethingMoving = false;
            for (int i = 0; i < 6; i++)
            {
                m_stones[i].setPosition(m_stones[i].getPosition() + new Vector3(0, 0, (int)m_stones[i].getVy()));
                if ((int)m_stones[i].getVy() != 0 || (int)m_stones[i].getVx() != 0)
                {
                    somethingMoving = true;
                    // Check if colliding with other stone
                    for (int j = 0; j < 6; j++)
                    {
                        if (j != i)
                        {
                            m_stones[i].checkCollisionWith(m_stones[j]);
                        }
                    }
                }
            }
            if (!somethingMoving && m_wasMoving)
            {
                m_powerBar.setZero();
                m_moveSlider = true;
            }

            // Shoot new stone
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

                // Loop Sound "02_Mitte"
                //soundMitteLoop = soundMitte.CreateInstance();
                //soundMitteLoop.IsLooped = true;
                //soundMitteLoop.Play();

                // Sound "02_Mitte"
                soundMitte.Play();
            }

            // Reset stones if every stone was played and stopped moving
            bool bNoMoreStones = true;
            for (int i = 0; i < 6; ++i)
            {
                if (m_stones[i].getPosition() == m_zeroPosition || (int)m_stones[i].getVx() != 0 || (int)m_stones[i].getVy() != 0)
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



            if ((gameTime.TotalGameTime - m_previousGameTime.TotalGameTime).Milliseconds >= 50)
            {

                // Apply Resistance
                for (int i = 0; i < 6; i++)
                {
                    m_stones[i].setPosition(m_stones[i].getPosition() + new Vector3(0, 0, (int)m_stones[i].getVy()));
                    if (m_stones[i].getVy() != 0 || m_stones[i].getVx() != 0)
                    {
                        m_stones[i].applyResistance();
                    }
                }

                // Update Powerbar
                if (m_moveSlider)
                    m_powerBar.update();

                m_previousGameTime = new GameTime(gameTime.TotalRealTime, gameTime.ElapsedGameTime, gameTime.TotalGameTime, gameTime.ElapsedGameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            m_spriteBatch.Begin(SpriteBlendMode.AlphaBlend);//start drawing 2D IMages
            m_powerBar.draw(m_spriteBatch);
            m_spriteBatch.End();
            DrawFloor();
            DrawStone();

            

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
                            * Matrix.CreateTranslation(m_stones[i].getPosition())
                            * Matrix.CreateScale(m_stoneScale);
                        effect.View = Matrix.CreateLookAt(m_cameraPosition,
                            m_cameraLookAt, Vector3.Up);
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
                        * Matrix.CreateTranslation(m_iceFloorPos)
                        * Matrix.CreateScale (m_iceFloorScale);
                    effect.View = Matrix.CreateLookAt(m_cameraPosition,
                        m_cameraLookAt, Vector3.Up);
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
