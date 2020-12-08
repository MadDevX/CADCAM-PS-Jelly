using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class TesselationShadedShaderWrapper : TesselationShaderWrapper
    {
        public int PatchVertices { get; }
        public bool OverrideTessLevels { get; }
        public bool IsTextured { get; }

        private int _shaderNormalMatrixLocation;
        private int _shaderLightPosLocation;
        private int _shaderLightColLocation;
        private int _shaderCamPosLocation;
        private int _shaderDiffuseMapLocation = -1;
        private int _shaderHeightMapLocation = -1;
        private int _shaderNormalMapLocation = -1;
        public TesselationShadedShaderWrapper(Shader shader, int patchVertices, bool overrideTessLevels, bool isTextured, string name) : base(shader, name)
        {
            PatchVertices = patchVertices;
            OverrideTessLevels = overrideTessLevels;
            IsTextured = isTextured;
        }

        protected override void SetUniformLocations()
        {
            base.SetUniformLocations();
            _shaderNormalMatrixLocation = GL.GetUniformLocation(Shader.Handle, "normal");
            _shaderLightPosLocation = GL.GetUniformLocation(Shader.Handle, "lightPos");
            _shaderLightColLocation = GL.GetUniformLocation(Shader.Handle, "lightCol");
            _shaderCamPosLocation = GL.GetUniformLocation(Shader.Handle, "camPos");
            _shaderDiffuseMapLocation = GL.GetUniformLocation(Shader.Handle, "diffuseMap");
            _shaderHeightMapLocation =  GL.GetUniformLocation(Shader.Handle, "heightMap");
            _shaderNormalMapLocation =  GL.GetUniformLocation(Shader.Handle, "normalMap");
        }

        public void SetNormalMatrix(Matrix3 mtx)
        {
            CheckShaderBinding();
            GL.UniformMatrix3(_shaderNormalMatrixLocation, false, ref mtx);
        }

        public void SetLightPosition(Vector3 position)
        {
            CheckShaderBinding();
            GL.Uniform3(_shaderLightPosLocation, position);
        }

        public void SetLightColor(Vector3 color)
        {
            CheckShaderBinding();
            GL.Uniform3(_shaderLightColLocation, color);
        }
        public void SetCameraPosition(Vector3 position)
        {
            CheckShaderBinding();
            GL.Uniform3(_shaderCamPosLocation, position);
        }

        public void SetDiffuseMap(int textureUnit)
        {
            if (IsTextured)
            {
                CheckShaderBinding();
                GL.Uniform1(_shaderDiffuseMapLocation, textureUnit);
            }
        }

        public void SetHeightMap(int textureUnit)
        {
            if (IsTextured)
            {
                CheckShaderBinding();
                GL.Uniform1(_shaderHeightMapLocation, textureUnit);
            }
        }

        public void SetNormalMap(int textureUnit)
        {
            if (IsTextured)
            {
                CheckShaderBinding();
                GL.Uniform1(_shaderNormalMapLocation, textureUnit);
            }
        }
    }
}
