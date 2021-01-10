﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public static class MathExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static Matrix4 _powerToBernsteinMtx = new Matrix4(1.0f,    0.0f,        0.0f,     0.0f,
                                                                  1.0f, 1.0f / 3.0f,    0.0f,     0.0f,
                                                                  1.0f, 2.0f / 3.0f, 1.0f / 3.0f, 0.0f,
                                                                  1.0f,    1.0f,        1.0f,     1.0f);

        /// <summary>
        /// Returns maximum absolute value from given values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double MaxAbs(params double[] values)
        {
            var max = 0.0;
            for(int i = 0; i < values.Length; i++)
            {
                var abs = Math.Abs(values[i]);
                max = abs > max ? abs : max; 
            }
            return max;
        }

        public static Vector3 EulerAngles(this Quaternion q)
        {
            var w2 = q.W * q.W;
            var x2 = q.X * q.X; 
            var y2 = q.Y * q.Y; 
            var z2 = q.Z * q.Z; 
            var eX = (float)Math.Atan2(-2 * (q.Y * q.Z - q.W * q.X), w2 - x2 - y2 + z2);
            var eY = (float)Math.Asin(MathHelper.Clamp(2 * (q.X * q.Z + q.W * q.Y), -1.0, 1.0));
            var eZ = (float)Math.Atan2(-2 * (q.X * q.Y - q.W * q.Z), w2 + x2 - y2 - z2);
            return new Vector3(eX, eY, eZ);
        }

        public static (Vector3 a, Vector3 b, Vector3 c, Vector3 d) BSplineToBernstein(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 bernA, bernB, bernC, bernD;
            var oneThird = 1.0f / 3.0f;
            var twoThirds = 2.0f / 3.0f;

            var firstMid = Vector3.Lerp(a, b, twoThirds);
            var secondMid = Vector3.Lerp(b, c, oneThird);
            var thirdMid = Vector3.Lerp(b, c, twoThirds);
            var fourthMid = Vector3.Lerp(c, d, oneThird);
            bernA = Vector3.Lerp(firstMid, secondMid, 0.5f);
            bernB = secondMid;
            bernC = thirdMid;
            bernD = Vector3.Lerp(thirdMid, fourthMid, 0.5f);
            return (bernA, bernB, bernC, bernD);
        }

        public static void PowerToBernstein(Vector3 a, Vector3 b, Vector3 c, Vector3 d, ref Vector3[] result)
        {
            var x = new Vector4(a.X, b.X, c.X, d.X);
            var y = new Vector4(a.Y, b.Y, c.Y, d.Y);
            var z = new Vector4(a.Z, b.Z, c.Z, d.Z);

            x = PowerToBernstein(x);
            y = PowerToBernstein(y);
            z = PowerToBernstein(z);

            result[0] = new Vector3(x.X, y.X, z.X);
            result[1] = new Vector3(x.Y, y.Y, z.Y);
            result[2] = new Vector3(x.Z, y.Z, z.Z);
            result[3] = new Vector3(x.W, y.W, z.W);

        }

        public static Vector4 PowerToBernstein(Vector4 powerBasisCoefficients)
        {
            return _powerToBernsteinMtx * powerBasisCoefficients;
        }

        public static Vector3 RoundToDivisionValue(this Vector3 v, float divisionValue)
        {
            var newPos = v;
            var moduloX = (newPos.X % divisionValue + divisionValue) % divisionValue;
            moduloX = moduloX > 0.5f * divisionValue ? moduloX - divisionValue : moduloX;
            var moduloY = (newPos.Y % divisionValue + divisionValue) % divisionValue;
            moduloY = moduloY > 0.5f * divisionValue ? moduloY - divisionValue : moduloY;
            var moduloZ = (newPos.Z % divisionValue + divisionValue) % divisionValue;
            moduloZ = moduloZ > 0.5f * divisionValue ? moduloZ - divisionValue : moduloZ;
            return newPos - new Vector3(moduloX, moduloY, moduloZ);
        }

        public static Vector3 RoundToDivisionValue(this Vector3 v, float divisionValueRaw, float baseUnit = 1.0f)
        {
            var divisionValue = divisionValueRaw * baseUnit;
            var newPos = v;
            var moduloX = (newPos.X % divisionValue + divisionValue) % divisionValue;
            moduloX = moduloX > 0.5f * divisionValue ? moduloX - divisionValue : moduloX;
            var moduloY = (newPos.Y % divisionValue + divisionValue) % divisionValue;
            moduloY = moduloY > 0.5f * divisionValue ? moduloY - divisionValue : moduloY;
            var moduloZ = (newPos.Z % divisionValue + divisionValue) % divisionValue;
            moduloZ = moduloZ > 0.5f * divisionValue ? moduloZ - divisionValue : moduloZ;
            return newPos - new Vector3(moduloX, moduloY, moduloZ);
        }


        #region Conversion
        public static Vector2 Float(this Vector2d v)
        {
            return new Vector2((float)v.X, (float)v.Y);
        }

        public static Vector2d Double(this Vector2 v)
        {
            return new Vector2d(v.X, v.Y);
        }

        public static Vector3 Float(this Vector3d v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }

        public static Vector3d Double(this Vector3 v)
        {
            return new Vector3d(v.X, v.Y, v.Z);
        }

        public static Vector4 Float(this Vector4d v)
        {
            return new Vector4((float)v.X, (float)v.Y, (float)v.Z, (float)v.W);
        }

        public static Vector4d Double(this Vector4 v)
        {
            return new Vector4d(v.X, v.Y, v.Z, v.W);
        }

        public static Quaternion Float(this Quaterniond v)
        {
            return new Quaternion((float)v.X, (float)v.Y, (float)v.Z, (float)v.W);
        }

        public static Quaterniond Double(this Quaternion v)
        {
            return new Quaterniond(v.X, v.Y, v.Z, v.W);
        }

        public static Vector3d RotateVector(this Quaterniond quat, Vector3d v)
        {
            var q0 = quat.W;
            var q = quat.Xyz;
            return (q0 * q0 - q.LengthSquared) * v + 2.0 * Vector3d.Dot(q, v) * q + 2.0 * q0 * Vector3d.Cross(q, v);
        }

        public static Vector3d Multiply(this Matrix3d mat, Vector3d v)
        {
            var x = Vector3d.Dot(mat.Row0, v);
            var y = Vector3d.Dot(mat.Row1, v);
            var z = Vector3d.Dot(mat.Row2, v);
            return new Vector3d(x, y, z);
        }
        #endregion
    }
}
