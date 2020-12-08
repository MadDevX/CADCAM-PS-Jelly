using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class TesselationShaderWrapper : ShaderWrapper
    {
        private int _shaderTessLevelOuterLocation;
        private int _shaderTessLevelInnerLocation;

        public TesselationShaderWrapper(Shader shader, string name) : base(shader, name)
        {
        }

        protected override void SetUniformLocations()
        {
            base.SetUniformLocations();
            _shaderTessLevelOuterLocation = GL.GetUniformLocation(Shader.Handle, "tessLevelOuter");
            _shaderTessLevelInnerLocation = GL.GetUniformLocation(Shader.Handle, "tessLevelInner");
        }

        public void SetTessLevelOuter(int tessLevel)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderTessLevelOuterLocation, tessLevel);
        }

        public void SetTessLevelInner(int tessLevel)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderTessLevelInnerLocation, tessLevel);
        }
    }
}
