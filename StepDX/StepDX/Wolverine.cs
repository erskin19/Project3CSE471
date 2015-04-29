using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace StepDX
{
    public class Wolverine : PolygonTextured
    {
        // Task 1: Limit the movement of the player with left and right bounds
        private float playerMinX = 0.4f;                    // Minimum x allowed
        private float playerMaxX = 31.6f;                   // Maximum x allowed

        private Vector2 p = new Vector2(0, 0);  // Position
        private Vector2 v = new Vector2(0, 0);  // Velocity

        public Vector2 P { set { p = value; } get { return p; } }
        public Vector2 V { set { v = value; } get { return v; } }

        private Vector2 pSave;  // Position
        private Vector2 vSave;  // Velocity

        //New stuff for texturing two objects.
        protected List<Vector2> textureCHead = new List<Vector2>();
        protected List<Vector2> textureCBody = new List<Vector2>();

        public override List<Vector2> Vertices { get { return verticesBody; } }

        float timeElapsed = 0;
        bool slideDirection = true; // "false" simply means spin the other way.

        public Wolverine(float x, float y)
        {

            transparent = true;

            P = new Vector2(x, y);
            
            //Quick and dirty hack to fix positioning problem
            y = y + .5f;

            //Head
            AddVertexHead(new Vector2(x, y));
            AddVertexHead(new Vector2(x - .0625f, y + .025f));
            AddVertexHead(new Vector2(x - .125f, y + 0));

            AddVertexHead(new Vector2(x - .1f, y - .1f));
            AddVertexHead(new Vector2(x - .095f, y - .105f));
            AddVertexHead(new Vector2(x + .01875f, y - .085f));

            AddVertexHead(new Vector2(x + .125f, y - .125f));
            AddVertexHead(new Vector2(x + .125f, y + .125f));
            AddVertexHead(new Vector2(x - .05f, y + .075f));
            AddVertexHead(new Vector2(x - .0625f, y + .025f));


            AddTexHead(new Vector2(.1044f, .5117f));
            AddTexHead(new Vector2(.0510f, .4414f));
            AddTexHead(new Vector2(0, .5f));

            AddTexHead(new Vector2(.0255f, .6250f));
            AddTexHead(new Vector2(.0394f, .6328f));
            AddTexHead(new Vector2(.0928f, .6061f));

            AddTexHead(new Vector2(.1555f, .6468f));
            AddTexHead(new Vector2(.1555f, .3281f));
            AddTexHead(new Vector2(.0603f, .3828f));
            AddTexHead(new Vector2(.0510f, .4414f));

            //Body
            AddVertexBody(new Vector2(x + .125f, y + .5f));
            AddVertexBody(new Vector2(x + .125f, y - .5f));
            AddVertexBody(new Vector2(x + 1, y - .5f));
            AddVertexBody(new Vector2(x + 1, y + .5f));

            AddTexBody(new Vector2(.1555f, 0));
            AddTexBody(new Vector2(.1555f, 1));
            AddTexBody(new Vector2(1, 1));
            AddTexBody(new Vector2(1, 0));

            v.X = -.3f;
        }
        public void SaveState()
        {
            pSave = p;
            vSave = v;
        }

        public void RestoreState()
        {
            p = pSave;
            v = vSave;
        }

        protected List<Vector2> verticesHead = new List<Vector2>();
        protected List<Vector2> verticesBody = new List<Vector2>();
        protected VertexBuffer verticesVHead = null;
        protected VertexBuffer verticesVBody = null;

        public void AddVertexHead(Vector2 vertex)
        {
            if (verticesVHead == null)
                verticesHead.Add(vertex);
        }

        public void AddVertexBody(Vector2 vertex)
        {
            if (verticesVBody == null)
                verticesBody.Add(vertex);
        }

        public void AddTexHead(Vector2 v)
        {
            textureCHead.Add(v);
        }

        public void AddTexBody(Vector2 v)
        {
            textureCBody.Add(v);
        }

        protected List<Vector2> verticesMHead = new List<Vector2>();  // The vertices
        protected List<Vector2> verticesMBody = new List<Vector2>();


        private float spriteTime = 0;
        private float spriteRate = 6;   // 6 per second

        public void changeDirection()
        {
            v.X = -v.X;
        }
        
        public override void Advance(float dt)
        {

            // Euler steps
            p.X += v.X * dt;
            p.Y += v.Y * dt;

            timeElapsed += dt;

            if(timeElapsed > 1){
                timeElapsed %= 1;
                slideDirection = !slideDirection;
            }

            Vector2 headMove;
            if (slideDirection)
                headMove = new Vector2(0, .1f * (timeElapsed) - .05f);
            else
                headMove = new Vector2(0, -.1f * (timeElapsed) + .05f);
            

            // Move the vertices
            verticesMHead.Clear();
            foreach (Vector2 x in verticesHead)
            {
                Vector2 loc = Vector2.Add(x, p);
                verticesMHead.Add(Vector2.Add(loc, headMove));
            }
            

            verticesMBody.Clear();
            foreach (Vector2 x in verticesBody)
            {
                verticesMBody.Add(new Vector2(x.X + p.X, x.Y + p.Y));
            }
        }

        /*
         * We need this because we draw a head and a body, which requires two vertex buffers to do.
        */
        public override void Render(Device device)
        {
            // Draw head first
            List<Vector2> vertices = verticesMHead;

            // Ensure we have at least a triangle
            if (vertices.Count >= 3)
            {

                // Ensure the number of vertices and textures are the same
                System.Diagnostics.Debug.Assert(textureCHead.Count == vertices.Count);

                if (verticesVHead == null)
                {
                    verticesVHead = new VertexBuffer(typeof(CustomVertex.PositionColoredTextured), // Type
                       vertices.Count,      // How many
                       device, // What device
                       0,      // No special usage
                       CustomVertex.PositionColoredTextured.Format,
                       Pool.Managed);
                }

                GraphicsStream gs = verticesVHead.Lock(0, 0, 0);     // Lock the background vertex list
                int clr = color.ToArgb();

                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector2 v = vertices[i];
                    Vector2 t = textureCHead[i];
                    gs.Write(new CustomVertex.PositionColoredTextured(v.X, v.Y, 0, clr, t.X, t.Y));
                }

                verticesVHead.Unlock();
                if (transparent)
                {
                    device.RenderState.AlphaBlendEnable = true;
                    device.RenderState.SourceBlend = Blend.SourceAlpha;
                    device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                }
                device.SetTexture(0, texture);
                device.SetStreamSource(0, verticesVHead, 0);
                device.VertexFormat = CustomVertex.PositionColoredTextured.Format;
                device.DrawPrimitives(PrimitiveType.TriangleFan, 0, vertices.Count - 2);
                device.SetTexture(0, null);
                if (transparent)
                    device.RenderState.AlphaBlendEnable = false;
            }

            //Now the body
            vertices = verticesMBody;

            // Ensure we have at least a triangle
            if (vertices.Count >= 3)
            {

                // Ensure the number of vertices and textures are the same
                System.Diagnostics.Debug.Assert(textureCBody.Count == vertices.Count);

                if (verticesVBody == null)
                {
                    verticesVBody = new VertexBuffer(typeof(CustomVertex.PositionColoredTextured), // Type
                       vertices.Count,      // How many
                       device, // What device
                       0,      // No special usage
                       CustomVertex.PositionColoredTextured.Format,
                       Pool.Managed);
                }

                GraphicsStream gs = verticesVBody.Lock(0, 0, 0);     // Lock the background vertex list
                int clr = color.ToArgb();

                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector2 v = vertices[i];
                    Vector2 t = textureCBody[i];
                    gs.Write(new CustomVertex.PositionColoredTextured(v.X, v.Y, 0, clr, t.X, t.Y));
                }

                verticesVBody.Unlock();
                if (transparent)
                {
                    device.RenderState.AlphaBlendEnable = true;
                    device.RenderState.SourceBlend = Blend.SourceAlpha;
                    device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                }
                device.SetTexture(0, texture);
                device.SetStreamSource(0, verticesVBody, 0);
                device.VertexFormat = CustomVertex.PositionColoredTextured.Format;
                device.DrawPrimitives(PrimitiveType.TriangleFan, 0, vertices.Count - 2);
                device.SetTexture(0, null);
                if (transparent)
                    device.RenderState.AlphaBlendEnable = false;
            }
        }
        
    }
}
