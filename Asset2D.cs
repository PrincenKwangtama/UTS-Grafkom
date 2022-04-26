
using System;
using System.Collections.Generic;
using System.IO;
using LearnOpenTK.Common;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Tes
{   static class Constants
    {
        public const string path = "../../../Shader/";
        //"C:/Users/Ryan Inka Chandra/source/repos/Tes/Tes/Shader/shader.vert", "C:/Users/Ryan Inka Chandra/source/repos/Tes/Tes/Shader/shader.frag"
    }

    internal class Asset2D
    {
        float[] _vertices =
               {
            
        };

        int _vertexBufferObject;
        int _elementBufferObject;
        int _vertexArrayObject;
        int index;
        int[] pascalNum;

        uint[] _indices =
        {
             //0,1,3, //segitiga peratama (atas)
             //1,2,3 //segitigaa kedua (bawah)
        };

        Shader _shader;

        public Asset2D(float [] vertices,uint [] indices)
        {
            _vertices = vertices;
            _indices = indices;
            index = 0;
        }

        public void Load(string shadervert,string shaderfrag)
        {
            //base.OnLoad();
            //ganti back warna


            GL.ClearColor(0.1f, 0.0f, 0.0f, 0.1f);

            //inisialisasi
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            /*
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            */

            GL.GetInteger(GetPName.MaxVertexAttribs, out int maxAtrCount);
            Console.WriteLine(maxAtrCount);

            if(_indices.Length != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            }

            //_shader = new Shader("C:/Users/shader.vert","C:/Users/shader.frag");
            _shader = new Shader(shadervert,shaderfrag);
            _shader.Use();
        }

        public void Render(int drawStatus) //0 = segitiga, 1= lingkaran/elips, 2=garis, 3= lengkung (brezier)
        {
            _shader.Use();
            GL.BindVertexArray(_vertexArrayObject);

            if (_indices.Length != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                GL.BindVertexArray(_vertexArrayObject);
                
                if(drawStatus == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                }
                else if(drawStatus == 1)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (_vertices.Length + 1) / 3);
                }
                else if(drawStatus == 2)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, index);
                }
                else if(drawStatus == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, (_vertices.Length + 1) / 3);
                }
            }

            //int vertexColorLoc = GL.GetUniformLocation(_shader.Handle, "ourColor");
            //GL.Uniform4(vertexColorLoc,0.5f, 0.0f, 0.0f,0.1f);        
        }

        public void CreateCircle(float center_x,float center_y,float radius)
        {
            _vertices = new float[1080];

            for (int i = 0; i < 360; i++)
            {
                double degInRand = i * Math.PI / 180;

                //x
                _vertices[i*3] = radius * (float)Math.Cos(degInRand) + center_x;

                //y
                _vertices[i*3 + 1] = radius * (float)Math.Sin(degInRand) + center_y;

                //z
                _vertices[i * 3 + 2] = 0;
            }
        }

        public void CreateElipse(float center_x, float center_y, float radius_x,float radius_y)
        {
            _vertices = new float[1080];

            for (int i = 0; i < 360; i++)
            {
                double degInRand = i * Math.PI / 180;

                //x
                _vertices[i * 3] = radius_x * (float)Math.Cos(degInRand) + center_x;

                //y
                _vertices[i * 3 + 1] = radius_y * (float)Math.Sin(degInRand) + center_y;

                //z
                _vertices[i * 3 + 2] = 0;
            }
        }

        public void UpdateMousePosition(float _x, float _y)
        {
            _vertices[index * 3] = _x;
            _vertices[index * 3 + 1] = _y;
            _vertices[index * 3 + 2] = 0;
            index++;

            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

       public List<int> getRowPascal(int rowIndex)
        {
            List<int> currow = new List<int>();
            //------
            currow.Add(1);
            if (rowIndex == 0)
            {
                return currow;
            }
            //-----
            List<int> prev = getRowPascal(rowIndex - 1);
            for (int i = 1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            currow.Add(1);
            return currow;
        }

        public List<float> createCureveBezier()
        {
            List<float> _verticesBezier = new List<float>();
            List<int> _pascal = getRowPascal(index - 1);
            pascalNum = _pascal.ToArray();

            for(float t=0;t<=1.0f;t+=0.01f)
            {
                Vector2 p = getP(index, t);
                _verticesBezier.Add(p.X);
                _verticesBezier.Add(p.Y);
                _verticesBezier.Add(0);
            }

            return _verticesBezier;
        }

        public Vector2 getP (int n,float t)
        {
            Vector2 v = new Vector2();
            float k;
            for (int i = 0;i<n;i++)
            {
                k = (float)Math.Pow((1 - t), n - 1 - i) * (float)Math.Pow(t, i)*pascalNum[i];
                v.X += k * _vertices[i * 3];
                v.Y += k * _vertices[i * 3 + 1];
            }
            return v;
        }

        public bool checkVerticelength()
        {
            if(_vertices[0]==0)
            {
                return false;
            }
            
            if((_vertices.Length + 1)/3 > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void setVertices(float [] _temp)
        {
            _vertices = _temp;
        }
    }
}
