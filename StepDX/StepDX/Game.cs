﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace StepDX
{
    public partial class Game : Form
    {
        /// <summary>
        /// The DirectX device we will draw on
        /// </summary>
        private Device device = null;
        /// <summary>
        /// Height of our playing area (meters)
        /// </summary>
        private float playingH = 4;

        /// <summary>
        /// Width of our playing area (meters)
        /// </summary>
        private float playingW = 11;
        /// <summary>
        /// Vertex buffer for our drawing
        /// </summary>
        private VertexBuffer vertices = null;
        private VertexBuffer verticeshex = null;
        /// <summary>
        /// The background image class
        /// </summary>
        private Background background = null;

        /// <summary>
        /// Block Player variables
        /// </summary>
        /* private Vector2 playerLoc = new Vector2(0.4f, 1);   // Where our player is
        private Vector2 playerSpeed = new Vector2(0, 0);    // How fast we are moving
        float playerMinX = 0.4f;                    // Minimum x allowed
        float playerMaxX = 31.6f;                   // Maximum x allowed
       */

        private Random random = new Random();

        /// <summary>
        /// What the last time reading was
        /// </summary>
        private long lastTime;

        /// <summary>
        /// A stopwatch to use to keep track of time
        /// </summary>
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        private GameSounds sound;

        /// <summary>
        /// Polygon floor
        /// </summary>
       // Polygon floor = new Polygon();
        /// <summary>
        /// All of the polygons that make up our world
        /// </summary>
        List<Polygon> world = new List<Polygon>();
        List<Polygon> powerup = new List<Polygon>();
        Polygon powerup_to_remove = new Polygon();
        List<Polygon> coins = new List<Polygon>();
        List<Sword> swords = new List<Sword>();
        Sword sword_to_remove = new Sword();
        Polygon coin_to_remove = new Polygon();
        PowerUp basketball = new PowerUp();

        List<Wolverine> wolverines = new List<Wolverine>();
        Wolverine wolverine_to_remove;
        /// <summary>
        /// Our player sprite
        /// </summary>
        GameSprite player = new GameSprite();
        Wolverine wolverine;
        /// <summary>
        /// The collision testing subsystem
        /// </summary>
        Collision collision = new Collision();
        //Microsoft.DirectX.Direct3D.Font font;

        public bool game_over = false;

        Texture spritetexture;
        Texture spritepowertexture;
        PolygonTextured pt = new PolygonTextured();
        Sword sword_sprite = new Sword();
        PolygonTextured flag = new PolygonTextured();

        private int score = 0;
        

        public Game()
        {
            InitializeComponent();
            if (!InitializeDirect3D())
                return;
            // To create a triangle
          /*  vertices = new VertexBuffer(typeof(CustomVertex.PositionColored), // Type of vertex
                                        3,      // How many
                                        device, // What device
                                        0,      // No special usage
                                        CustomVertex.PositionColored.Format,
                                        Pool.Managed);*/

            // To create a rectangle
            vertices = new VertexBuffer(typeof(CustomVertex.PositionColored), // Type of vertex
                                       4,      // How many
                                       device, // What device
                                       0,      // No special usage
                                       CustomVertex.PositionColored.Format,
                                       Pool.Managed);
            verticeshex = new VertexBuffer(typeof(CustomVertex.PositionColored), // Type of vertex
                                       6,      // How many
                                       device, // What device
                                       0,      // No special usage
                                       CustomVertex.PositionColored.Format,
                                       Pool.Managed);

            background = new Background(device, playingW, playingH);

            //font = new Microsoft.DirectX.Direct3D.Font(device, new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 20f));

            sound = new GameSounds(this);

            // Add a polygon
            /* floor.AddVertex(new Vector2(0, 1));
            floor.AddVertex(new Vector2(playingW, 1));
            floor.AddVertex(new Vector2(playingW, 0.9f));
            floor.AddVertex(new Vector2(0, 0.9f));
            floor.Color = Color.CornflowerBlue;*/

            // Add multiple polygons to form the world
            Polygon floor = new Polygon();
            floor.AddVertex(new Vector2(0, 1));
            floor.AddVertex(new Vector2(playingW, 1));
            floor.AddVertex(new Vector2(playingW, 0.9f));
            floor.AddVertex(new Vector2(0, 0.9f));
            floor.Color = Color.CornflowerBlue;
            world.Add(floor);
            for (int i = 0; i < 10; i++)
            {
                Texture coin_texture = TextureLoader.FromFile(device, "../../coin.png");
                PolygonTextured hexagon = new PolygonTextured();
                hexagon.Transparent = true;
                hexagon.Tex = coin_texture;
                
                float x = (float)random.Next(2,10) ;
                float y = (float)random.Next(1,6);
                
                hexagon.AddVertex(new Vector2(x + 0.5f / 6, y));
                hexagon.AddTex(new Vector2(0.75f, 1f));
                hexagon.AddVertex(new Vector2(x, y + 1.732f / 12));
                hexagon.AddTex(new Vector2(1f, 0.5f));
                hexagon.AddVertex(new Vector2(x + 0.5f / 6, y + 1.732f / 6));
                hexagon.AddTex(new Vector2(0.75f, 0f));
                hexagon.AddVertex(new Vector2(x + 1.5f / 6, y + 1.732f / 6));
                hexagon.AddTex(new Vector2(0.25f, 0f));
                hexagon.AddVertex(new Vector2(x + 2f / 6, y + 1.732f / 12));
                hexagon.AddTex(new Vector2(0f, 0.5f));
                hexagon.AddVertex(new Vector2(x + 1.5f / 6, y));
                hexagon.AddTex(new Vector2(0.25f, 1f));
                hexagon.Color = Color.Transparent;
                coins.Add(hexagon);
            }

            Texture flag_texture = TextureLoader.FromFile(device, "../../../textures/flag.png");
            flag.AddVertex(new Vector2(9.7f, 2));
            flag.AddVertex(new Vector2(10.7f, 2));
            flag.AddVertex(new Vector2(10.7f, 1));
            flag.Tex = flag_texture;
            flag.AddTex(new Vector2(0, 0));
            flag.AddTex(new Vector2(1, 0));
            flag.AddTex(new Vector2(1, 1));
            //world.Add(flag);
            //basketball
            Texture basketballtexture = TextureLoader.FromFile(device, "../../basketball.png");
            basketball.Transparent = true;
            basketball.P = new Vector2(2, 2);
            //basketball.V = new Vector2(0.1f,-0.1f);
            basketball.Tex = basketballtexture;

            basketball.AddVertex(new Vector2(-0.5f/3, 1.732f/6));
            basketball.AddTex(new Vector2(0, 1));
            basketball.AddVertex(new Vector2(0.5f/3, 1.732f/6));
            basketball.AddTex(new Vector2(0, 0));
            basketball.AddVertex(new Vector2(0.5f/3, 0));
            basketball.AddTex(new Vector2(1, 0));
            basketball.AddVertex(new Vector2(-0.5f/3, 0));
            basketball.AddTex(new Vector2(1, 1));
            basketball.Color = Color.Transparent;
            //powerup.Add(basketball);
            //Vector2 v_b = basketball.V;
            //v_b.Y = -0.5f;
            //basketball.V = v_b;

            AddObstacle(2, 3, 1.7f, 1.9f, Color.Crimson);
            AddObstacle(4, 4.2f, 1, 2.1f, Color.Coral);
            AddObstacle(5, 6, 2.2f, 2.4f, Color.BurlyWood);
            AddObstacle(5.5f, 6.5f, 3.2f, 3.4f, Color.PeachPuff);
            AddObstacle(6.5f, 7.5f, 2.5f, 2.7f, Color.Chocolate);
            AddObstacle(8.5f, 9.5f, 1.0f, 1.2f, Color.AliceBlue);

            Platform platform = new Platform();
            platform.AddVertex(new Vector2(3.2f, 2));
            platform.AddVertex(new Vector2(3.9f, 2));
            platform.AddVertex(new Vector2(3.9f, 1.8f));
            platform.AddVertex(new Vector2(3.2f, 1.8f));
            platform.Color = Color.Black;
            world.Add(platform);

            Texture texture = TextureLoader.FromFile(device, "../../stone08.bmp");
            //PolygonTextured pt = new PolygonTextured();
            pt.Tex = texture;
            pt.AddVertex(new Vector2(1.2f, 3.5f));
            pt.AddTex(new Vector2(0, 1));
            pt.AddVertex(new Vector2(1.9f, 3.5f));
            pt.AddTex(new Vector2(0, 0));
            pt.AddVertex(new Vector2(1.9f, 3.3f));
            pt.AddTex(new Vector2(1, 0));
            pt.AddVertex(new Vector2(1.2f, 3.3f));
            pt.AddTex(new Vector2(1, 1));
            pt.Color = Color.Transparent;
            world.Add(pt);

            //Texture spritetexture = TextureLoader.FromFile(device, "../../guy8.bmp");
            spritetexture = TextureLoader.FromFile(device, "../../../textures/small_mario.png");
            spritepowertexture = TextureLoader.FromFile(device, "../../../textures/big_mario.png");
            player.Transparent = true;
            player.P = new Vector2(0.5f, 1);
            player.Tex = spritetexture;
            
            player.AddVertex(new Vector2(-0.2f, 0));
            player.AddTex(new Vector2(0, 1));
            player.AddVertex(new Vector2(-0.2f, 1));
            player.AddTex(new Vector2(0, 0));
            player.AddVertex(new Vector2(0.2f, 1));
            player.AddTex(new Vector2(0.125f, 0));
            player.AddVertex(new Vector2(0.2f, 0));
            player.AddTex(new Vector2(0.125f, 1));
            player.Color = Color.Transparent;

            /*
            Texture wolverineTex = TextureLoader.FromFile(device, "../../../textures/wolverine.bmp");
            wolverine = new Wolverine(2, 2);
            wolverine.Tex = wolverineTex;
            */
            Texture wolvSprite = TextureLoader.FromFile(device, "../../../textures/wolverine.png");

            for (int i = 0; i < 1; i++) 
            {
                //wolverine = new Wolverine((float)random.Next(3, 5) + (float)random.NextDouble(),  .5f);
                wolverine = new Wolverine(3, .5f);
                
                //wolverine.Transparent = true;
                //wolverine.P = new Vector2(0.5f, 1);
                wolverine.Tex = wolvSprite;
                wolverines.Add(wolverine);
            }
                
            /*
            wolverine.AddVertex(new Vector2(-0.2f, 0));
            wolverine.AddTex(new Vector2(0, 1));
            wolverine.AddVertex(new Vector2(-0.2f, 1));
            wolverine.AddTex(new Vector2(0, 0));
            wolverine.AddVertex(new Vector2(0.2f, 1));
            wolverine.AddTex(new Vector2(0.125f, 0));
            wolverine.AddVertex(new Vector2(0.2f, 0));
            wolverine.AddTex(new Vector2(0.125f, 1));
            wolverine.Color = Color.Transparent;
            */

            /*
            Texture swordtexture = TextureLoader.FromFile(device, "../../sword_alpha.png");
            sword_sprite.Transparent = true;
            //sword_sprite.P = new Vector2(2,2);
            sword_sprite.Tex = swordtexture;
            sword_sprite.AddVertex(new Vector2(1, 0));
            sword_sprite.AddTex(new Vector2(0,1));
            sword_sprite.AddVertex(new Vector2(2, 0.5f));
            sword_sprite.AddTex(new Vector2(1,0.5f));
            sword_sprite.AddVertex(new Vector2(1, 1));
            sword_sprite.AddTex(new Vector2(0,0));
            sword_sprite.Color = Color.Transparent;
            */
            // Determine the last time
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;
        }
        /// <summary>
        /// Initialize the Direct3D device for rendering
        /// </summary>
        /// <returns>true if successful</returns>
        private bool InitializeDirect3D()
        {
            try
            {
                // Now let's setup our D3D stuff
                PresentParameters presentParams = new PresentParameters();
                presentParams.Windowed = true;
                presentParams.SwapEffect = SwapEffect.Discard;

                device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            }
            catch (DirectXException)
            {
                return false;
            }

            return true;
        }

        public void Render()
        {
            if (device == null)
                return;

            device.Clear(ClearFlags.Target, System.Drawing.Color.Blue, 1.0f, 0);
            int wid = Width;                            // Width of our display window
            int hit = Height;                           // Height of our display window.
            float aspect = (float)wid / (float)hit;     // What is the aspect ratio?

            device.RenderState.ZBufferEnable = false;   // We'll not use this feature
            device.RenderState.Lighting = false;        // Or this one...
            device.RenderState.CullMode = Cull.None;    // Or this one...

            float widP = playingH * aspect;         // Total width of window
            // Static background
            //device.Transform.Projection = Matrix.OrthoOffCenterLH(0, widP, 0, playingH, 0, 1);
            
            // Background moves with player
            //float winCenter = playerLoc.X;
            float winCenter = player.P.X;
            if (winCenter - widP / 2 < 0)
                winCenter = widP / 2;
            else if (winCenter + widP / 2 > playingW)
                winCenter = playingW - widP / 2;

            device.Transform.Projection = Matrix.OrthoOffCenterLH(winCenter - widP / 2,
                                                                  winCenter + widP / 2,
                                                                  0, playingH, 0, 1);
            //Begin the scene
            device.BeginScene();
            // Render the background
            background.Render();
            // Render the floor polygon
            //floor.Render(device);
            // Render all the polygons in the world
            foreach (Polygon p in world)
            {
                p.Render(device);
            }


            if(coins.Count >= 1){
            foreach (Polygon p in coins)
            {
                p.Render(device);
            }
            }

            /*// Render the triangle (later a rectangle)
            GraphicsStream gs = vertices.Lock(0, 0, 0);     // Lock the vertex list
            int clr = Color.FromArgb(255, 0, 0).ToArgb();
            int vertex_clr = Color.FromArgb(0, 255, 0).ToArgb();*/
            // To draw a simple rectangle
         /* gs.Write(new CustomVertex.PositionColored(1, 1, 0, clr));
            gs.Write(new CustomVertex.PositionColored(1, 3, 0, clr));
            gs.Write(new CustomVertex.PositionColored(3, 3, 0, clr));
            // Change the color of one vertex
            //gs.Write(new CustomVertex.PositionColored(3, 3, 0, vertex_clr));
            // To draw rectangle
            gs.Write(new CustomVertex.PositionColored(3, 1, 0, clr));
            */
            // To draw the block player
            /*gs.Write(new CustomVertex.PositionColored(playerLoc.X - 0.1f, playerLoc.Y, 0, clr));
            gs.Write(new CustomVertex.PositionColored(playerLoc.X - 0.1f, playerLoc.Y + 0.5f, 0, clr));
            gs.Write(new CustomVertex.PositionColored(playerLoc.X + 0.1f, playerLoc.Y + 0.5f, 0, clr));
            gs.Write(new CustomVertex.PositionColored(playerLoc.X + 0.1f, playerLoc.Y, 0, clr));
            vertices.Unlock();

            device.SetStreamSource(0, vertices, 0);
            device.VertexFormat = CustomVertex.PositionColored.Format;
            // To draw triangle
            // device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            // To draw rectangle
            device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);*/
           
            foreach (PolygonTextured p in powerup)
            {
                p.Render(device);
            }
            foreach(PolygonTextured p in swords)
            {
                p.Render(device);
            }
            //sword_sprite.Render(device);
            foreach (Wolverine w in wolverines)
            {
                w.Render(device);
            }
            flag.Render(device);
            player.Render(device);
            //End the scene

            Sprite sp = new Sprite(device);
            Microsoft.DirectX.Direct3D.Font font = new Microsoft.DirectX.Direct3D.Font(device, new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 20f));
            font.DrawText(null, "score: " + score, new Point(3, 3), Color.Black);

            if(game_over == true){
                //Microsoft.DirectX.Direct3D.Font font2 = new Microsoft.DirectX.Direct3D.Font(device, new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 40f));
                font.DrawText(null, "You Win! Score:" + score, new Point(200, 200), Color.Black);
                font.Dispose();
            }
            font.Dispose();
            
            device.EndScene();
            device.Present();
        }

        private void AddObstacle(float p1, float p2, float p3, float p4, Color color)
        {
            Polygon p = new Polygon();
            p.AddVertex(new Vector2(p1, p4));
            p.AddVertex(new Vector2(p2, p4));
            p.AddVertex(new Vector2(p2, p3));
            p.AddVertex(new Vector2(p1, p3));
            p.Color = color;
           world.Add(p);
        }

        /// <summary>
        /// Advance the game in time
        /// </summary>
        public void Advance()
        {
            /*// How much time change has there been?
            long time = stopwatch.ElapsedMilliseconds;
            float delta = (time - lastTime) * 0.001f;       // Delta time in milliseconds
            lastTime = time;
            foreach (Polygon p in world)
                p.Advance(delta);
            player.Advance(delta);
            playerSpeed.Y = playerSpeed.Y * 9.8f;
            playerLoc += playerSpeed * delta;
            if (playerLoc.X < playerMinX)
                playerLoc.X = playerMinX;
            else if (playerLoc.X > playerMaxX)
                playerLoc.X = playerMaxX;
            playerSpeed.Y = 0;
            playerLoc += playerSpeed * delta;
            if (playerLoc.X < playerMinX)
                playerLoc.X = playerMinX;
            else if (playerLoc.X > playerMaxX)
                playerLoc.X = playerMaxX;*/

            // How much time change has there been?
            long time = stopwatch.ElapsedMilliseconds;
            long power_start_time = 0;
            float delta = (time - lastTime) * 0.001f;       // Delta time in milliseconds
            lastTime = time;
            
            while (delta > 0)
            {

                float step = delta;
                if (step > 0.05f)
                    step = 0.05f;

                float maxspeed = Math.Max(Math.Abs(player.V.X), Math.Abs(player.V.Y));
                if (maxspeed > 0)
                {
                    step = (float)Math.Min(step, 0.05 / maxspeed);
                }
               
                player.Advance(step);
                basketball.Advance(step);
                foreach(Wolverine wolverine in wolverines)
                {
                    wolverine.Advance(step);
                }
                foreach(Sword p in swords)
                {
                    p.Advance(step);
                    if (p.P.X > 15)
                    {
                        sword_to_remove = p;
                    }
                }
                swords.Remove(sword_to_remove);
                //sword_sprite.Advance(step);
               /* foreach (Polygon p in world)
                {
                    p.Advance(step);     
                }*/  //Some refactoring
                foreach (Polygon p in world)
                {
                    p.Advance(step);
                    if (collision.Test(player, p))
                    {
                        //powerup.Add(basketball);
                        float depth = collision.P1inP2 ?
                                  collision.Depth : -collision.Depth;
                        player.P = player.P + collision.N * depth;
                        Vector2 v = player.V;
                        if (collision.N.X != 0)
                            v.X = 0;
                        if (collision.N.Y != 0)
                            v.Y = 0;
                            
                        player.V = v;
                        player.Advance(0);
                
                    }
                    else if (player.P.X > 10)
                    {
                        //Restart();
                        //Microsoft.DirectX.Direct3D.Font font = new Microsoft.DirectX.Direct3D.Font(device, new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 40f));
                        //font.DrawText(null, "You Win!" + score, new Point(3, 3), Color.Black);
                        game_over = true;
                        //while (true) { ; }

                    }
                    if (collision.Test(basketball, p))
                    {
                        //powerup.Add(basketball);
                        float depth = collision.P1inP2 ?
                                  collision.Depth : -collision.Depth;
                        basketball.P = basketball.P + collision.N * depth;
                        Vector2 v2 = basketball.V;
                        if (collision.N.X != 0)
                            v2.X = -v2.X;
                        if (collision.N.Y != 0)
                            v2.Y = -0.5f;

                        basketball.V = v2;
                        basketball.Advance(0);

                    }
                    foreach (Wolverine wolverine in wolverines)
                    {
                        if (collision.Test(wolverine, p))
                        {
                            //powerup.Add(basketball);
                            float depth = collision.P1inP2 ?
                                      collision.Depth : -collision.Depth;
                            wolverine.P = wolverine.P + collision.N * depth;
                            Vector2 v3 = wolverine.V;
                            Vector2 v4 = wolverine.P;
                            if (collision.N.X != 0)
                                v4.X = v4.X + 1;
                            v3.X = -v3.X;
                            if (collision.N.Y != 0)
                                v3.Y = 0;

                            wolverine.P = v4;
                            wolverine.V = v3;
                            wolverine.Advance(0);

                        }
                    }
                }

                if (collision.Test(pt, player))
                {
                    Vector2 v = basketball.V;
                    v.X = .5f;
                    //v.Y = -0.5f;

                    if (powerup.Count < 1)
                    {
                        basketball.P = new Vector2(1.5f, 3.7f);
                        basketball.V = v;
                        powerup.Add(basketball);
                    }
                }

                foreach (Polygon p in powerup)
                {
                    if (collision.Test(p, player))
                    {
                        //powerup.Remove(basketball);
                        powerup_to_remove = p;
                        player.Tex = spritepowertexture;
                        power_start_time = time;
                    }
                }
                powerup.Remove(powerup_to_remove);

                if (power_start_time != 0)
                {
                    if ((time - power_start_time) > 5)
                    {
                        player.Tex = spritetexture;
                    }
                }

                foreach (Polygon p in coins)
                {
                    //p.Advance(step);
                    if (collision.Test(player, p))
                    {
                        coin_to_remove = p;
                        sound.Coin();
                        score += 100;
                        //coins.Remove(p);
                    }
                }

                coins.Remove(coin_to_remove);

                foreach(Sword s in swords)
                {
                    foreach(Wolverine p in wolverines)
                    {
                        
                        /*if (collision.Test(s, p))
                        {
                            wolverine_to_remove = p;
                            sword_to_remove = s;
                            sound.Death();
                        }*/


                        if (Math.Abs(s.P.X - .7f - p.P.X) < .01 && Math.Abs(s.P.Y - p.P.Y) < 1)
                        {
                            sword_to_remove = s;
                            wolverine_to_remove = p;
                            score += 250;
                            sound.Death();
                        }
                    }
                }

                wolverines.Remove(wolverine_to_remove);
                swords.Remove(sword_to_remove);

                foreach (Wolverine w in wolverines)
                {

                
                    if (Math.Abs(player.P.X - w.P.X - 3.5) < .6 && Math.Abs(player.P.Y - w.P.Y) < 1)
                    {
                        Restart();
                    }
                }
                

                delta -= step;
            }

        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
           /* if (e.KeyCode == Keys.Escape)
                this.Close(); // Esc was pressed
            else if (e.KeyCode == Keys.Right)
                playerSpeed.X = 5;
            else if (e.KeyCode == Keys.Left)
                playerSpeed.X = -5;
            else if (e.KeyCode == Keys.Space)
                playerSpeed.Y = 7;*/
            if (e.KeyCode == Keys.Escape)
                this.Close(); // Esc was pressed
            else if (e.KeyCode == Keys.Right)
            {
                Vector2 v = player.V;
                v.X = 1.5f;
                player.V = v;
            }
            else if (e.KeyCode == Keys.Left)
            {
                Vector2 v = player.V;
                v.X = -1.5f;
                player.V = v;
            }
            else if (e.KeyCode == Keys.Up)
            {
                // Task 2: Allow jumping only if player is standing on something
                if (player.OnBlock)
                {
                        Vector2 v = player.V;
                        v.Y = 5;
                        player.V = v;
                        player.A = new Vector2(0, -9.8f);
                        sound.Jump();
               
                 }
            }
            else if (e.KeyCode == Keys.Q)
            {
                Vector2 v = basketball.V;
                v.X = 1.5f;
                v.Y = -0.5f; 
                basketball.V = v;
            }
            else if (e.KeyCode == Keys.R)
            {
                Restart();
            }
            else if(e.KeyCode == Keys.Space)
            {
                //Vector2 vs = new Vector2();
                //vs.X = player.P.X - 1;
                //vs.Y = player.P.Y;
                //sword_sprite.P = vs;

                swords.Add(MakeSword());
            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            /*if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
                playerSpeed.X = 0;
            */
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
            {
                Vector2 v = player.V;
                v.X = 0;
                player.V = v;
            }
        }

        public void Restart()
        {
            player.P = new Vector2(0.5f, 1);
        }

        Sword MakeSword()
        {
            Sword s = new Sword();
            Texture swordtexture = TextureLoader.FromFile(device, "../../sword_alpha.png");
            s.Transparent = true;
            //sword_sprite.P = new Vector2(2,2);
            s.Tex = swordtexture;
            s.AddVertex(new Vector2(1, 0));
            s.AddTex(new Vector2(0, 1));
            s.AddVertex(new Vector2(2, 0.5f));
            s.AddTex(new Vector2(1, 0.5f));
            s.AddVertex(new Vector2(1, 1));
            s.AddTex(new Vector2(0, 0));
            s.Color = Color.Transparent;
            Vector2 vs = new Vector2();
            vs.X = player.P.X - 1;
            vs.Y = player.P.Y;
            Vector2 vvs = new Vector2();
            vvs.X = 1.5f;
            vvs.Y = 0;
            s.V = vvs;
            s.P = vs;
            return s;
        }
    }
}
