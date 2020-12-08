using MadEngine.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public static class Registry
    {
        public static TessellationLevelsManager TessellationLevels { get; } = new TessellationLevelsManager();

        public static SingletonField<IUpdateLoop> UpdateLoop { get; } = new SingletonField<IUpdateLoop>(nameof(UpdateLoop));
        public static SingletonField<IFixedUpdateLoop> FixedUpdateLoop { get; } = new SingletonField<IFixedUpdateLoop>(nameof(FixedUpdateLoop));
    }
}
