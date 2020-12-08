using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class SceneManager : IDisposable
    {
        public Scene CurrentScene { get; private set; } = null;
        public SceneManager(Scene scene)
        {
            SetScene(scene);
        }

        public void Dispose()
        {
            if (CurrentScene != null) CurrentScene.Dispose();
        }

        /// <summary>
        /// Returns previous scene (that should be disposed)
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public Scene SetScene(Scene scene)
        {
            var curScene = CurrentScene;
            CurrentScene = scene;
            return curScene;
        }

        /// <summary>
        /// Returns previous scene (that should be disposed)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Scene CreateAndSet(string name)
        {
            var newScene = new Scene(name);
            return SetScene(newScene);
        }
    }
}
