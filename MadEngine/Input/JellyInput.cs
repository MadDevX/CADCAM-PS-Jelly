using MadEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MadEngine.Input
{
    public enum TransformationType
    {
        None,
        Translation,
        Rotation,
        Scale
    }

    public class JellyInput : IDisposable
    {
        public bool EnableViewPlaneTranslation { get; set; } = true;
        public TransformationType TransformationType { get; private set; } = TransformationType.None;

        public string Axis 
        { 
            get
            {
                var s = "";
                if (Mults.X > 0.0f) s += "X";
                if (Mults.Y > 0.0f) s += "Y";
                if (Mults.Z > 0.0f) s += "Z";
                if (s == "") s = "Any/None";
                return s;
            }
        }

        private readonly INode _jelly;
        private readonly INode _controlFrame;
        private readonly GLControl _control;
        private readonly Camera _camera;
        private float TranslateMultiplier => (_translateSensitivity / _control.Width) * _camera.DistanceToTarget;
        private float RotateMultiplier => (_rotateSensitivity / _control.Width);
        private float ScaleMultiplier => (_scaleSensitivity / _control.Width);

        private Point _prevPos;
        private float _rotateSensitivity = (float)Math.PI * 1.0f;//5.0f;
        private float _translateSensitivity = 2.5f;
        private float _scaleSensitivity = 5.0f;

        private Vector3 _mults = Vector3.Zero;
        private Vector3 Mults
        {
            get => _mults;
            set
            {
                _mults = value;
            }
        }


        private Point _currentInputBuffer = Point.Empty;

        public JellyInput(INode jelly, INode controlFrame, GLControl control, Camera camera)
        {
            _jelly = jelly;
            _controlFrame = controlFrame;
            _control = control;
            _camera = camera;
            Initialize();
        }

        private void Initialize()
        {
            _control.KeyDown += KeyDown;
            _control.MouseMove += MouseMove;
        }

        public void Dispose()
        {
            _control.KeyDown -= KeyDown;
            _control.MouseMove -= MouseMove;
        }

        private void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.KeyCode == System.Windows.Forms.Keys.T) TransformationType = TransformationType.Translation;
            else if (e.KeyCode == System.Windows.Forms.Keys.R) TransformationType = TransformationType.Rotation;
            else if (e.KeyCode == System.Windows.Forms.Keys.S) TransformationType = TransformationType.Scale;
            else if (e.KeyCode == System.Windows.Forms.Keys.Escape) 
            {
                TransformationType = TransformationType.None; 
                Mults = Vector3.Zero;
            }

            if (TransformationType != TransformationType.None)
            {
                if (TransformationType == TransformationType.Scale)
                {
                    if      (e.KeyCode == System.Windows.Forms.Keys.X) Mults = new Vector3(1.0f, Mults.Y, Mults.Z);
                    else if (e.KeyCode == System.Windows.Forms.Keys.Y) Mults = new Vector3(Mults.X, 1.0f, Mults.Z);
                    else if (e.KeyCode == System.Windows.Forms.Keys.Z) Mults = new Vector3(Mults.X, Mults.Y, 1.0f);
                }
                else
                {
                    if      (e.KeyCode == System.Windows.Forms.Keys.X) Mults = Vector3.UnitX;
                    else if (e.KeyCode == System.Windows.Forms.Keys.Y) Mults = Vector3.UnitY;
                    else if (e.KeyCode == System.Windows.Forms.Keys.Z) Mults = Vector3.UnitZ;
                }
            }
        }

        private void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point posDiff = new Point(0, 0);

            if (TransformationType != TransformationType.None)
            {
                posDiff = new Point(e.Location.X - _prevPos.X, e.Location.Y - _prevPos.Y);
                if (Math.Abs(posDiff.X) > 100 || Math.Abs(posDiff.Y) > 100)
                {
                    posDiff = Point.Empty;
                }
            }

            Vector3 diffVector;
            switch (TransformationType)
            {
                case TransformationType.Translation:
                    posDiff = BufferInput(posDiff, TranslateMultiplier);
                    diffVector = new Vector3(Mults.X * posDiff.X, Mults.Y * posDiff.X, Mults.Z * posDiff.X); //only left-right  mouse movement is used as transformation input
                    HandleTranslation(diffVector, posDiff);
                    break;
                case TransformationType.Rotation:
                    posDiff = BufferInput(posDiff, RotateMultiplier);
                    diffVector = new Vector3(Mults.X * posDiff.X, Mults.Y * posDiff.X, Mults.Z * posDiff.X); //only left-right  mouse movement is used as transformation input
                    HandleRotation(diffVector);
                    break;
                case TransformationType.Scale:
                    posDiff = BufferInput(posDiff, ScaleMultiplier);
                    diffVector = new Vector3(Mults.X * posDiff.X, Mults.Y * posDiff.X, Mults.Z * posDiff.X); //only left-right  mouse movement is used as transformation input
                    HandleScale(diffVector, posDiff);
                    break;
            }

            _prevPos = e.Location;
        }

        #region Transformations Processing
        private Point BufferInput(Point posDiff, float TransformationMultiplier)
        {
            return posDiff;
        }

        private void HandleTranslation(Vector3 diffVector, Point posDiff)
        {
            if (Mults.Length > 0.0f)
            {
                diffVector *= TranslateMultiplier;
            }
            else if(EnableViewPlaneTranslation)
            {
                diffVector = _camera.ViewPlaneVectorToWorldSpace(new Vector2(posDiff.X, -posDiff.Y)) * TranslateMultiplier;
            }
            TranslateRaw(_controlFrame, diffVector);
        }

        private void HandleRotation(Vector3 diffVector)
        {
            diffVector *= RotateMultiplier;
            RotateRaw(_controlFrame, diffVector);
        }

        private void HandleScale(Vector3 diffVector, Point posDiff)
        {
            diffVector *= ScaleMultiplier;
            ScaleRaw(_controlFrame, diffVector);
        }

        public static void TranslateRaw(INode node, Vector3 diffVector)
        {
            node.Transform.Translate(diffVector);
        }

        public static void RotateRaw(INode node, Vector3 diffVector)
        {
            node.Transform.RotateAround(node.Transform.Position, diffVector);
        }
        public static void ScaleRaw(INode node, Vector3 diffVector)
        {
            node.Transform.Scale += diffVector;
        }


        #endregion
    }
}
