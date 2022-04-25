using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using LearnOpenTK.Common;
using OpenTK.Windowing.Common;

using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing.Imaging;

namespace Test
{
    internal class Assets_3D
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<uint> _indices = new List<uint>();

        float[] _color = new float[4];

        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;
        int index;
        int[] _pascal;
        int indexGaris = 2;
        Matrix4 _view;
        Matrix4 _projection;
        Matrix4 _model;
        String objType = "Not Assigned";

        public Vector3 _centerPosition = new Vector3(0, 0, 0);
        public List<Vector3> _euler = new List<Vector3>();
        public List<Assets_3D> Child;
        public List<Matrix4> childModel;
        Matrix4 model;

        float _x;
        float _y;
        float _z;

        public Assets_3D(List<Vector3> vertices, List<uint> indices, float[] color)
        {
            _vertices = vertices;
            _indices = indices;
            _color = color;
            Initialize();
        }

        public Assets_3D()
        {
            _color[0] = 1f;
            _color[1] = 1f;
            _color[2] = 1f;
            _color[3] = 1f;

            _vertices = new List<Vector3>();
            Initialize();
        }

        public Assets_3D(float r, float g, float b, float alpha)
        {
            _color[0] = r;
            _color[1] = g;
            _color[2] = b;
            _color[3] = alpha;

            _vertices = new List<Vector3>();
            Initialize();
        }

        public List<Assets_3D> GetChild()
        {
            return Child;
        }

        public void Initialize()
        {
            _euler = new List<Vector3>();
            //sumbu X
            _euler.Add(new Vector3(1, 0, 0));
            //sumbu y
            _euler.Add(new Vector3(0, 1, 0));
            //sumbu z
            _euler.Add(new Vector3(0, 0, 1));
            _model = Matrix4.Identity;
            _centerPosition = new Vector3(0, 0, 0);
            Child = new List<Assets_3D>();
            childModel = new List<Matrix4>();
            model = Matrix4.Identity;
        }

        public Vector3 ChangeVector(float x, float y, float z)
        {
            _centerPosition = new Vector3(x, y, z);
            return _centerPosition;
        }

        public Matrix4 getModel()
        {
            return model;
        }

        private void setX(float x)
        {
            _x = x;
        }

        private void setY(float y)
        {
            _y = y;
        }

        private void setZ(float z)
        {
            _z = z;
        }

        public float getX()
        {
            return _x;
        }

        public float getY()
        {
            return _y;
        }

        public float getZ()
        {
            return _z;
        }

        public void load(string shadervert, string shaderfrag, float Size_x, float Size_y)
        {
            //inisialisasi
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //Untuk segitiga berwarna
            /*_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);*/

            if (_indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * Vector3.SizeInBytes, _indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            _shader = new Shader(shadervert, shaderfrag);
            /*_shader = new Shader("C:/GrafKom/Pertemuan1/Pertemuan1/Shader/shader.vert",
                "C:/GrafKom/Pertemuan1/Pertemuan1/Shader/shader.frag");*/
            _shader.Use();

            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size_x / (float)Size_y, 0.1f, 100.0f);

            foreach (var item in Child)
            {
                item.load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }

        public void render(int pilihan,Matrix4 temp, double time, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();

            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "unicolor");
            GL.Uniform4(vertexColorLocation, _color[0], _color[1], _color[2], _color[3]);

            GL.BindVertexArray(_vertexArrayObject);

            _model = temp;

            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);

            //Matrix4 model = Matrix4.Identity;
            //model += model * Matrix4.CreateTranslation(0.0f, 0.3f, 0.0f);
            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {

                if (pilihan == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                }
                else if (pilihan == 1)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
                else if (pilihan == 2)
                {

                }
                else if (pilihan == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
            }
            foreach (var item in Child)
            {
                item.render(pilihan,  temp, time, camera_view, camera_projection);
            }


        }


        public void render(double time, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();

            //Ambil uniform dari shader untuk warna
            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "unicolor");
            GL.Uniform4(vertexColorLocation, _color[0], _color[1], _color[2], _color[3]);

