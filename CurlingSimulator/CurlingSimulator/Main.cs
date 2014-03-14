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

        // Sprite Font
        private SpriteFont spriteFont;

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

        // Diversion
        Diversion m_diversion;

        // Point Counter
        int m_pointCounter;
        int m_score;

        // Stone Counter
        int m_stoneCounter;

        // Models
        // Stones
        int m_numberOfStones;
        CStone[] m_stones;
        CStone[] m_stonesSecond;
        int m_idCurrentStone;
        float m_stoneRotation;
        float m_stoneScale;
        Vector3 m_zeroPosition;
        Vector3 m_startPosition;

        // Floor
        Floor m_iceFloor;
        Vector3 m_iceFloorPos;
        float m_iceFloorScale;
        float m_iceFloorRotation;

        //Arrow
        Arrow m_arrow;
        Vector3 m_arrowPos;
        float m_arrowScale;
        float m_arrowRotation;

        //Hall
        Hall m_hall;
        Vector3 m_hallPos;
        float m_hallScale;
        float m_hallRotation;

        // Camera
        Vector3 m_cameraPosition;
        Vector3 m_cameraPositionOffset;
        Vector3 m_cameraPositionOffsetField;
        Vector3 m_cameraPositionAllField;
        Vector3 m_cameraLookAt;
        Vector3 m_cameraLookAtOffsetField;
        Vector3 m_cameraLookAtAllField;
        int m_stoneIdCamera;

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

        //SoundEffectInstance soundMitteLoop;
        SoundEffectInstance soundTrackSportsLoop;

        //Videos
        Video vid;
        VideoPlayer vidplayer;

        Texture2D vidTexture;
        Rectangle vidRectangle;

        //Startscreen
        bool s; //Play
        bool c; //Control
        Texture2D ControlStartscreen;

        //Controller Connection
        Texture2D Connected;
        Texture2D Disconnected;
        bool v; //Connected Anzeige

        GamePadState gamePadState;

        //Endscreen
        Texture2D ScoreBeat;
        Texture2D ScoreFail;

        int scfa;

        public Main()
        {
            m_graphics = new GraphicsDeviceManager(this);

            m_graphics.PreferredBackBufferHeight = SCREENHEIGHT;
            m_graphics.PreferredBackBufferWidth = SCREENWIDTH;

            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            gamePadState = GamePad.GetState(PlayerIndex.One);

            v = false; //connected anzeige
            s = true; //Startscreen, Play
            c = false; //Startscreen, Control

            vidplayer = new VideoPlayer();

            m_keyboardState = Keyboard.GetState();
            //Camera
            m_cameraPositionOffset = new Vector3(0.0f, 7.0f, 20.0f);
            m_cameraPositionOffsetField = new Vector3(0.0f, 130.0f, 16.5f);
            m_cameraLookAtOffsetField = new Vector3(0f, 0, 16.0f);
            m_cameraPosition = m_cameraPositionOffset;
            m_cameraPositionAllField = new Vector3(0.0f, 130.0f, 85.0f);
            m_cameraLookAtAllField = new Vector3(0.0f, 30.0f, -50.0f);
            m_stoneIdCamera = 0;
            //IceFloor
            m_iceFloorRotation = 0.0f;
            m_iceFloorPos = new Vector3(0.0f, -0.7f, -400.0f);
            m_iceFloorScale = 1;
            //Arrow
            m_arrowPos = new Vector3(0.0f, 1.0f, 0.0f);
            m_arrowRotation = 0;
            m_arrowScale = 1;
            //Hall
            m_hallPos = new Vector3(0.0f, 0.0f, -50.0f);
            m_hallRotation = 0.0f;
            m_hallScale = 1;
            //Stone
            m_numberOfStones = 8;
            m_stoneRotation = 0.0f;
            m_stoneScale = 1;
            m_idCurrentStone = -1;
            m_moveSlider = true;
            m_zeroPosition = new Vector3(0, 0, 500);
            m_startPosition = new Vector3(0, 0, 0);
            m_previousGameTime = new GameTime();

            m_pointCounter = 0;
            m_score = 0;

            m_stoneCounter = 0;

            m_cameraLookAt = new Vector3(0, 0, 0);
            base.Initialize();
        }

        protected override void LoadContent()
        {

            // Load Video
            vid = Content.Load<Video>("StartScreen");
            vidRectangle = new Rectangle(GraphicsDevice.Viewport.X - 138, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width + 280, GraphicsDevice.Viewport.Height);
            vidplayer.Play(vid);

            //Endscreen
            ScoreBeat = Content.Load<Texture2D>("ScoreBeat");
            ScoreFail = Content.Load<Texture2D>("ScoreFail");


            //Startscreen_Control
            ControlStartscreen = Content.Load<Texture2D>("Control_Startscreen");
            Connected = Content.Load<Texture2D>("Connected");
            Disconnected = Content.Load<Texture2D>("Disconnected");

            // Create a new SpriteBatch, which can be used to draw textures.
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("MyFont");

            m_powerBar = new CPowerBar(Content.Load<Texture2D>("powerbar_full"), new Vector2(20, 450));
            m_powerBar.setSlider(Content.Load<Texture2D>("powerbar_slider"));

            m_diversion = new Diversion(new Vector2(390, 400));
            m_diversion.setSlider(Content.Load<Texture2D>("powerbar_slider"));

            // TODO: use this.Content to load your game content here

            m_stones = new CStone[m_numberOfStones];
            for (int i = 0; i < m_numberOfStones; ++i)
            {
                m_stones[i] = new CStone(Content.Load<Model>("Models\\CurlingsteinRed"), 0, 0, 500);
            }
            m_stones[0].setPosition(m_startPosition);

            m_iceFloor = new Floor(Content.Load<Model>("Models\\EisFlaeche10"), 0, 0, 0);

            m_arrow = new Arrow(Content.Load<Model>("Models\\Arrow"), 0, 0, 0);

            m_hall = new Hall(Content.Load<Model>("Models\\Halle4"), 0, 0, 0);


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


        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            m_stoneCounter = m_numberOfStones - m_stoneIdCamera - 1;

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                this.Exit();


            if ((gameTime.TotalGameTime - m_previousGameTime.TotalGameTime).Milliseconds >= 20)
            {
                // Check if any stone is moving
                bool somethingMoving = false;
                for (int i = 0; i < m_numberOfStones; i++)
                {
                    m_stones[i].setPosition(m_stones[i].getPosition() + new Vector3(0, 0, m_stones[i].getVy()));
                    if (m_stones[i].getVy() <= -0.011f || m_stones[i].getVx() >= 0.011f)
                    {
                        somethingMoving = true;
                        // Check if colliding with other stone
                        for (int j = 0; j < m_numberOfStones; j++)
                        {
                            if (j != i)
                            {
                                if (m_stones[i].checkCollisionWith(m_stones[j]))

                                    break;
                            }
                        }
                    }
                }
                if (!somethingMoving && m_wasMoving)
                {
                    m_powerBar.setZero();
                    m_moveSlider = true;
                    int nextId = m_idCurrentStone + 1;

                    if (nextId == m_numberOfStones)
                        nextId = 0;
                    m_stoneIdCamera = nextId;
                    m_stones[nextId].setPosition(m_startPosition);
                    m_arrowPos = new Vector3(0.0f, 1.0f, 0.0f);
                    if (nextId == 0)
                    {
                        for (int i = 0; i < m_numberOfStones; i++)
                        {
                            //Count this Rounds Score
                            Vector3 currentStone = m_stones[i].getPosition();
                            // Mittels Pythagoras Abstand zwischen aktuellem Stein und Zielpunkt(0,3,-400) berechnen, Zielradius ist 21
                            if ((Math.Sqrt(Math.Pow(currentStone.X, 2f) + Math.Pow(currentStone.Z + 400f, 2f)) + 1.5f <= 21f) && (Math.Sqrt(Math.Pow(currentStone.X, 2f) + Math.Pow(currentStone.Z + 400f, 2f)) + 1.5f > 15f))
                                m_pointCounter++;
                            else if ((Math.Sqrt(Math.Pow(currentStone.X, 2f) + Math.Pow(currentStone.Z + 400f, 2f)) + 1.5f <= 15f) && (Math.Sqrt(Math.Pow(currentStone.X, 2f) + Math.Pow(currentStone.Z + 400f, 2f)) + 1.5f > 9f))
                                m_pointCounter += 2;
                            else if ((Math.Sqrt(Math.Pow(currentStone.X, 2f) + Math.Pow(currentStone.Z + 400f, 2f)) + 1.5f <= 9f) && (Math.Sqrt(Math.Pow(currentStone.X, 2f) + Math.Pow(currentStone.Z + 400f, 2f)) + 1.5f > 3f))
                                m_pointCounter += 5;
                            else if (Math.Sqrt(Math.Pow(currentStone.X, 2f) + Math.Pow(currentStone.Z + 400f, 2f)) + 1.5f <= 3f)
                                m_pointCounter += 10;

                            m_stones[i].setPosition(m_zeroPosition);
                        }
                        m_score = m_pointCounter;
                        if (m_score >= 10)
                        { scfa = 1; }
                        if (m_score < 10)
                        { scfa = 2; }
                        m_pointCounter = 0;
                        m_stones[0].setPosition(m_startPosition);
                    }
                }

                // Shoot new stone
                m_wasMoving = somethingMoving;
                bool spaceWasDown = m_keyboardState.IsKeyDown(Keys.Space);
                m_keyboardState = Keyboard.GetState();
                if (((spaceWasDown && m_keyboardState.IsKeyUp(Keys.Space)) || gamePadState.Buttons.A == ButtonState.Pressed) && !somethingMoving)
                {
                    m_idCurrentStone++;
                    if (m_idCurrentStone == m_numberOfStones)
                        m_idCurrentStone = 0;
                    float speed = m_powerBar.getValue() * -4.05f;
                    if (speed <= 0.011)
                    {
                        m_stones[m_idCurrentStone].setVy(speed);
                        float div = m_diversion.getValue() * speed * -0.3f;
                        m_stones[m_idCurrentStone].setVx(div);
                    }
                    if (speed <= 0.011)
                        m_moveSlider = false;
                    m_diversion.setZero();

                    // Loop Sound "02_Mitte"
                    //soundMitteLoop = soundMitte.CreateInstance();
                    //soundMitteLoop.IsLooped = true;
                    //soundMitteLoop.Play();

                    // Sound "02_Mitte"
                    soundMitte.Play();
                }



                //Makes the Modelrotating
                m_stoneRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
                MathHelper.ToRadians(0.1f);

                if (m_moveSlider)
                    m_powerBar.update();

                // Update Diversion
                if (m_keyboardState.IsKeyDown(Keys.Left))
                    m_diversion.moveLeft();

                if (m_keyboardState.IsKeyDown(Keys.Right))
                    m_diversion.moveRight();

                m_arrowRotation = m_diversion.getValue() * -0.158f;


                // Apply Resistance
                for (int i = 0; i < m_numberOfStones; i++)
                {
                    m_stones[i].setPosition(m_stones[i].getPosition() + new Vector3(m_stones[i].getVx(), 0, m_stones[i].getVy()));
                    if (m_stones[i].getVy() < -0.011f || m_stones[i].getVx() > 0.011f)
                    {
                        m_stones[i].applyResistance();
                    }
                }

                //Watches values
                Console.WriteLine("m_cameraPostion, x: " + m_cameraPosition.X + " y: " + m_cameraPosition.Y + " z: " + m_cameraPosition.Z);
                //Console.WriteLine("m_stonePostion, x: " + m_stones[m_idCurrentStone].getPosition().X + " y: " + m_stones[m_idCurrentStone].getPosition().Y + " z: " + m_stones[m_idCurrentStone].getPosition().Z);

                if (m_keyboardState.IsKeyDown(Keys.P))
                {
                    s = false;
                    vidplayer.Stop();
                    // Loop Sound "07_SoundTrackSport" (Hintergrundmusik)

                    soundTrackSportsLoop = soundSoundTrackSports.CreateInstance();
                    soundTrackSportsLoop.IsLooped = true;
                    soundTrackSportsLoop.Play();

                }

                if (m_keyboardState.IsKeyDown(Keys.C))
                {
                    c = true;
                    s = false;
                }

                if (m_keyboardState.IsKeyDown(Keys.Escape))
                {
                    s = true;
                    c = false;
                    scfa = 0;
                }

                if (gamePadState.IsConnected)
                {
                    v = true;

                }
                else
                {
                    v = false;
                }
                Console.WriteLine("m_stonePos.Y: " + m_stones[m_stoneIdCamera].getPosition().Z);
                //Resets if stone is outside of the field. (Places the stone somewhere else and sets the speed to zero)

                for (int i = 0; i < 8; i++)
                {
                    if (m_stones[i].getPosition().X > 23.5 || m_stones[i].getPosition().X < -23.5 || m_stones[i].getPosition().Z < -440.0)
                    {
                        m_stones[i].setPosition(new Vector3(100.0f, 0.0f, 0.0f));

                        m_stones[i].setVx(0.0f);
                        m_stones[i].setVy(0.0f);


                    }
                    if (m_stones[i].getPosition().Z > -333.0 && m_stones[i].getVx() == 0.0f && m_stones[i].getVy() == 0.0f && m_stones[i].getPosition().Z < 0.0)
                    {
                        m_stones[i].setPosition(new Vector3(100.0f, 0.0f, 0.0f));

                        m_stones[i].setVx(0.0f);
                        m_stones[i].setVy(0.0f);


                    }
                }


                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit(); // Exit Game

                if (gamePadState.Buttons.X == ButtonState.Pressed) //control gamepad
                {
                    c = true;
                    s = false;
                }

                if (gamePadState.Buttons.B == ButtonState.Pressed) //go back gamepad
                {
                    s = true;
                    c = false;
                    scfa = 0;
                }

                if (gamePadState.Buttons.Y == ButtonState.Pressed) //Play gamepad
                {
                    s = false;
                    vidplayer.Stop();
                    // Loop Sound "07_SoundTrackSport" (Hintergrundmusik)

                    soundTrackSportsLoop = soundSoundTrackSports.CreateInstance();
                    soundTrackSportsLoop.IsLooped = true;
                    soundTrackSportsLoop.Play();
                }

                //if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                //  m_diversion.moveLeft();

                //if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                //  m_diversion.moveRight();


                //if STRG is pressed you look at the end of the Field
                if ((m_keyboardState.IsKeyDown(Keys.LeftControl) == true) || gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    m_cameraLookAt = m_iceFloorPos + m_cameraLookAtOffsetField;
                    m_cameraPosition = m_iceFloorPos + m_cameraPositionOffsetField;
                }
                //if Alt is pressed you look at the whole Field
                else if ((m_keyboardState.IsKeyDown(Keys.LeftAlt) == true) || gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    m_cameraLookAt = m_cameraLookAtAllField;
                    m_cameraPosition = m_cameraPositionAllField;
                }
                else if (m_idCurrentStone >= 0 || (m_keyboardState.IsKeyDown(Keys.LeftAlt) == false && m_keyboardState.IsKeyDown(Keys.LeftControl) == false && gamePadState.Buttons.RightShoulder != ButtonState.Pressed && gamePadState.Buttons.LeftShoulder != ButtonState.Pressed))
                {
                    //Camera follows current stone
                    m_cameraLookAt = m_stones[m_stoneIdCamera].getPosition();
                    //m_cameraPosition = m_stones[m_idCurrentStone].getPosition();
                    m_cameraPosition = m_stones[m_stoneIdCamera].getPosition() + m_cameraPositionOffset;
                }

                Vector3 up;
                Vector3 down;

                if (((m_keyboardState.IsKeyDown(Keys.Up) || (gamePadState.DPad.Up == ButtonState.Pressed)) && !somethingMoving) && m_stones[m_stoneIdCamera].getPosition().X < 23.0)
                {
                    up = new Vector3(0.5f, 0.0f, 0.0f);
                    m_stones[m_stoneIdCamera].setPosition(m_stones[m_stoneIdCamera].getPosition() + up);
                    m_arrowPos = m_arrowPos + up;
                }

                if (((m_keyboardState.IsKeyDown(Keys.Down) || (gamePadState.DPad.Down == ButtonState.Pressed)) && !somethingMoving) && m_stones[m_stoneIdCamera].getPosition().X > -23.0)
                {
                    down = new Vector3(-0.5f, 0.0f, 0.0f);
                    m_stones[m_stoneIdCamera].setPosition(m_stones[m_stoneIdCamera].getPosition() + down);
                    m_arrowPos = m_arrowPos + down;
                }

                Console.WriteLine("m_stonePos.X: " + m_stones[m_stoneIdCamera].getPosition().X);
                /* if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                 {
                     m_cameraLookAt = m_iceFloorPos + m_cameraLookAtOffsetField;
                     m_cameraPosition = m_iceFloorPos + m_cameraPositionOffsetField;
                 }
                 //if Alt is pressed you look at the whole Field
                 else if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                 {
                     m_cameraLookAt = m_cameraLookAtAllField;
                     m_cameraPosition = m_cameraPositionAllField;
                 }
                 else if (m_idCurrentStone >= 0 || (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed) == false && (gamePadState.Buttons.RightShoulder == ButtonState.Pressed == false))
                 {
                     //Camera follows current stone
                     m_cameraLookAt = m_stones[m_stoneIdCamera].getPosition();
                     //m_cameraPosition = m_stones[m_idCurrentStone].getPosition();
                     m_cameraPosition = m_stones[m_stoneIdCamera].getPosition() + m_cameraPositionOffset;
                 }
             */
                m_previousGameTime = new GameTime(gameTime.TotalRealTime, gameTime.ElapsedGameTime, gameTime.TotalGameTime, gameTime.ElapsedGameTime);
            }
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            DrawHall();
            DrawFloor();
            DrawArrow();
            DrawStone();
            //DrawHall();

            m_spriteBatch.Begin(SpriteBlendMode.AlphaBlend);//start drawing 2D IMages
            m_powerBar.draw(m_spriteBatch);
            m_spriteBatch.End();

            if (s == true)
            {
                vidTexture = vidplayer.GetTexture();
                m_spriteBatch.Begin();
                m_spriteBatch.Draw(vidTexture, vidRectangle, Color.White);
                m_spriteBatch.End();
            }

            if (c == true)
            {
                m_spriteBatch.Begin();
                m_spriteBatch.Draw(ControlStartscreen, new Vector2(0, 0), Color.White);
                m_spriteBatch.End();
            }

            DrawFonts(gameTime);
            if (v == true)
            {

                m_spriteBatch.Begin();
                m_spriteBatch.Draw(Connected, new Vector2(764, 52), Color.White);
                m_spriteBatch.End();
            }
            else
            {
                m_spriteBatch.Begin();
                m_spriteBatch.Draw(Disconnected, new Vector2(764, 52), Color.White);
                m_spriteBatch.End();
            }

            if (scfa == 1)
            {
                m_spriteBatch.Begin();
                m_spriteBatch.Draw(ScoreBeat, new Vector2(0, 0), Color.White);
                m_spriteBatch.End();
            }

            if (scfa == 2)
            {
                m_spriteBatch.Begin();
                m_spriteBatch.Draw(ScoreFail, new Vector2(0, 0), Color.White);
                m_spriteBatch.End();
            }


            base.Draw(gameTime);
        }

        private void DrawStone()
        {
            for (int i = 0; i < m_numberOfStones; i++)
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
                        * Matrix.CreateScale(m_iceFloorScale);
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

        private void DrawHall()
        {

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[m_hall.getModel().Bones.Count];
            m_hall.getModel().CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in m_hall.getModel().Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(3.0f, 3.0f, 3.0f); // a red light
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(m_hallRotation)
                        * Matrix.CreateTranslation(m_hallPos)
                        * Matrix.CreateScale(m_hallScale);
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

        private void DrawArrow()
        {

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[m_arrow.getModel().Bones.Count];
            m_arrow.getModel().CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in m_arrow.getModel().Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(3.0f, 3.0f, 3.0f);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(m_arrowRotation)
                        * Matrix.CreateTranslation(m_arrowPos)
                        * Matrix.CreateScale(m_arrowScale);
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

        private void DrawFonts(GameTime GameTime)
        {
            m_spriteBatch.Begin();
            string outputString = m_score.ToString();
            m_spriteBatch.DrawString(spriteFont, "Score Last Round:", new Vector2(10, 10), Color.Black);
            m_spriteBatch.DrawString(spriteFont, outputString, new Vector2(10, 30), Color.Black);
            string stones = m_stoneCounter.ToString();
            m_spriteBatch.DrawString(spriteFont, "Remaining Stones:", new Vector2(585, 570), Color.Black);
            m_spriteBatch.DrawString(spriteFont, stones, new Vector2(775, 570), Color.Black);
            m_spriteBatch.End();
        }
    }
}
