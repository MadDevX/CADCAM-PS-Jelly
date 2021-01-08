using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Physics
{
    public class InertiaData
    {
        /// <summary>
        /// Tensor in respect to corner at (0,0,0), where cube is from (0,0,0) to (size, size, size).
        /// </summary>
        public Matrix3d Tensor { get; private set; }
        /// <summary>
        /// Inverse tensor in respect to corner at (0,0,0), where cube is from (0,0,0) to (size, size, size).
        /// </summary>
        public Matrix3d InverseTensor { get; private set; }
        public double Mass { get; private set; }
        public Vector3d CenterOfMass { get; private set; }

        public InertiaData(double size, double ro)
        {
            Recalculate(size, ro);
        }

        public void Recalculate(double size, double ro)
        {
            Mass = CubeMass(size, ro);
            CenterOfMass = CubeCenterOfMass(size);
            Tensor = Steiner(CubeTensorCoM(size, ro), -CenterOfMass, Mass);
            InverseTensor = InvertedTensor(size, ro);
        }

        private Matrix3d InvertedTensor(double size, double ro)
        {
            var a5ro = size * size * size * size * size * ro;
            var denom = 1.0 / (11.0 * a5ro);
            return new Matrix3d(new Vector3d(30.0 * denom, 18.0 * denom, 18.0 * denom),
                                new Vector3d(18.0 * denom, 30.0 * denom, 18.0 * denom),
                                new Vector3d(18.0 * denom, 18.0 * denom, 30.0 * denom));
        }

        public Matrix3d RotateTensor(Matrix3d tensor, Matrix3d rotationMatrix)
        {
            var trRot = Matrix3d.Transpose(rotationMatrix);
            return Matrix3d.Mult(Matrix3d.Mult(rotationMatrix, tensor), trRot);
        }


        private double CubeMass(double size, double ro)
        {
            return size * size * size * ro;
        }
        private Vector3d CubeCenterOfMass(double size)
        {
            return new Vector3d(size * 0.5, size * 0.5, size * 0.5);
        }

        private Matrix3d CubeTensorCoM(double size, double ro)
        {
            var a5 = size * size * size * size * size;
            var elem = a5 * ro / 6.0;
            return new Matrix3d(new Vector3d(elem, 0.0, 0.0),
                                new Vector3d(0.0, elem, 0.0),
                                new Vector3d(0.0, 0.0, elem));
        }

        private Matrix3d PointMassTensor(double mass, Vector3d position)
        {
            var x = position.X;
            var y = position.Y;
            var z = position.Z;
            return new Matrix3d(mass * new Vector3d(y*y + z*z, -x*y, -x*z),
                                mass * new Vector3d(-x*y, x*x+z*z, -y*z),
                                mass * new Vector3d(-x*z, -y*z, x*x+y*y));
        }

        private Matrix3d Steiner(Matrix3d tensorCoM, Vector3d offset, double bodyMass)
        {
            return Matrix3d.Add(tensorCoM, PointMassTensor(bodyMass, offset));
        }

        private Matrix3d SteinerToCoM(Matrix3d offsetTensor, Vector3d offset, double bodyMass)
        {
            var pointTensor = PointMassTensor(bodyMass, offset);
            pointTensor.Row0 *= -1.0;
            pointTensor.Row1 *= -1.0;
            pointTensor.Row2 *= -1.0;
            return Matrix3d.Add(offsetTensor, pointTensor);
        }
    }
}