            GL.BindVertexArray(_vertexArrayObject);

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);

            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if (objType == "Solid Circular")
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (int)(_vertices.Count));
                }
                else if (objType == "Hollow Circular")
                {
                    GL.DrawArrays(PrimitiveType.TriangleStrip, 1, (int)(_vertices.Count - 1));
                }
                else
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (int)(_vertices.Count));
                }
            }

            foreach (var item in Child)
            {
                item.render(time, camera_view, camera_projection);
            }
        }

        public void Scale(float scale, Assets_3D temp)
        {
            model = model * Matrix4.CreateTranslation(-temp._centerPosition);

            model = model * Matrix4.CreateScale(scale);

            model = model * Matrix4.CreateTranslation(temp._centerPosition);

            foreach (var i in Child)
            {
                i.Scale(scale, temp);
            }
        }

        //poros putaran di pusat objek
        //objIndex -> obj yg mana? (index nya di list)
        //axis -> diputar di sb apa?
        //0 = sb x, 1 = sb y, dll = sb z
        //angle -> brp derajat?
        public void RotateOnCore(int axis, float angle)
        {
            RotateOnZero(0, -30);
            RotateOnZero(1, -45);

            model = model * Matrix4.CreateTranslation(-_x, -_y, -_z);

            RotateOnZero(axis, angle);

            model = model * Matrix4.CreateTranslation(_x, _y, _z);

            RotateOnZero(1, 45);
            RotateOnZero(0, 30);

            foreach (var item in Child)
            {
                item.RotateOnCore(axis, angle);
            }
        }

        public void RotateOnCore(int axis, float angle, int childIndex)
        {
            RotateOnZero(0, -30);
            RotateOnZero(1, -45);

            model = model * Matrix4.CreateTranslation(-_x, -_y, -_z);

            RotateOnZero(axis, angle);

            model = model * Matrix4.CreateTranslation(_x, _y, _z);

            RotateOnZero(1, 45);
            RotateOnZero(0, 30);

            Child[childIndex].RotateOnCore(axis, angle);
        }

        //poros putaran di 0 0 0
        //objIndex -> obj yg mana? (index nya di list)
        //axis -> diputar di sb apa?
        //0 = sb x, 1 = sb y, dll = sb z
        //angle -> brp derajat?
        public void RotateOnZero(int axis, float angle)
        {
            if (axis == 0)
            {
                model = model * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angle));
            }
            else if (axis == 1)
            {
                model = model * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angle));
            }
            else
            {
                model = model * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(angle));
            }
        }

        public void Translate(float x, float y, float z)
        {
            model = model * Matrix4.CreateTranslation(x, y, z);

            _centerPosition.X += x;
            _centerPosition.Y += y;
            _centerPosition.Z += z;

            foreach (var item in Child)
            {
                item.Translate(x, y, z);
            }
        }

        public void Translate(float x, float y, float z, int childIdx)
        {
            model = model * Matrix4.CreateTranslation(x, y, z);

            _centerPosition.X += x;
            _centerPosition.Y += y;
            _centerPosition.Z += z;

            Child[childIdx].Translate(x, y, z);
        }

        public void TranslateOnGrid(float x, float y, float z)
        {
            RotateOnZero(0, -30);
            RotateOnZero(1, -45);

            model = model * Matrix4.CreateTranslation(x, y, z);

            _centerPosition.X += x;
            _centerPosition.Y += y;
            _centerPosition.Z += z;

            RotateOnZero(1, 45);
            RotateOnZero(0, 30);

            foreach (var item in Child)
            {
                item.TranslateOnGrid(x, y, z);
            }
        }

        public void Scale(float scale)
        {
            RotateOnZero(0, -30);
            RotateOnZero(1, -45);

            model = model * Matrix4.CreateTranslation(-_x, -_y, -_z);

            model = model * Matrix4.CreateScale(scale);

            model = model * Matrix4.CreateTranslation(_x, _y, _z);

            RotateOnZero(1, 45);
            RotateOnZero(0, 30);

            foreach (var item in Child)
            {
                item.Scale(scale);
            }
        }

        public void Scale(float xScale, float yScale, float zScale)
        {
            RotateOnZero(0, -30);
            RotateOnZero(1, -45);

            model = model * Matrix4.CreateTranslation(-_x, -_y, -_z);

            model = model * Matrix4.CreateScale(zScale, yScale, zScale);

            model = model * Matrix4.CreateTranslation(_x, _y, _z);

            RotateOnZero(1, 45);
            RotateOnZero(0, 30);

            foreach (var item in Child)
            {
                item.Scale(xScale, yScale, zScale);
            }
        }

        public void createCurveBezier()
        {
            _vertices.Add(new Vector3(0, 0, 0));
            _vertices.Add(new Vector3(1, 0, 0));
            _vertices.Add(new Vector3(0, 1, 0));

            List<Vector3> _verticesBezier = new List<Vector3>();
            List<int> pascal = new List<int>();
            if (_vertices.Count > 1)
            {
                pascal = getRow(_vertices.Count);
                for (float t = 0; t <= 1.0f; t += 0.005f)
                {
                    Vector3 p = getP(pascal, t);
                    _verticesBezier.Add(p);
                }
            }
            _vertices = _verticesBezier;
        }
        public Vector3 getP(List<int> pascal, float t)
        {
            Vector3 p = new Vector3(0, 0, 0);
            for (int i = 0; i < _vertices.Count; i++)
            {
                float temp = (float)Math.Pow((1 - t), _vertices.Count - 1 - i) * (float)Math.Pow(t, i) * pascal[i];
                p += temp * _vertices[i];
            }
            return p;
        }
        public List<int> getRow(int rowIndex)
        {
            List<int> currow = new List<int>();
            currow.Add(1);
            if (rowIndex == 0)
            {
                return currow;
            }

            List<int> prev = getRow(rowIndex - 1);
            for (int i = 1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            currow.Add(1);
            return currow;
        }

        public void createTrapezoid(float x, float y, float z, float length, float wide, float depth, float slope)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Solid";

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //1 2 5 6 -> Tutup (Atas)
            //3 4 7 8 -> Alas (Bawah)


            //TITIK 1 
            temp_vector.X = x - (slope + length) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + (slope + length) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createPyramidBox(float x, float y, float z, float length)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Solid";

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createPararelogram(float x, float y, float z, float length, float wide, float depth, float slope)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Solid";

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + (length - slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + (length - slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createBoxVertices(float x, float y, float z, float xLength, float yLength, float zLength)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Solid";

            //biar lebih fleksibel jangan inisialiasi posisi dan 
            //panjang kotak didalam tapi ditaruh ke parameter
            float _positionX = x;
            float _positionY = y;
            float _positionZ = z;

            //Buat temporary vector
            Vector3 temp_vector;
            //1. Inisialisasi vertex
            // Titik 1
            temp_vector.X = _positionX - xLength / 2.0f; // x 
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 2
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);
            // Titik 3
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z
            _vertices.Add(temp_vector);

            // Titik 4
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 5
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 6
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 7
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 8
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);
            //2. Inisialisasi index vertex
            _indices = new List<uint> {
                // Segitiga Depan 1
                0, 1, 2,
                // Segitiga Depan 2
                1, 2, 3,
                // Segitiga Atas 1
                0, 4, 5,
                // Segitiga Atas 2
                0, 1, 5,
                // Segitiga Kanan 1
                1, 3, 5,
                // Segitiga Kanan 2
                3, 5, 7,
                // Segitiga Kiri 1
                0, 2, 4,
                // Segitiga Kiri 2
                2, 4, 6,
                // Segitiga Belakang 1
                4, 5, 6,
                // Segitiga Belakang 2
                5, 6, 7,
                // Segitiga Bawah 1
                2, 3, 6,
                // Segitiga Bawah 2
                3, 6, 7
            };
        }

        public void createEllipsoid(float radiusX, float radiusY, float radiusZ, float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Solid Circular";

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 300)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createEllipsoid2(float radiusX, float radiusY, float radiusZ, float x, float y, float z, int sectorCount, int stackCount)
        {
            setX(x);
            setY(y);
            setZ(z);
            objType = "Solid Circular";

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, _x, _y, _z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }
        public void createEllipsoidV2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            setX(_x);
            setY(_y);
            setZ(_z);
            objType = "Solid Circular";

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }


        //tabung nya akan memanjnag di sb z (horizontal)
        public void createTube(float x, float y, float z, float rad, float thick, double portion, bool downside)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Solid Circular";

            if (portion > 1)
            {
                portion = 1;
            }

            float pi = (float)Math.PI;
            float perimeter = (float)MathHelper.DegreesToRadians(360 * portion);
            Vector3 temp_vector;

            if (downside)
            {
                for (float u = -pi; u <= (-pi + perimeter); u += pi / 360)
                {
                    temp_vector.X = x + (float)Math.Cos(u) * rad;
                    temp_vector.Y = y + (float)Math.Sin(u) * rad;
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);

                    temp_vector.X = x + (float)Math.Cos(u) * rad;
                    temp_vector.Y = y + (float)Math.Sin(u) * rad;
                    temp_vector.Z = z + thick;
                    _vertices.Add(temp_vector);
                }
            }
            else
            {
                for (float u = 0; u <= perimeter; u += pi / 260)
                {
                    temp_vector.X = x + (float)Math.Cos(u) * rad;
                    temp_vector.Y = y + (float)Math.Sin(u) * rad;
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);

                    temp_vector.X = x + (float)Math.Cos(u) * rad;
                    temp_vector.Y = y + (float)Math.Sin(u) * rad;
                    temp_vector.Z = z + thick;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createTube(float x, float y, float z, float rad, float thick, float angleStart, float angleFin)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Solid Circular";

            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float u = MathHelper.DegreesToRadians(angleStart); u <= MathHelper.DegreesToRadians(angleFin); u += pi / 300)
            {
                temp_vector.X = x + (float)Math.Cos(u) * rad;
                temp_vector.Y = y + (float)Math.Sin(u) * rad;
                temp_vector.Z = z;
                _vertices.Add(temp_vector);

                temp_vector.X = x + (float)Math.Cos(u) * rad;
                temp_vector.Y = y + (float)Math.Sin(u) * rad;
                temp_vector.Z = z + thick;
                _vertices.Add(temp_vector);
            }
        }

        public void createRing(float x, float y, float z, float radMajor, float radMinor, float length, float angleStart, float angleFin)
        {
            _x = x;
            _y = y;
            _z = z;
            objType = "Hollow Circular";

            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float u = MathHelper.DegreesToRadians(angleStart); u <= MathHelper.DegreesToRadians(angleFin); u += pi / 300)
            {
                temp_vector.X = x + (float)Math.Cos(u) * radMajor;
                temp_vector.Y = y + (float)Math.Sin(u) * radMajor;
                temp_vector.Z = z;
                _vertices.Add(temp_vector);

                temp_vector.X = x + (float)Math.Cos(u) * radMinor;
                temp_vector.Y = y + (float)Math.Sin(u) * radMinor;
                temp_vector.Z = z;
                _vertices.Add(temp_vector);

                temp_vector.X = x + (float)Math.Cos(u) * radMajor;
                temp_vector.Y = y + (float)Math.Sin(u) * radMajor;
                temp_vector.Z = z + length;
                _vertices.Add(temp_vector);

                temp_vector.X = x + (float)Math.Cos(u) * radMinor;
                temp_vector.Y = y + (float)Math.Sin(u) * radMinor;
                temp_vector.Z = z + length;
                _vertices.Add(temp_vector);
            }
        }

        public void createCrescent(float x, float y, float z, float length, float heigth, float crescWide, bool faceDown)
        {
            setX(x);
            setY(y);
            setZ(z);
            objType = "Hollow Circular";

            float pi = (float)Math.PI;
            Vector3 temp_vector;

            if (faceDown)
            {
                for (float i = -pi; i <= 0; i += (pi / 360))
                {

                    temp_vector.X = x + (float)Math.Cos(i) * length;
                    temp_vector.Y = y + (float)Math.Sin(i) * heigth;
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);

                    temp_vector.X = x + (float)Math.Cos(i) * length;
                    temp_vector.Y = y + (float)Math.Sin(i) * heigth - crescWide;
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }
            else
            {
                for (float i = 0; i <= pi; i += (pi / 360))
                {

                    temp_vector.X = x + (float)Math.Cos(i) * length;
                    temp_vector.Y = y + (float)Math.Sin(i) * heigth;
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);

                    temp_vector.X = x + (float)Math.Cos(i) * length;
                    temp_vector.Y = y + (float)Math.Sin(i) * heigth - crescWide;
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createStaircase(float x, float y, float z, float wide, int steps)
        {
            _x = x;
            _y = y;
            _z = z;

            float stepHeigth = 0.0125f * 1.5f;
            //true heigth = 0.025f

            createBoxVertices(x, y, z, 0.03f, stepHeigth, wide);

            for (int i = 1; i <= steps + 1; i++)
            {
                x += 0.03f;
                stepHeigth += (0.0125f * 1.5f);
                y += (0.00625f * 1.5f);

                createBoxChild(x, y, z, 0.03f, stepHeigth, wide);

                /*
                if(i % 2 == 0)
                {
                    createBoxChild(x, y, z, 0.03f, stepHeigth, wide,0.5f,0.5f,0.5f,1f);
                }
                else
                {
                    createBoxChild(x, y, z, 0.03f, stepHeigth, wide);
                }
                */
            }
        }

        public void spin(Vector3 pivot, Vector3 vector, float angle)
        {
            //pivot -> mau rotate di titik mana
            //vector -> mau rotate di sumbu apa? (x,y,z)
            //angle -> rotatenya berapa derajat?
            var real_angle = angle;
            angle = MathHelper.DegreesToRadians(angle);

            //mulai ngerotasi
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = getRotationResult(pivot, vector, angle, _vertices[i]);
            }
            //rotate the euler direction
            for (int i = 0; i < 3; i++)
            {
                _euler[i] = getRotationResult(pivot, vector, angle, _euler[i], true);

                //NORMALIZE
                //LANGKAH - LANGKAH
                //length = akar(x^2+y^2+z^2)
                float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
                Vector3 temporary = new Vector3(0, 0, 0);
                temporary.X = _euler[i].X / length;
                temporary.Y = _euler[i].Y / length;
                temporary.Z = _euler[i].Z / length;
                _euler[i] = temporary;
            }
            _centerPosition = getRotationResult(pivot, vector, angle, _centerPosition);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes,
                _vertices.ToArray(), BufferUsageHint.StaticDraw);
            foreach (var item in Child)
            {
                item.spin(pivot, vector, real_angle);
            }
        }

        Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;
            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                (float)temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                (float)temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                (float)temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));
            newPosition.Y =
                (float)temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                (float)temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                (float)temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));
            newPosition.Z =
                (float)temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                (float)temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                (float)temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }
        public void resetEuler()
        {
            _euler[0] = new Vector3(1, 0, 0);
            _euler[1] = new Vector3(0, 1, 0);
            _euler[2] = new Vector3(0, 0, 1);
        }

        //method custom 
        public void createDrop(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 7.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 7.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 7.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 7.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createPinggirDrop(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 0.0f;
            temp_vector.Y = y + length / 0.0f;
            temp_vector.Z = z - length / 0.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 7.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 0.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 0.0f;
            temp_vector.Y = y + length / 0.0f;
            temp_vector.Z = z + length / 0.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 7.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 0.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createSebelahDrop(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createSebelahQ(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 7.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createBelakangDrop(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createTackle(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 3.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 2
            //Atas

            temp_vector.X = x + length / 3.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x - length / 3.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 4

            temp_vector.X = x + length / 3.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 5
            //Atas
            temp_vector.X = x - length / 3.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x + length / 3.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x - length / 3.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 8

            temp_vector.X = x + length / 5.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createLilTackle(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 2
            //Atas

            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 4

            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 5
            //Atas
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);

            //TITIK 8

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createBar(float x, float y, float z, float length, float wide)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            Vector3 temp_vector;

            //TITIK 0
            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - wide / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 1
            //Atas

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - wide / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 2

            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - wide / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - wide / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 4
            //Atas
            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + wide / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 5

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + wide / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + wide / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + wide / 1.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,                              
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1 
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createAlas(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            Vector3 temp_vector;

            //TITIK 0
            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 1.8f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 1.5f;
            _vertices.Add(temp_vector);

            //TITIK 2
            //Atas

            temp_vector.X = x + length / 1.8f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 1.5f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x - length / 1.8f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 4

            temp_vector.X = x + length / 1.8f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 5
            //Atas
            temp_vector.X = x - length / 1.8f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 3.5f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x + length / 1.8f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 3.5f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x - length / 1.8f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);

            //TITIK 8

            temp_vector.X = x + length / 1.8f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,                              
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1 
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createBox2(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            Vector3 temp_vector;

            //TITIK 0
            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y + length / 1.0f;
            temp_vector.Z = z - length / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 1
            //Atas

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y + length / 0.0f;
            temp_vector.Z = z - length / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 2

            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 4
            //Atas
            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y + length / 0.0f;
            temp_vector.Z = z + length / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 5

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y + length / 0.0f;
            temp_vector.Z = z + length / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x - length / 1.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 1.0f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x + length / 1.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 1.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,                              
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1 
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createPyramidBox2(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createPyramidBox4(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 10.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 6.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 6.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 10.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 6.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 10.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 6.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createReversePyramidBox2(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createCylinder(float radius, float height, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float i = -pi / 2; i <= pi / 2; i += pi / 360)
            {
                for (float j = -pi; j <= pi; j += pi / 360)
                {
                    temp_vector.X = radius * (float)Math.Cos(i) * (float)Math.Cos(j) + _centerPosition.X;
                    temp_vector.Y = radius * (float)Math.Cos(i) * (float)Math.Sin(j) + _centerPosition.Y;
                    if (temp_vector.Y < _centerPosition.Y)
                        temp_vector.Y = _centerPosition.Y - height * 0.5f;
                    else
                        temp_vector.Y = _centerPosition.Y + height * 0.5f;
                    temp_vector.Z = radius * (float)Math.Sin(i) + _centerPosition.Z;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createBoxChild(float x, float y, float z, float lx, float ly, float lz)
        {
            Assets_3D newChild = new Assets_3D(_color[0], _color[1], _color[2], _color[3]);

            newChild.createBoxVertices(x, y, z, lx, ly, lz);

            Child.Add(newChild);
            childModel.Add(Matrix4.Identity);
        }

        public void createBoxChild(float x, float y, float z, float lx, float ly, float lz, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);

            newChild.createBoxVertices(x, y, z, lx, ly, lz);

            Child.Add(newChild);
            childModel.Add(Matrix4.Identity);
        }

        public void createPararelogramChild(float x, float y, float z, float lx, float ly, float lz, float slope)
        {
            Assets_3D newChild = new Assets_3D(_color[0], _color[1], _color[2], _color[3]);

            newChild.createPararelogram(x, y, z, lx, ly, lz, slope);

            Child.Add(newChild);
            childModel.Add(Matrix4.Identity);
        }

        public void createTubeChild(float x, float y, float z, float rad, float thick, double portion, bool downleft, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);

            newChild.createTube(x, y, z, rad, thick, portion, downleft);

            Child.Add(newChild);
            childModel.Add(Matrix4.Identity);
        }

        public void createCrescentChild(float x, float y, float z, float length, float heigth, float crescWide, bool faceDown, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);

            newChild.createCrescent(x, y, z, length, heigth, crescWide, faceDown);

            Child.Add(newChild);
        }

        public void createRingChild(float x, float y, float z, float radMajor, float radMinor, float length, float angleStart, float angleFin)
        {
            Assets_3D newChild = new Assets_3D();

            newChild.createRing(x, y, z, radMajor, radMinor, length, angleStart, angleFin);

            Child.Add(newChild);
        }

        public void addChild(float x, float y, float z, float length, int pilihan, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);
            if (pilihan == 0)
            {
                newChild.createBoxVertices(x, y, z, length, length, length);
            }
            else if (pilihan == 1)
            {
                newChild.createPyramidBox(x, y, z, length);
            }

            Child.Add(newChild);
            childModel.Add(Matrix4.Identity);
        }

        public void addChildBalok(float x, float y, float z, float length, float wide, float depth, int pilihan, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);
            if (pilihan == 1)
            {
                newChild.createBoxVertices(x, y, z, length, wide, depth);
            }

            Child.Add(newChild);
            childModel.Add(Matrix4.Identity);
        }

        public void addChildElipsoid(float x, float y, float z, float radX, float radY, float radZ, int sectorCount, int stackCount, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);
            newChild.createEllipsoid2(x, y, z, radX, radY, radZ, sectorCount, stackCount);
            Child.Add(newChild);
            childModel.Add(Matrix4.Identity);
        }

        public void addChildElipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);
            newChild.createEllipsoid(radiusX, radiusY, radiusZ, _x, _y, _z);
            Child.Add(newChild);
        }
    }
}
