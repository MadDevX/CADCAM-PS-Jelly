using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public static class ShaderPaths
    {
        public static readonly string SimplePhongVSPath = "simpleVSPhong.vert";
        public static readonly string PhongVSPath = "vsPhong.vert";
        public static readonly string PhongFSPath = "fsPhong.frag";
        public static readonly string PhongTexturedFSPath = "fsPhongTextured.frag";
        public static readonly string SimpleVSPath = "simpleVs.vert";
        public static readonly string SimpleFSPath = "simpleFs.frag";
        public static readonly string VSPath = "vs.vert";
        public static readonly string VSWorldPath = "vsWorld.vert";
        public static readonly string FSPath = "fs.frag";
        public static readonly string VSQuadPath = "vsQuad.vert";
        public static readonly string FSQuadPath = "fsQuad.frag";
        public static readonly string TESCQuadPath = "tcQuad.tesc";
        public static readonly string TESEQuadPath = "teQuad.tese";
        public static readonly string TESCBezierPath = "tcBezier.tesc";
        public static readonly string TESCBezierDynamicLODPath = "tcBezierDynamicLOD.tesc";
        public static readonly string TESEBezierPath = "teBezier.tese";
        public static readonly string TESEBezierTexturedPath = "teBezierTextured.tese";
    }
}
