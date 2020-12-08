using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Architecture
{
    class TextureProvider
    {
        public static TextureProvider Instance { get; private set; }
        public Texture HeightMap { get; }
        public Texture DiffuseMap { get; }
        public Texture NormalMap { get; }
        public TextureProvider()
        {
            HeightMap = new Texture(TexturePaths.HeightPath);
            DiffuseMap = new Texture(TexturePaths.DiffusePath);
            NormalMap = new Texture(TexturePaths.NormalPath);
            Instance = this;
        }
    }
}
