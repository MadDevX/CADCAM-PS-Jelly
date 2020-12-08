using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public interface IUpdateLoop
    {
        float TimeScale { get; set; }
        event Action<float> OnUpdate;
    }
    public interface IRenderLoop
    {
        event Action OnRenderLoop;
    }
    public interface IFixedUpdateLoop
    {
        float FixedDeltaTime { get; set; }
        event Action<float> OnFixedUpdate;
    }
}
