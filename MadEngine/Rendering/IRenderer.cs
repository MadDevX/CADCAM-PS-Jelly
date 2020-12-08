﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public interface IRenderer : IDisposable
    {
        void Render(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix);
    }
}
