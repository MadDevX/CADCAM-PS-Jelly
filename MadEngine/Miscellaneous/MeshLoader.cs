using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadEngine.Utility;
using ObjLoader;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using OpenTK;

namespace MadEngine.Miscellaneous
{
    public class MeshLoader
    {
        public struct VertexObjData
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 texCoord;
            public int index;

            public VertexObjData(Vector3 position, Vector3 normal, Vector2 texCoord, int index)
            {
                this.position = position;
                this.normal = normal;
                this.texCoord = texCoord;
                this.index = index;
            }
        }

        public static (Mesh mesh, Vector3[] positions, Vector3[] normals) LoadObj(string path)
        {
            ObjLoaderFactory factory = new ObjLoaderFactory();
            var objLoader = factory.Create();
            using (var fStream = new FileStream(path, FileMode.Open))
            {
                var result = objLoader.Load(fStream);

                Dictionary<FaceVertex, VertexObjData> vertexData = new Dictionary<FaceVertex, VertexObjData>();

                var verts = result.Vertices;
                var norms = result.Normals;
                var tex = result.Textures;
                var faces = result.Groups[0].Faces;
                uint[] indices = new uint[faces.Count * 3];
                for (int i = 0; i < faces.Count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var vert = faces[i][j];

                        if (vertexData.ContainsKey(vert) == false)
                        {
                            var v = ConvertVert(verts[vert.VertexIndex - 1]);
                            var n = ConvertNorm(norms[vert.NormalIndex - 1]);
                            var t = ConvertTexCoord(tex[vert.TextureIndex - 1]);
                            vertexData.Add(vert, new VertexObjData(v, n, t, vertexData.Count));
                        }
                    }
                }

                List<VertexObjData> sortedVertexData = vertexData.Values.ToList();

                //sortedVertexData.Sort((a, b) => a.index.CompareTo(b.index));

                for (int i = 0; i < faces.Count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        indices[i * 3 + j] = (uint)vertexData[faces[i][j]].index;
                    }
                }

                var positions = new Vector3[sortedVertexData.Count];
                var normals = new Vector3[sortedVertexData.Count];
                float[] vertices = new float[sortedVertexData.Count * VertexLayout.Stride(VertexLayout.Type.PositionNormal)];
                for (int i = 0; i < sortedVertexData.Count; i++)
                {
                    var vert = sortedVertexData[i];
                    positions[vert.index] = vert.position;
                    normals[vert.index] = vert.normal;
                    VBOUtility.SetVertex(vertices, vert.position, vert.normal, vert.index);
                }

                var mesh = new Mesh(VertexLayout.Type.PositionNormal);
                mesh.SetBufferData(vertices, indices, OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw);
                return (mesh, positions, normals);
            }
        }

        private static Vector3 ConvertVert(ObjLoader.Loader.Data.VertexData.Vertex vertex)
        {
            return new Vector3(vertex.X, vertex.Y, vertex.Z);
        }
        private static Vector3 ConvertNorm(ObjLoader.Loader.Data.VertexData.Normal normal)
        {
            return new Vector3(normal.X, normal.Y, normal.Z);
        }
        private static Vector2 ConvertTexCoord(ObjLoader.Loader.Data.VertexData.Texture tex)
        {
            return new Vector2(tex.X, tex.Y);
        }
    }
}
