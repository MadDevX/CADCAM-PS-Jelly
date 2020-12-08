using MadEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MadEngine.Components
{
    public abstract class UpdatableMComponent : MComponent
    {
        private IUpdateLoop _loop;
        private IFixedUpdateLoop _fixedLoop;

        public UpdatableMComponent()
        {
            _loop = Registry.UpdateLoop.I;
            _fixedLoop = Registry.FixedUpdateLoop.I;
        }

        public override void Initialize()
        {
            _loop.OnUpdate += OnUpdateWrapper;
            _fixedLoop.OnFixedUpdate += OnFixedUpdateWrapper;
            base.Initialize();
        }

        public override void Dispose()
        {
            _fixedLoop.OnFixedUpdate -= OnFixedUpdateWrapper;
            _loop.OnUpdate -= OnUpdateWrapper;
            base.Dispose();
        }

        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnFixedUpdate(float deltaTime) { }

        private void OnUpdateWrapper(float deltaTime)
        {
            //TODO: this is left here because (probably) if Node initialization is costly, 
            //there may be a situation where Stopwatch calls OnTick before object 
            //initialization was completed (because it runs on a different thread)
            if (_initialized == false) MessageBox.Show("Update Loop tried to execute before component initialization finished", 
                                                       "Whoopsie Daisy!", MessageBoxButton.OK, MessageBoxImage.Warning);
            if(Enabled && _initialized)
            {
                OnUpdate(deltaTime);
            }
        }

        private void OnFixedUpdateWrapper(float deltaTime)
        {
            //TODO: Same as above
            if (_initialized == false) MessageBox.Show("Update Loop tried to execute before component initialization finished", 
                                                       "Whoopsie Daisy!", MessageBoxButton.OK, MessageBoxImage.Warning);
            if(Enabled && _initialized)
            {
                OnFixedUpdate(deltaTime);
            }
        }
    }
}
