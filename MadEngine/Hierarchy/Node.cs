using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using OpenTK;
using MadEngine.Components;

namespace MadEngine
{
    public class Node : INode
    {
        public event Action<INode> OnDisposed;

        public ITransform Transform { get; private set; }
        public IList<IRenderer> Renderers { get; } = new List<IRenderer>();
        private IList<IMComponent> _components { get; } = new List<IMComponent>();

        public string Name { get; set; }

        /// <summary>
        /// Do not modify collection through this property - use dedicated methods (AttachChild, DetachChild)
        /// </summary>
        public IList<INode> Children => Transform.Children;
        public INode Parent => Transform.ParentNode;

        public Matrix4 GlobalModelMatrix
        {
            get
            {
                return Transform.LocalModelMatrix * Transform.ParentNode.GlobalModelMatrix;
            }
        }


        public Node(ITransform transform, string name)
        {
            Transform = transform;
            Transform.Node = this;
            Name = name;
        }

        /// <summary>
        /// Frees resources and detaches itself from parent node.
        /// </summary>
        public virtual void Dispose()
        {
            foreach(var comp in _components)
            {
                comp.Dispose();
            }
            _components.Clear();
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Children[i].Dispose();
            }
            Children.Clear();
            OnDisposed?.Invoke(this);
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            foreach (var renderer in Renderers)
            {
                renderer.Render(camera, Transform.LocalModelMatrix, parentMatrix);
            }
            var modelMat = Transform.LocalModelMatrix * parentMatrix;
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, modelMat);
            }
        }

        /// <summary>
        /// Attaches child to this node
        /// </summary>
        /// <param name="child"></param>
        public void AttachChild(INode child)
        {
            if (child.Transform.ParentNode != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            child.Transform.ParentNode = this;
            Children.Add(child);
            child.OnDisposed += HandleChildDisposed;
        }

        /// <summary>
        /// Detaches child from this node
        /// </summary>
        /// <param name="child"></param>
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
            foreach(var component in components)
            { 
                component.SetOwnerNode(this);
                _components.Add(component);
                if(component is IRenderer rend)
                {
                    Renderers.Add(rend);
                }
            }
            foreach(var component in _components)
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

        public T GetComponent<T>()/* where T : IMComponent*/
        {
            foreach (var component in _components)
            {
                if (component is T tComponent) return tComponent;
            }

            throw new InvalidOperationException($"Node {Name} does not have {typeof(T)} Component attached");
        }

        private void HandleChildDisposed(INode node)
        {
            DetachChild(node);
        }

        protected void InvokeOnDisposed()
        {
            OnDisposed?.Invoke(this);
        }

    }
}
