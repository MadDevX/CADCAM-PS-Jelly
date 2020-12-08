using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadEngine.Components;

namespace MadEngine
{
    public interface INode : IDisposable
    {
        event Action<INode> OnDisposed;
        string Name { get; set; }
        ITransform Transform { get; }
        IList<IRenderer> Renderers { get; }
        Matrix4 GlobalModelMatrix { get; }
        INode Parent { get; }
        IList<INode> Children { get; }

        void Render(Camera camera, Matrix4 parentMatrix);
        void AttachChild(INode node);
        bool DetachChild(INode node);

        void AttachComponents(params IMComponent[] components);
        void DetachComponents(params IMComponent[] components);
        T GetComponent<T>() where T : IMComponent;
    }
}
