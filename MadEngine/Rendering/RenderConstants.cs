﻿using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public static class RenderConstants
    {
        public static float LINE_SIZE = 1.0f;
        public static float POINT_SIZE = 4.0f;
        public static float SELECTED_POINT_SIZE = 5.0f;
        public static float GRID_SIZE = 0.1f;
        public static float POLYGON_SIZE = 0.5f;
        public static float CURVE_SIZE = 1.5f;
        public static float GIZMO_SIZE = 2.0f;
        public static float SURFACE_SIZE = 1.0f;

        public static Color4 PARAMETRIC_OBJECT_DEFAULT_COLOR = Color4.White;
        public static Color4 PARAMETRIC_OBJECT_SELECTED_COLOR = Color4.Yellow;
        public static Color4 POLYGON_DEFAULT_COLOR = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
        public static Color4 POLYGON_SELECTED_COLOR = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
        public static Vector3 LIGHT_POS = new Vector3(0.0f, 3.0f, 0.0f);
        public static Vector3 LIGHT_COL = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
