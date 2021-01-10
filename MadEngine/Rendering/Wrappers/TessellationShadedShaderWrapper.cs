using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class TessellationShadedShaderWrapper : ShadedShaderWrapper
    {
        public int PatchVertices { get; }
        public bool OverrideTessLevels { get; }


        private int _shaderTessLevelOuterLocation;
        private int _shaderTessLevelInnerLocation;

        public TessellationShadedShaderWrapper(Shader shader, int patchVertices, bool overrideTessLevels, bool isTextured, string name) : base(shader, isTextured, name)
        {
            PatchVertices = patchVertices;
            OverrideTessLevels = overrideTessLevels;
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
