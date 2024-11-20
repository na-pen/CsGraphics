namespace CsGraphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Imaginary
    {
        private double value = 0;

        /// <summary>
        /// Imaginary im = 0 の形で代入できるようにする
        /// </summary>
        /// <param name="val">虚数</param>
        public static implicit operator Imaginary(double val)
        {
            return new Imaginary { value = val };
        }

        /// <summary>
        /// 乗算記号を利用して虚数の積を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の積</returns>
        public static double operator *(Imaginary a, Imaginary b)
        {
            double result = -1* a.value * b.value;
            return result;
        }

        /// <summary>
        /// 加算記号を利用して虚数の和を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の和</returns>
        public static double operator +(Imaginary a, Imaginary b)
        {
            double result = a.value + b.value;
            return result;
        }

        /// <summary>
        /// 減算記号を利用して虚数の差を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の差</returns>
        public static double operator -(Imaginary a, Imaginary b)
        {
            double result = a.value - b.value;
            return result;
        }

        /// <summary>
        /// 除算記号を利用して虚数の積を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の積</returns>
        public static double operator /(Imaginary a, Imaginary b)
        {
            double result = -1 * a.value / b.value;
            return result;
        }
    }
}
