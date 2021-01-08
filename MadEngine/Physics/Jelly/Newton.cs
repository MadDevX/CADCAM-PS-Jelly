using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Physics
{
    public static class Newton
    {
        public static Quaterniond AdvanceByTimeStep(Quaterniond x, Func<Quaterniond, double, Quaterniond> slope, double t, double dt)
        {
            Quaterniond dir = slope.Invoke(x, t);
            return x + dir * dt;
        }

        public static Vector3d AdvanceByTimeStep(Vector3d x, Func<Vector3d, double, Vector3d> slope, double t, double dt)
        {
            Vector3d dir = slope.Invoke(x, t);
            return x + dir * dt;
        }

        public static double AdvanceByTimeStep(double x, Func<double, double, double> slope, double t, double dt)
        {
            double dir = slope.Invoke(x, t);
            return x + dir * dt;
        }
    }
}
