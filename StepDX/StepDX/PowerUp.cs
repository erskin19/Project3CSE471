using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace StepDX
{
    public class PowerUp : PolygonTextured
    {

        private Vector2 p = new Vector2(0, 0);  // Position
        private Vector2 v = new Vector2(0, 0);
        private Vector2 a = new Vector2(0, -9.81f);

        public Vector2 P { set { p = value; } get { return p; } }
        public Vector2 V { set { v = value; } get { return v; } }
        protected List<Vector2> verticesM = new List<Vector2>();  // The vertices

        public override List<Vector2> Vertices { get { return verticesM; } }

        private bool onBlock;
        public bool OnBlock { set { onBlock = value; } get { return onBlock; } }
        public override void Advance(float dt)
        {
            
            v.Add(Vector2.Multiply(a, dt));
            p.X += v.X * dt;
            p.Y += v.Y * dt;



            // Move the vertices
            verticesM.Clear();
            foreach (Vector2 x in verticesB)
            {
                verticesM.Add(new Vector2(x.X + p.X, x.Y + p.Y));
            }
        }
    }
}
