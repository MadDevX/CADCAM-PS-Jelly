﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class ShaderProvider : IDisposable
    {
        /// <summary>
        /// Transforms by model and camera matrices, no lighting.
        /// </summary>
        public readonly ShaderWrapper DefaultShader;
        /// <summary>
        /// Transforms only by camera matrices (vertices are rendered without model matrix transformation), no lighting.
        /// </summary>
        public readonly ShaderWrapper DefaultWorldShader;

        public readonly TessellationShadedShaderWrapper SurfaceShaderBezier;
        public readonly TessellationShadedShaderWrapper SurfaceShaderBezierDynamicLOD;
        public readonly SimpleShaderWrapper OverlayShader;
        public readonly TessellationShadedShaderWrapper TesselatedPhongShader;
        public readonly ShadedShaderWrapper PhongShader;


        public ShaderProvider()
        {
            DefaultShader = new ShaderWrapper(new Shader(ShaderPaths.VSPath, ShaderPaths.FSPath), nameof(DefaultShader));
            DefaultWorldShader = new ShaderWrapper(new Shader(ShaderPaths.VSWorldPath, ShaderPaths.FSPath), nameof(DefaultWorldShader));
            OverlayShader = new SimpleShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.SimpleFSPath), nameof(OverlayShader));

            //TODO: use dynamic LOD tesc and change override LOD variable
            SurfaceShaderBezierDynamicLOD = new TessellationShadedShaderWrapper(new Shader(ShaderPaths.SimplePhongVSPath, ShaderPaths.TESCBezierDynamicLODPath, ShaderPaths.TESEBezierTexturedPath, ShaderPaths.PhongTexturedFSPath), 16, true, true, nameof(SurfaceShaderBezierDynamicLOD));
            SurfaceShaderBezier =           new TessellationShadedShaderWrapper(new Shader(ShaderPaths.SimplePhongVSPath, ShaderPaths.TESCBezierPath, ShaderPaths.TESEBezierPath, ShaderPaths.PhongFSPath), 16, true, false, nameof(SurfaceShaderBezier));
            TesselatedPhongShader =         new TessellationShadedShaderWrapper(new Shader(ShaderPaths.SimplePhongVSPath, ShaderPaths.TESCQuadPath, ShaderPaths.TESEQuadPath, ShaderPaths.PhongFSPath), 4, true, false, nameof(TesselatedPhongShader));
            PhongShader =                   new ShadedShaderWrapper(new Shader(ShaderPaths.PhongVSPath, ShaderPaths.PhongFSPath), false, nameof(PhongShader));
        }

        public void UpdateShadersCameraMatrices(Camera camera)
        {
            DefaultShader.Shader.Use();
            camera.SetCameraMatrices(DefaultShader); 
            DefaultWorldShader.Shader.Use();
            camera.SetCameraMatrices(DefaultWorldShader);
            SurfaceShaderBezier.Shader.Use();
            camera.SetCameraMatrices(SurfaceShaderBezier);
            SurfaceShaderBezierDynamicLOD.Shader.Use();
            camera.SetCameraMatrices(SurfaceShaderBezierDynamicLOD);
            TesselatedPhongShader.Shader.Use();
            camera.SetCameraMatrices(TesselatedPhongShader);
            PhongShader.Shader.Use();
            camera.SetCameraMatrices(PhongShader);
        }

        public void Dispose()
        {
            TesselatedPhongShader.Dispose();
            OverlayShader.Dispose();
            SurfaceShaderBezierDynamicLOD.Dispose();
            SurfaceShaderBezier.Dispose();
            DefaultWorldShader.Dispose();
            DefaultShader.Dispose();
        }
    }
}
