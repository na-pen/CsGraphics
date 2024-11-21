namespace CsGraphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Complex
    {
        private double real = 0;
        private Imaginary imaginary = 0;

        /// <summary>
        /// Complex cp = (3,4)  //3+4i の形で代入できるようにする
        /// </summary>
        /// <param name="val">複素数</param>
        public static implicit operator Complex((double,double) val)
        {
            return new Complex { real = val.Item1, imaginary = val.Item2 };
        }
    }
}
