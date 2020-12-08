﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MadEngine
{
    public enum ProjectionMode
    {
        Perspective,
        Orthographic
    }

    public class Camera : IDisposable
    {

        public Vector3 Target => _target;
        public Vector3 Position => _position;
        public float DistanceToTarget => (_target - _position).Length;
        public float Pitch => _pitch;
        public float NearPlane => _nearPlane;

        private ProjectionMode _projectionMode = ProjectionMode.Perspective;
        public ProjectionMode ProjectionMode 
        {
            get => _projectionMode;
            set
            {
                _projectionMode = value;
                UpdateProjectionMatrix();
            }
        }
        public Matrix4 ProjectionMatrix => _projectionMatrix;
        public Matrix4 ViewMatrix => _viewMatrix;

        public virtual Color4 FilterColor => Color4.White;

        protected ViewportManager _viewportManager;

        protected Vector3 _position = Vector3.UnitZ;
        protected Vector3 _target = Vector3.Zero;

        protected float _yaw;
        protected float _pitch;

        protected float _nearPlane = 0.01f;
        protected float _farPlane = 100.0f;
        protected float _fov = MathHelper.DegreesToRadians(90.0f);
        protected float _aspectRatio = 1.0f;

        protected Matrix4 _projectionMatrix;
        protected Matrix4 _viewMatrix;

        protected float _sensitivity = 1.5f;

        public Camera(ViewportManager viewportManager)
        {
            _viewportManager = viewportManager;
            _viewportManager.OnViewportChanged += OnViewportChanged;
            Reset(0.0f);
        }

        public virtual void Dispose()
        {
            _viewportManager.OnViewportChanged -= OnViewportChanged;
        }

        /// <summary>
        /// Also sets filterColor
        /// </summary>
        /// <param name="shaderWrapper"></param>
        public virtual void SetCameraMatrices(ShaderWrapper shaderWrapper)
        {
            SetViewMatrix(shaderWrapper);
            SetProjectionMatrix(shaderWrapper);
            shaderWrapper.SetFilterColor(FilterColor);
        }

        #region Camera operations

        public void Rotate(float horizontal, float vertical)
        {
            var xAngleDiff = (float)Math.PI * horizontal;
            var yAngleDiff = -(float)Math.PI * vertical;
            if (Math.Abs(_pitch + yAngleDiff) >= Math.PI * 0.499f) yAngleDiff = 0.0f; //limit camera movement
            _yaw = (_yaw + xAngleDiff) % (2.0f * (float)Math.PI);
            _pitch += yAngleDiff;

            RecalculatePosition((_position - _target).Length);

            UpdateViewMatrix();
        }

        public void ChangeFOV(float degrees)
        {
            _fov = MathHelper.Clamp(degrees, MathHelper.DegreesToRadians(1.0f), MathHelper.DegreesToRadians(359.0f));
            UpdateProjectionMatrix();
        }

        public void ChangeOffset(int clicks)
        {
            if(clicks > 0)
            {
                _position += (_target - _position) * 0.1f;
            }
            else if (clicks < 0)
            {
                _position -= (_target - _position) * 0.1f;
            }
            UpdateViewMatrix();
            if (ProjectionMode == ProjectionMode.Orthographic) UpdateProjectionMatrix();
        }

        public void Translate(float horizontal, float vertical)
        {
            var trVec = ViewPlaneVectorToWorldSpace(-horizontal, vertical);
            var toAdd = trVec * DistanceToTarget * _sensitivity;
            _target += toAdd;
            _position += toAdd;
            UpdateViewMatrix();
        }

        public Vector3 ViewPlaneVectorToWorldSpace(Vector2 vect) 
        {
            return ViewPlaneVectorToWorldSpace(vect.X, vect.Y);
        }

        public Vector3 ViewPlaneVectorToWorldSpace(float x, float y)
        {
            var planeTr = new Vector4(x, y, 0.0f, 1.0f);
            var rot = CalculateCameraRotation();
            var trVec = Vector4.Transform(planeTr, rot);
            return new Vector3(trVec.X, trVec.Y, trVec.Z);
        }

        protected Quaternion CalculateCameraRotation()
        {
            return Quaternion.FromMatrix(new Matrix3(Matrix4.LookAt(Vector3.Zero, _target - _position, Vector3.UnitY)));
        }

        /// <summary>
        /// Sets camera pitch, multiplying given parameter by PI.
        /// </summary>
        /// <param name="pitchMult">Value between -1 and 1</param>
        public void Reset(float pitchMult = 0.0f)
        {
            pitchMult = MathHelper.Clamp(pitchMult, 0.0f, 1.0f);
            _position = Vector3.UnitZ;
            _target = Vector3.Zero;
            _yaw = (float)Math.PI*1.5f;
            _pitch = (float)-Math.PI * pitchMult * 0.5f;
            RecalculatePosition(2.0f); //default distance from target
            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        public void Move(Vector3 target)
        {
            var diff = _position - _target;
            _target = target;
            _position = _target + diff;
            UpdateViewMatrix();
        }

        #endregion

        private void RecalculatePosition(float distanceFromTarget)
        {
            Vector3 direction =
                new Vector3
                (
                    (float)Math.Cos(_yaw) * (float)Math.Cos(_pitch),
                    (float)Math.Sin(_pitch),
                    (float)Math.Sin(_yaw) * (float)Math.Cos(_pitch)
                );
            _position = -direction * distanceFromTarget + _target;
        }

        private void SetViewMatrix(ShaderWrapper shaderWrapper)
        {
            shaderWrapper.SetViewMatrix(_viewMatrix);
        }

        private void SetProjectionMatrix(ShaderWrapper shaderWrapper)
        {
            shaderWrapper.SetProjectionMatrix(_projectionMatrix);
        }

        private void OnViewportChanged(System.Drawing.Size obj)
        {
            _aspectRatio = (float)obj.Width / obj.Height;
            UpdateProjectionMatrix();
        }

        protected virtual void UpdateViewMatrix()
        {
            _viewMatrix = Matrix4.LookAt(_position, _target, Vector3.UnitY);
        }

        protected virtual void UpdateProjectionMatrix()
        {
            if (ProjectionMode == ProjectionMode.Perspective)
            {
                _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _nearPlane, _farPlane);
            }
            else
            {
                _projectionMatrix = Matrix4.CreateOrthographic(_aspectRatio * DistanceToTarget, DistanceToTarget, _nearPlane, _farPlane);
            }
        }
    }
}
