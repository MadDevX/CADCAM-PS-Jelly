using MadEngine.ParametricObjects;
using MadEngine.Physics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MadEngine.Miscellaneous
{
    public static class Bezier
    {
        private static ThreadLocal<Vector3d[]> _coordBufferSurf = new ThreadLocal<Vector3d[]>(() => new Vector3d[16]);
        private static ThreadLocal<Vector3d[]> _coordBufferCurv = new ThreadLocal<Vector3d[]>(() => new Vector3d[4]);
        public static Vector3d GetPoint(CubeArray<Vector3d> array, double u, double v, double w)
        {
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                {
                        _coordBufferSurf.Value[x + y * 4] = BezierHelper.Bezier(array[x, y, 0], array[x, y, 1], array[x, y, 2], array[x, y, 3], w);
                }

            for(int x = 0; x < 4; x++)
            {
                _coordBufferCurv.Value[x] = BezierHelper.Bezier(_coordBufferSurf.Value[x + 4 * 0], _coordBufferSurf.Value[x + 4 * 1], _coordBufferSurf.Value[x + 4 * 2], _coordBufferSurf.Value[x + 4 * 3], v);
            }

            return BezierHelper.Bezier(_coordBufferCurv.Value[0], _coordBufferCurv.Value[1], _coordBufferCurv.Value[2], _coordBufferCurv.Value[3], u);
        }

        public static Vector3d GetPoint(CubeArray<Vector3d> array, Vector3d uvw)
        {
            return GetPoint(array, uvw.X, uvw.Y, uvw.Z);
        }
    }
}
