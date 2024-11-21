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
        public static implicit operator Complex((double,Imaginary) val)
        {
            return new Complex { real = val.Item1, imaginary = val.Item2 };
        }

        /// <summary>
        /// 加算記号を利用して複素数の和を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の複素数</param>
        /// <param name="b">２つ目の複素数</param>
        /// <returns>２つの複素数の和</returns>
        public static Complex operator +(Complex a, Complex b)
        {
            Complex result = (a.real + b.real,a.imaginary + b.imaginary);
            return result;
        }

        /// <summary>
        /// 減算記号を利用して複素数の差を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の複素数</param>
        /// <param name="b">２つ目の複素数</param>
        /// <returns>２つの複素数の差</returns>
        public static Complex operator -(Complex a, Complex b)
        {
            Complex result = (a.real + b.real, a.imaginary + b.imaginary);
            return result;
        }

        /// <summary>
        /// 乗算記号を利用して複素数の積を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の複素数</param>
        /// <param name="b">２つ目の複素数</param>
        /// <returns>２つの複素数の積</returns>
        public static Complex operator *(Complex a, Complex b)
        {
            double _real = (a.real * b.real) - (a.imaginary * b.imaginary);
            Imaginary _imaginary = (a.imaginary * b.real) + (a.real * b.imaginary);
            Complex result = (_real,_imaginary);
            return result;
        }

        /// <summary>
        /// 除算記号を利用して複素数の商を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の複素数</param>
        /// <param name="b">２つ目の複素数</param>
        /// <returns>２つの複素数の商</returns>
        public static Complex operator /(Complex a, Complex b)
        {
            double _real = ((a.real * b.real) + (a.imaginary * b.imaginary)) / ((b.real * b.real) + (b.imaginary * b.imaginary));
            Imaginary _imaginary = ((a.imaginary * b.real) - (a.real * b.imaginary)) / ((b.real * b.real) + (b.imaginary * b.imaginary));

            Complex result = (_real, _imaginary);
            return result;
        }
    }
}
