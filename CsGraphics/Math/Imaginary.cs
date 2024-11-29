namespace CsGraphics.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 虚数の定義.
    /// </summary>
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
        /// 加算記号を利用して虚数の和を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の和</returns>
        public static Imaginary operator +(Imaginary a, Imaginary b)
        {
            Imaginary result = a.value + b.value;
            return result;
        }

        /// <summary>
        /// 加算記号を利用して虚数と実数の和を計算できるようにする設定
        /// </summary>
        /// <param name="a">虚数</param>
        /// <param name="b">実数</param>
        /// <returns>虚数と実数の和</returns>
        public static Complex operator +(Imaginary a, double b)
        {
            Complex result = (b, a);
            return result;
        }

        /// <summary>
        /// 加算記号を利用して虚数と実数の和を計算できるようにする設定
        /// </summary>
        /// <param name="a">実数</param>
        /// <param name="b">虚数</param>
        /// <returns>実数と虚数の和</returns>
        public static Complex operator +(double a, Imaginary b)
        {
            return b + a; // 順序を統一して処理
        }

        /// <summary>
        /// 減算記号を利用して虚数の差を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の差</returns>
        public static Imaginary operator -(Imaginary a, Imaginary b)
        {
            Imaginary result = a.value - b.value;
            return result;
        }

        /// <summary>
        /// 減算記号を利用して虚数と実数の差を計算できるようにする設定
        /// </summary>
        /// <param name="a">虚数</param>
        /// <param name="b">実数</param>
        /// <returns>虚数と実数の差</returns>
        public static Complex operator -(Imaginary a, double b)
        {
            Complex result = (-1 * b, a);
            return result;
        }

        /// <summary>
        /// 減算記号を利用して虚数と実数の差を計算できるようにする設定
        /// </summary>
        /// <param name="a">実数</param>
        /// <param name="b">虚数</param>
        /// <returns>虚数と実数の差</returns>
        public static Complex operator -(double a, Imaginary b)
        {
            return b + a; // 順序を統一して処理
        }

        /// <summary>
        /// 乗算記号を利用して虚数の積を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の積</returns>
        public static double operator *(Imaginary a, Imaginary b)
        {
            double result = -1 * a.value * b.value;
            return result;
        }

        /// <summary>
        /// 乗算記号を利用して虚数と実数の積を計算できるようにする設定
        /// </summary>
        /// <param name="a">虚数</param>
        /// <param name="b">実数</param>
        /// <returns>虚数と実数の積</returns>
        public static Imaginary operator *(Imaginary a, double b)
        {
            Imaginary result = new Imaginary();
            result.value = a.value * b;
            return result;
        }

        /// <summary>
        /// 乗算記号を利用して虚数と実数の積を計算できるようにする設定
        /// </summary>
        /// <param name="a">実数</param>
        /// <param name="b">虚数</param>
        /// <returns>虚数と実数の積</returns>
        public static Imaginary operator *(double a, Imaginary b)
        {
            return b * a; // 順序を統一して処理
        }

        /// <summary>
        /// 除算記号を利用して虚数の商を計算できるようにする設定
        /// </summary>
        /// <param name="a">１つ目の虚数</param>
        /// <param name="b">２つ目の虚数</param>
        /// <returns>２つの虚数の積</returns>
        public static double operator /(Imaginary a, Imaginary b)
        {
            double result = -1 * a.value / b.value;
            return result;
        }

        /// <summary>
        /// 除算記号を利用して虚数と実数の商を計算できるようにする設定
        /// </summary>
        /// <param name="a">虚数</param>
        /// <param name="b">実数</param>
        /// <returns>虚数と実数の商</returns>
        public static Imaginary operator /(Imaginary a, double b)
        {
            Imaginary result = new Imaginary();
            result.value = a.value / b;
            return result;
        }

        /// <summary>
        /// 除算記号を利用して虚数と実数の商を計算できるようにする設定
        /// </summary>
        /// <param name="a">実数</param>
        /// <param name="b">虚数</param>
        /// <returns>虚数と実数の商</returns>
        public static Imaginary operator /(double a, Imaginary b)
        {
            return b / a; // 順序を統一して処理
        }

        /// <summary>
        /// 虚数を文字列に変換する
        /// </summary>
        /// <returns>虚数(String)</returns>
        public override string ToString()
        {
            string result = value.ToString() + "i";
            return result;
        }
    }
}
