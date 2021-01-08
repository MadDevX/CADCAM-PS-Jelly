using System.Linq;

namespace MadEngine.Physics
{
    public struct Vector3i
    {
        public int X;
        public int Y;
        public int Z;

        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class CubeArray<T>
    {
        private T[] _elements;
        public int xSize;
        public int ySize;
        public int zSize;

        public int Length => _elements.Length;

        public CubeArray(int xSize, int ySize, int zSize)
        {
            this.xSize = xSize;
            this.ySize = ySize;
            this.zSize = zSize;
            _elements = new T[xSize * ySize * zSize];
        }

        public CubeArray(CubeArray<T> source)
        {
            xSize = source.xSize;
            ySize = source.ySize;
            zSize = source.zSize;
            _elements = source._elements.ToArray();
        }

        public T this[int x, int y, int z]
        {
            get => _elements[GetIndex(x,y,z)];
            set => _elements[GetIndex(x,y,z)] = value;
        }

        public T this[Vector3i elem]
        {
            get => _elements[GetIndex(elem.X, elem.Y, elem.Z)];
            set => _elements[GetIndex(elem.X, elem.Y, elem.Z)] = value;
        }

        public T this[int i]
        {
            get => _elements[i];
            set => _elements[i] = value;
        }

        private int GetIndex(int x, int y, int z)
        {
            return x + z * xSize + y * xSize * zSize;
        }
    }
}
