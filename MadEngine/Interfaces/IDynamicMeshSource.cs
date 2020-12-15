using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Rendering
{
    public interface IDynamicMeshSource
    {
        void Refresh(Mesh mesh);
    }
}
