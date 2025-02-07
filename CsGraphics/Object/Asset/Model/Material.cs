using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics.Object.Asset.Model
{
    internal class Material
    {
        internal Material(Dictionary<string, (string, byte, ushort, Color, Color, Color)> color)
        {
            Colors = color;
        }

        /// <summary>
        /// Get or sets マテリアルごとの各ポリゴンの(色,テクスチャ名).
        /// Dictionary<string, (string texture, byte illum, ushort specularExponent, Color diffuseColor, Color specularColor, Color ambientColor) >
        /// </summary>
        internal Dictionary<string, (string, byte, ushort, Color, Color, Color)> Colors { get; set; }
    }
}
