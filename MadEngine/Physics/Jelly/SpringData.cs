using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Physics
{
    public struct SpringData
    {
        public double c;
        public double length;
        public Vector3i a;
        public Vector3i b;
        public CubeArray<Vector3d> dataSourceA;
        public CubeArray<Vector3d> dataSourceB;


        public SpringData(double c, double length, Vector3i a, Vector3i b, CubeArray<Vector3d> dataSourceA, CubeArray<Vector3d> dataSourceB)
        {
            this.c = c;
            this.length = length;
            this.a = a;
            this.b = b;
            this.dataSourceA = dataSourceA;
            this.dataSourceB = dataSourceB;
        }
        public SpringData(double c, double length, Vector3i a, Vector3i b, CubeArray<Vector3d> dataSource):this(c, length, a, b, dataSource, dataSource) { }

        public Vector3d APos => dataSourceA[a];
        public Vector3d BPos => dataSourceB[b];
    }
}
