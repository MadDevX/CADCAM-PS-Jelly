using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Utility
{
    /// <summary>
    /// Contains generic meshes to be used across all objects in application
    /// </summary>
    public static class MeshUtility
    {

        /// <summary>
        /// Represents currently bound mesh.
        /// </summary>
        public static Mesh BoundMesh { get; set; } = null;

        private static Vector3[] SelectCPSet(int set)
        {
            if (set == 1) return BezierPositions1;
            if (set == 2) return BezierPositions2;
            if (set == 3) return BezierPositions3;
            throw new InvalidOperationException("invalid point set");
        }
        
        public static (float[] vertices, uint[] indices) WireframeCubeData(float size, Vector3 offset)
        {
            var verts = new float[3*8];
            var idx = new uint[24]
            {
                0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7
            };
            VBOUtility.SetVertex(verts, new Vector3(-0.5f, -0.5f, -0.5f) * size + offset, 0);
            VBOUtility.SetVertex(verts, new Vector3( 0.5f, -0.5f, -0.5f) * size + offset, 1);
            VBOUtility.SetVertex(verts, new Vector3( 0.5f, -0.5f,  0.5f) * size + offset, 2);
            VBOUtility.SetVertex(verts, new Vector3(-0.5f, -0.5f,  0.5f) * size + offset, 3);
            VBOUtility.SetVertex(verts, new Vector3(-0.5f,  0.5f, -0.5f) * size + offset, 4);
            VBOUtility.SetVertex(verts, new Vector3( 0.5f,  0.5f, -0.5f) * size + offset, 5);
            VBOUtility.SetVertex(verts, new Vector3( 0.5f,  0.5f,  0.5f) * size + offset, 6);
            VBOUtility.SetVertex(verts, new Vector3(-0.5f,  0.5f,  0.5f) * size + offset, 7);


            return (verts, idx);
        }

        public static Mesh Cube()
        {
            var layoutType = VertexLayout.Type.PositionNormalTexCoords;
            var mesh = new Mesh(layoutType);
            //36 vertices
            float[] vertices = {
                    // positions          // normals           // texture coords
                    -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                     0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
                    -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
                     0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,

                    -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
                     0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
                    -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
                     0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,

                    -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                    -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                    -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                    -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

                     0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                     0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                     0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                     0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

                    -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                     0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
                    -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
                     0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,

                    -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
                     0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
                    -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
                     0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f
            };

            uint[] indices = new uint[24];
            for (uint i = 0; i < 24; i++) indices[i] = i;

            mesh.SetBufferData(vertices, indices, BufferUsageHint.StaticDraw);
            return mesh;
        }

        public static Mesh BezierJoined(float width, float height, int segmentsX, int segmentsZ)
        {
            var mesh = new Mesh(VertexLayout.Type.PositionNormalTexCoords);
            var verticesX = 1 + 3 * segmentsX;
            var verticesZ = 1 + 3 * segmentsZ;
            var vertexCount = verticesX * verticesZ;
            float[] vertices = new float[vertexCount * VertexLayout.Stride(VertexLayout.Type.PositionNormalTexCoords)];
            int vertexIndex = 0;

            float deltaW = width  / (verticesX-1);
            float deltaH = height / (verticesZ-1);
            bool up = false;
            for (int z = 0; z < verticesX; z++)
            {
                up = false;
                for (int x = 0; x < verticesZ; x++)
                {
                    if (x % 3 == 0) up = !up;
                    var y = x % 3 == 1 || x % 3 == 2 ? width * 0.06f : 0.0f;
                    if (up == false) y = -y;

                    var pos = new Vector3(-width * 0.5f + deltaW * x, y, -height * 0.5f + deltaH * z);
                    var norm = new Vector3(0.0f, 1.0f, 0.0f);
                    var texCoords = new Vector2((float)x / (verticesX-1), (float)z / (verticesZ-1));
                    VBOUtility.SetVertex(vertices, pos, norm, texCoords, vertexIndex++);
                }
            }

            uint[] indices = new uint[segmentsX * segmentsZ * 16];
            int indicesIdx = 0;
            for (int vPatch = 0; vPatch < segmentsZ; vPatch++)
            {
                for (int uPatch = 0; uPatch < segmentsX; uPatch++)
                {
                    var uIdx = uPatch * 3;
                    var vIdx = vPatch * 3;
                    for (int j = vIdx; j < vIdx + 4; j++)
                    {
                        for (int i = uIdx; i < uIdx + 4; i++)
                        {
                            indices[indicesIdx] = (uint)(i + j * verticesX);
                            indicesIdx++;
                        }
                    }

                }
            }
            mesh.SetBufferData(vertices, indices, BufferUsageHint.StaticDraw);
            return mesh;
        }

        public static Mesh BezierPatch(float width, float height, int pointSet)
        {
            var mesh = new Mesh(VertexLayout.Type.PositionNormalTexCoords);
            var vertexCount = 16;
            float[] vertices = new float[vertexCount * VertexLayout.Stride(VertexLayout.Type.PositionNormalTexCoords)];
            int vertexIndex = 0;

            var segmentsX = 3;
            var segmentsZ = 3;

            float deltaW = width / 3;
            float deltaH = height / 3;

            var cpSet = SelectCPSet(pointSet);

            for (int z = 0; z <= segmentsX; z++)
            {
                for (int x = 0; x <= segmentsZ; x++)
                {
                    var pos = cpSet[x + z * (segmentsX+1)];
                    var norm = new Vector3(0.0f, 1.0f, 0.0f);
                    var texCoords = new Vector2((float)x / segmentsX, (float)z / segmentsZ);
                    VBOUtility.SetVertex(vertices, pos, norm, texCoords, vertexIndex++);
                }
            }

            uint[] indices = new uint[16];
            for (int i = 0; i < 16; i++) indices[i] = (uint)i;
            mesh.SetBufferData(vertices, indices, BufferUsageHint.StaticDraw);
            return mesh;
        }

        public static Mesh PlaneMesh(PrimitiveType type, float width, float height, int segmentsX = 1, int segmentsZ = 1)
        {
            var mesh = new Mesh(VertexLayout.Type.PositionNormalTexCoords);
            var vertexCount = 4 + 2 * (segmentsX-1) + (1 + segmentsX) * (segmentsZ -1);
            float deltaW = width / segmentsX;
            float deltaH = height / segmentsZ;

            float[] vertices = new float[vertexCount * VertexLayout.Stride(VertexLayout.Type.PositionNormalTexCoords)];
            int vertexIndex = 0;
            for(int z = 0; z <= segmentsZ; z++)
            {
                for(int x = 0; x <= segmentsX; x++)
                {
                    var pos = new Vector3(-width * 0.5f + deltaW * x, 0.0f, -height * 0.5f + deltaH * z);
                    var norm = new Vector3(0.0f, 1.0f, 0.0f);
                    var texCoords = new Vector2((float)x / segmentsX, (float)z / segmentsZ);
                    VBOUtility.SetVertex(vertices, pos, norm, texCoords, vertexIndex++);
                }
            }
            if (type == PrimitiveType.Triangles)
            {
                mesh.SetBufferData(vertices, PlaneIndicesTriangles(segmentsX, segmentsZ), BufferUsageHint.StaticDraw);
            }
            else if (type == PrimitiveType.Patches)
            {
                mesh.SetBufferData(vertices, PlaneIndicesPatches(segmentsX, segmentsZ), BufferUsageHint.StaticDraw);
            }
            else throw new InvalidOperationException("Only Triangles and Patches allowed");
            return mesh;
        }

        private static uint[] PlaneIndicesTriangles(int segmentsX, int segmentsZ)
        {
            int tableW = segmentsX + 1;
            int tableH = segmentsZ + 1;

            uint[] indices = new uint[segmentsX * segmentsZ * 6];
            int indexIdx = 0;
            for (int i = 0; i < segmentsX; i++)
            {
                for (int j = 0; j < segmentsZ; j++)
                {
                    var startIdx = (uint)(tableW * j + i);
                    //First triangle
                    indices[indexIdx++] = startIdx;
                    indices[indexIdx++] = startIdx + (uint)tableW;
                    indices[indexIdx++] = startIdx + 1;
                    //Second triangle
                    indices[indexIdx++] = startIdx + 1;
                    indices[indexIdx++] = startIdx + (uint)tableW;
                    indices[indexIdx++] = startIdx + (uint)tableW + 1;
                }
            }
            return indices;
        }

        private static uint[] PlaneIndicesPatches(int segmentsX, int segmentsZ)
        {
            int tableW = segmentsX + 1;
            int tableH = segmentsZ + 1;

            uint[] indices = new uint[segmentsX * segmentsZ * 4];
            int indexIdx = 0;
            for (int i = 0; i < segmentsX; i++)
            {
                for (int j = 0; j < segmentsZ; j++)
                {
                    var startIdx = (uint)(tableW * j + i);
                    indices[indexIdx++] = startIdx;
                    indices[indexIdx++] = startIdx + 1;
                    indices[indexIdx++] = startIdx + (uint)tableW;
                    indices[indexIdx++] = startIdx + (uint)tableW + 1;
                }
            }
            return indices;
        }

        private static Vector3[] BezierPositions1 =
        {
            new Vector3(-0.6f, 0.0f, -0.6f),
            new Vector3(-0.2f, 0.0f, -0.6f),
            new Vector3( 0.2f, 0.0f, -0.6f),
            new Vector3( 0.6f, 0.0f, -0.6f),

            new Vector3(-0.6f, 0.0f, -0.2f),
            new Vector3(-0.2f, 0.4f, -0.2f),
            new Vector3( 0.2f, 0.4f, -0.2f),
            new Vector3( 0.6f, 0.0f, -0.2f),

            new Vector3(-0.6f, 0.0f,  0.2f),
            new Vector3(-0.2f, 0.4f,  0.2f),
            new Vector3( 0.2f, 0.4f,  0.2f),
            new Vector3( 0.6f, 0.0f,  0.2f),

            new Vector3(-0.6f, 0.0f,  0.6f),
            new Vector3(-0.2f, 0.0f,  0.6f),
            new Vector3( 0.2f, 0.0f,  0.6f),
            new Vector3( 0.6f, 0.0f,  0.6f)
        };

        private static Vector3[] BezierPositions2 =
{
            new Vector3(-0.6f, 0.0f, -0.6f),
            new Vector3(-0.2f, 0.0f, -0.6f),
            new Vector3( 0.2f, 0.0f, -0.6f),
            new Vector3( 0.6f, 0.0f, -0.6f),

            new Vector3(-0.6f, 0.0f, -0.2f),
            new Vector3(-0.1f, 0.4f, -0.3f),
            new Vector3( 0.2f, 0.4f, -0.3f),
            new Vector3( 0.6f, 0.0f, -0.2f),

            new Vector3(-0.6f, 0.0f,  0.2f),
            new Vector3(-0.3f, 0.2f,  0.3f),
            new Vector3( 0.4f, 0.2f,  0.3f),
            new Vector3( 0.6f, 0.0f,  0.2f),

            new Vector3(-0.6f, 0.0f,  0.6f),
            new Vector3(-0.2f, 0.0f,  0.6f),
            new Vector3( 0.2f, 0.0f,  0.6f),
            new Vector3( 0.6f, 0.0f,  0.6f)
        };

        private static Vector3[] BezierPositions3 =
{
            new Vector3(-0.6f, 0.0f, -0.6f),
            new Vector3(-0.2f, 0.0f, -0.6f),
            new Vector3( 0.2f, 0.0f, -0.6f),
            new Vector3( 0.6f, 0.0f, -0.6f),

            new Vector3(-0.6f,  0.0f, -0.2f),
            new Vector3(-0.2f, -0.4f, -0.2f),
            new Vector3( 0.2f, -0.4f, -0.2f),
            new Vector3( 0.6f,  0.0f, -0.2f),

            new Vector3(-0.6f,  0.0f,  0.2f),
            new Vector3(-0.2f, -0.4f,  0.2f),
            new Vector3( 0.2f, -0.4f,  0.2f),
            new Vector3( 0.6f,  0.0f,  0.2f),

            new Vector3(-0.6f,  0.0f,  0.6f),
            new Vector3(-0.2f,  0.0f,  0.6f),
            new Vector3( 0.2f,  0.0f,  0.6f),
            new Vector3( 0.6f,  0.0f,  0.6f)
        };
    }
}
