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

        float rotationRate = .1f;
        double rotationAngle;
        float rotationTime = 0;
        bool rotateDirection = true; // "false" simply means spin the other way.

        public Wolverine(float x, float y)
        {

            transparent = true;

            P = new Vector2(x, y);
            
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

            v.X = -.1f;
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

            //Rotate Wolverine head
            if (rotateDirection)
                rotationAngle += rotationRate * (dt * Math.PI / 3 - Math.PI / 6);     // Between 30 and 60 degrees
            else
                rotationAngle -= rotationRate * (dt * Math.PI / 3 - Math.PI / 6);     // Between 30 and 60 degrees


            if (Math.Abs(rotationAngle) > Math.PI / 6)
            {
                rotationAngle = Math.PI / 6 * (rotateDirection ? -1 : 1);
                rotateDirection = !rotateDirection;
            }

            Matrix transform = new Matrix();
            transform.M11 = (float)Math.Cos(rotationAngle);
            transform.M21 = -(float)Math.Sin(rotationAngle);
            transform.M12 = (float)Math.Sin(rotationAngle);
            transform.M22 = (float)Math.Cos(rotationAngle);

            //float sin = (float)Math.Sin(rotationAngle);
            //float cos = (float)Math.Cos(rotationAngle);


            //Matrix transformbetter = new Matrix();
            //transform.M11 = (float)(cos + p.X*p.X * (1 - cos));
            //transform.M21 = (float)( p.X*p.Y *(1-cos));
            //transform.M31 = (float)(p.Y * sin);

            //transform.M12 = (float)(p.Y * p.X * (1 - cos));
            //transform.M22 = (float)(cos + p.Y * p.Y * (1 - cos));
            //transform.M32 = (float)(p.X * sin);

            //transform.M13 = (float)(-p.Y * sin);
            //transform.M23 = (float)(p.X * sin);
            //transform.M33 = cos;


            //Move Entire wolverine
            
            Vector2 newP = Vector2.Add(p, Vector2.Multiply(v, dt));

            // Move the vertices
            verticesMHead.Clear();
            foreach (Vector2 x in verticesHead)
            {
                x.Subtract(new Vector2(p.X /*+ .125f*/, p.Y));
                Vector4 vert = Vector2.Transform(x, transform);
                vert.Add(new Vector4(p.X /*+ .125f*/, p.Y, 0, 0));

                verticesMHead.Add(new Vector2(vert.X + p.X, vert.Y + p.Y));
            }
            p = newP;

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
