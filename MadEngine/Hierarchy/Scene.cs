using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MadEngine.Components;

namespace MadEngine
{
    public class Scene : INode
    {
        public event Action<INode> OnDisposed;

        public string Name { get; set; }
        /// <summary>
        /// Add and remove nodes by dedicated methods (AddNode and DeleteNode)
        /// </summary>
        public IList<INode> Children { get; } = new List<INode>();

        public Matrix4 GlobalModelMatrix
        {
            get
            {
                return Matrix4.Identity;
            }
        }

        public ITransform Transform { get; } = new DummyTransform();

        public IList<IRenderer> Renderers { get; } = new List<IRenderer> { new DummyRenderer() };

        public INode Parent => null;

        private IList<IMComponent> _components { get; } = new List<IMComponent>();

        public Scene(string name)
        {
            Name = name;
            Transform.Node = this;
        }

        public void Dispose()
        {
            foreach (var comp in _components)
            {
                comp.Dispose();
            }
            while (Children.Count > 0)
            {
                Children[Children.Count - 1].Dispose();
            }
            Children.Clear();
            OnDisposed?.Invoke(this);
        }

        public void Render(Camera camera)
        {
            foreach (var renderer in Renderers)
            {
                renderer.Render(camera, Transform.LocalModelMatrix, Matrix4.Identity);
            }
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, Matrix4.Identity);
            }
        }

        public void AttachChild(INode child)
        {
            if (child.Transform.ParentNode != null) throw new InvalidOperationException("Tried to attach node that has another parent");


            child.Transform.ParentNode = this;
            Children.Add(child);
            child.OnDisposed += HandleChildDisposed;
        }

        public bool DetachChild(INode child)
        {

            var val = Children.Remove(child);
            if (val)
            {
                child.Transform.ParentNode = null;
                child.OnDisposed -= HandleChildDisposed;
            }
            return val;
        }

        public void AttachComponents(params IMComponent[] components)
        {
            foreach (var component in components)
            {
                component.SetOwnerNode(this);
                _components.Add(component);
                if (component is IRenderer rend)
                {
                    Renderers.Add(rend);
                }
            }
            foreach (var component in _components)
            {
                component.Initialize();
            }
        }

        public void DetachComponents(params IMComponent[] components)
        {
            foreach (var component in components)
            {
                _components.Remove(component);
                if (component is IRenderer rend)
                {
                    Renderers.Remove(rend);
                }
            }
        }

        public T GetComponent<T>() where T : IMComponent
        {
            foreach (var component in _components)
            {
                if (component is T tComponent) return tComponent;
            }

            throw new InvalidOperationException($"Scene {Name} does not have {typeof(T)} Component attached");
        }

        private void HandleChildDisposed(INode node)
        {
            DetachChild(node);
        }

        public void Render(Camera camera, Matrix4 parentMatrix){}
    }
}
