using MadEngine.Components;
using MadEngine.Rendering;
using MadEngine.Utility;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MadEngine.Miscellaneous
{
    public class JellySimulation : IDisposable
    {   
        private SceneManager _sceneManager;
        private ShaderProvider _shaderProvider;

        //JELLY
        public LineRenderer WireframeRenderer { get; private set; }
        public LineRenderer ControlFrameRenderer { get; private set; }
        public DynamicPatchRenderer JellyRenderer { get; private set; }
        public DynamicMeshRenderer JellyMeshRenderer { get; private set; }

        public Node Jelly { get; private set; }
        public Node ControlFrame { get; private set; }

        public JellyController JellyController { get; private set; }

        public JellySimulation(SceneManager sceneManager, ShaderProvider shaderProvider)
        {
            _sceneManager = sceneManager;
            _shaderProvider = shaderProvider;

            ControlFrame = new Node(new Transform(Vector3.Zero, Quaternion.Identity /* Quaternion.FromEulerAngles(1.0f, 1.0f, 0.0f)*/, Vector3.One), "controlFrame");
            ControlFrameRenderer = new LineRenderer(shaderProvider.DefaultShader);
            var jellyControlFrame = new JellyControlFrame(ControlFrameRenderer, 1.0);
            ControlFrame.AttachComponents(ControlFrameRenderer, jellyControlFrame);

            Jelly = new Node(new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One), "jelly");
            
            var boundingBoxRenderer = new LineRenderer(shaderProvider.DefaultShader);
            var jellyBoundingBox = new JellyBoundingBox(boundingBoxRenderer, 5.0);
            
            var jellyData = new JellyData();
            JellyRenderer = new DynamicPatchRenderer(shaderProvider.SurfaceShaderBezier, new Mesh(VertexLayout.Type.Position), jellyData);
            JellyController = new JellyController(jellyControlFrame);

            WireframeRenderer = new LineRenderer(shaderProvider.DefaultShader);
            var jellyCPVisualizer = new JellyControlPointVisualizer(WireframeRenderer);
            Jelly.AttachComponents(jellyData, JellyRenderer, JellyController, boundingBoxRenderer, jellyBoundingBox, 
                                    WireframeRenderer, jellyCPVisualizer);


            _sceneManager.CurrentScene.AttachChild(Jelly);
            _sceneManager.CurrentScene.AttachChild(ControlFrame);

            var meshData = MeshLoader.LoadObj("monkey2.obj");
            var monkey = new Node(new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One), "monkey");
            var jellyMesh = new JellyMesh(jellyData, meshData.positions, meshData.normals);
            JellyMeshRenderer = new DynamicMeshRenderer(shaderProvider.PhongShader, meshData.mesh);

            monkey.AttachComponents(JellyMeshRenderer, jellyMesh);
            _sceneManager.CurrentScene.AttachChild(monkey);
        }

        public void Dispose()
        {
            Jelly.Dispose();
            ControlFrame.Dispose();

            Jelly = null;
            ControlFrame = null;
            WireframeRenderer = null;
            JellyRenderer = null;
            ControlFrameRenderer = null;
            JellyController = null;
        }
    }
}
