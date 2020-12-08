using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Physics
{
    public static class RungeKutta
    {
        public static Quaterniond AdvanceByTimeStep(Quaterniond x, Func<Quaterniond, double, Quaterniond> slope, double t, double dt)
        {
            double ht = dt * 0.5;
            Quaterniond k1 = slope.Invoke(x, t);
            Quaterniond k2 = slope.Invoke(x + k1 * ht, t + ht);
            Quaterniond k3 = slope.Invoke(x + k2 * ht, t + ht);
            Quaterniond k4 = slope.Invoke(x + k3 * dt, t + dt);

            double denom = 1.0 / 6.0;
            Quaterniond avgSlope = (k1 + 2.0 * k2 + 2.0 * k3 + k4) * denom;

            return x + avgSlope * dt;
        }

        public static Vector3d AdvanceByTimeStep(Vector3d x, Func<Vector3d, double, Vector3d> slope, double t, double dt)
        {
            double ht = dt * 0.5;
            Vector3d k1 = slope.Invoke(x, t);
            Vector3d k2 = slope.Invoke(x + k1 * ht, t + ht);
            Vector3d k3 = slope.Invoke(x + k2 * ht, t + ht);
            Vector3d k4 = slope.Invoke(x + k3 * dt, t + dt);

            Vector3d avgSlope = (k1 + 2.0 * k2 + 2.0 * k3 + k4) / 6.0;

            return x + avgSlope * dt;
        }

        public static double AdvanceByTimeStep(double x, Func<double, double, double> slope, double t, double dt)
        {
            double ht = dt * 0.5;
            double k1 = slope.Invoke(x, t);
            double k2 = slope.Invoke(x + k1 * ht, t + ht);
            double k3 = slope.Invoke(x + k2 * ht, t + ht);
            double k4 = slope.Invoke(x + k3 * dt, t + dt);

            double avgSlope = (k1 + 2.0 * k2 + 2.0 * k3 + k4) / 6.0;

            return x + avgSlope * dt;
        }

    }
}
