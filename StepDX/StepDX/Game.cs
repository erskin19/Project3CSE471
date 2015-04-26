using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private float playingW = 32;
        /// <summary>
        /// Vertex buffer for our drawing
        /// </summary>
        private VertexBuffer vertices = null;

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

        /// <summary>
        /// What the last time reading was
        /// </summary>
        private long lastTime;

        /// <summary>
        /// A stopwatch to use to keep track of time
        /// </summary>
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Polygon floor
        /// </summary>
       // Polygon floor = new Polygon();
        /// <summary>
        /// All of the polygons that make up our world
        /// </summary>
        List<Polygon> world = new List<Polygon>();
        /// <summary>
        /// Our player sprite
        /// </summary>
        GameSprite player = new GameSprite();
        /// <summary>
        /// The collision testing subsystem
        /// </summary>
        Collision collision = new Collision();
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
            background = new Background(device, playingW, playingH);

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
            PolygonTextured pt = new PolygonTextured();
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

            Texture spritetexture = TextureLoader.FromFile(device, "../../guy8.bmp");
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
            player.Render(device);
            //End the scene
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
               
                
               /* foreach (Polygon p in world)
                {
                    p.Advance(step);     
                }*/  //Some refactoring
                foreach (Polygon p in world)
                {
                    p.Advance(step);
                    if (collision.Test(player, p))
                    {
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
            else if (e.KeyCode == Keys.Space)
            {
                // Task 2: Allow jumping only if player is standing on something
                if (player.OnBlock)
                {
                        Vector2 v = player.V;
                        v.Y = 7;
                        player.V = v;
                        player.A = new Vector2(0, -9.8f);
               
                 }
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
    }
}
