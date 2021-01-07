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
        
        //SPINNING TOP
        public LineRenderer DiagonalRenderer { get; private set; }
        public LineRenderer CubeRenderer { get; private set; }
        public LineRenderer TrajectoryRenderer { get; private set; }
        public TrajectoryDrawer TrajectoryDrawer { get; private set; }
        public SpinningTopMovement SpinningTopMovement { get; private set; }
        public Node Cube { get; private set; }


        //JELLY
        public LineRenderer WireframeRenderer { get; private set; }
        public LineRenderer ControlFrameRenderer { get; private set; }
        public Node Jelly { get; private set; }
        public Node ControlFrame { get; private set; }

        public float CubeSize
        {
            get => (float)SpinningTopMovement.CubeSize;
            set
            {
                SpinningTopMovement.CubeSize = value;
                SpinningTopMovement.RestartSimulation();
            }
        }

        public float CubeDensity
        {
            get => (float)SpinningTopMovement.CubeDensity;
            set
            {
                SpinningTopMovement.CubeDensity = value;
                SpinningTopMovement.RestartSimulation();
            }
        }

        public float Omega
        {
            get => (float)SpinningTopMovement.Omega;
            set
            {
                SpinningTopMovement.Omega = value;
                SpinningTopMovement.RestartSimulation();
            }
        }

        private float _defaultDensity = 1.0f;
        private float _defaultSize = 1.0f;


        private float _diagonalInclination = 0.0f;
        public float DiagonalInclination 
        {
            get => _diagonalInclination; 
            set
            {
                _diagonalInclination = value;
                var axis = new Vector3(0.0f, 0.0f, 1.0f).Normalized();
                var rot = Quaternion.FromAxisAngle(axis, MathHelper.DegreesToRadians(value));
                var newQuat = rot * _baseRotation;
                Cube.Transform.Rotation = newQuat;
                SpinningTopMovement.RestartSimulation();
                //var q = Cube.Transform.Rotation.Double();
                //var rotatedVector = q.RotateVector(new Vector3d(1.0, 1.0, 1.0));
                //MessageBox.Show(rotatedVector.ToString());
            }
        }

        //                                                                                                                change sign to swap cube rotation (top to bottom)  
        private Quaternion _baseRotation = //Quaternion.Identity;
                                           Quaternion.FromAxisAngle(new Vector3(0.0f, 0.0f, 1.0f).Normalized(), (float) (Math.Atan(Math.Sqrt(2)) + MathHelper.DegreesToRadians(90.0))) *
                                           Quaternion.FromAxisAngle(new Vector3(1.0f, 0.0f, 0.0f).Normalized(), (float) (Math.PI* 0.75));


        public JellySimulation(SceneManager sceneManager, ShaderProvider shaderProvider)
        {
            _sceneManager = sceneManager;
            _shaderProvider = shaderProvider;

            ControlFrame = new Node(new Transform(Vector3.Zero, Quaternion.Identity /* Quaternion.FromEulerAngles(1.0f, 1.0f, 0.0f)*/, Vector3.One), "controlFrame");
            ControlFrameRenderer = new LineRenderer(shaderProvider.DefaultShader);
            var jellyControlFrame = new JellyControlFrame(ControlFrameRenderer, 1.0);
            ControlFrame.AttachComponents(ControlFrameRenderer, jellyControlFrame);

            Jelly = new Node(new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One), "jelly");
            var jellyData = new JellyData();
            var jellyRenderer = new DynamicMeshRenderer(shaderProvider.SurfaceShaderBezier, new Mesh(VertexLayout.Type.Position), jellyData);
            var jellyController = new PlaceholderJellyController();
            
            var boundingBoxRenderer = new LineRenderer(shaderProvider.DefaultShader);
            var jellyBoundingBox = new JellyBoundingBox(boundingBoxRenderer, 5.0);

            WireframeRenderer = new LineRenderer(shaderProvider.DefaultShader);
            var jellyCPVisualizer = new JellyControlPointVisualizer(WireframeRenderer);
            Jelly.AttachComponents(jellyData, jellyRenderer, jellyController, boundingBoxRenderer, jellyBoundingBox, 
                                    WireframeRenderer, jellyCPVisualizer);


            Cube = new Node(new Transform(Vector3.Zero, _baseRotation, Vector3.One * 0.1f), "cube");
            DiagonalRenderer = new LineRenderer(_shaderProvider.DefaultShader) { Color = Color4.Orange };
            var offset = new Vector3(0.5f, 0.5f, 0.5f);
            DiagonalRenderer.SetLine(new Vector3(0.5f, 0.5f, 0.5f) + offset, new Vector3(-0.5f, -0.5f, -0.5f) + offset);
            //var meshRend = new MeshRenderer(provider.PhongShader, MeshUtility.Cube());
            CubeRenderer = new LineRenderer(_shaderProvider.DefaultShader) { Color = Color4.White };
            TrajectoryRenderer = new LineRenderer(_shaderProvider.DefaultWorldShader);
            TrajectoryDrawer = new TrajectoryDrawer(TrajectoryRenderer);
            var data = MeshUtility.WireframeCubeData(_defaultSize, offset);
            CubeRenderer.SetData(data.vertices, data.indices);
            SpinningTopMovement = new SpinningTopMovement(_defaultSize, _defaultDensity, CubeRenderer, DiagonalRenderer);
            Cube.AttachComponents(CubeRenderer, DiagonalRenderer, SpinningTopMovement, TrajectoryRenderer, TrajectoryDrawer);

            //_sceneManager.CurrentScene.AttachChild(Cube);
            _sceneManager.CurrentScene.AttachChild(Jelly);
            _sceneManager.CurrentScene.AttachChild(ControlFrame);
            DiagonalInclination = 10.0f;
        }

        public void Dispose()
        {
            Cube.Dispose();
            Cube = null;
            DiagonalRenderer = null;
            CubeRenderer = null;
        }
    }
}
