namespace CsGraphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// IEnumeratorとかの設定.
    /// </summary>
    internal static class DataExtensions
    {
        /// <summary>
        /// Foreachを使うためのGetEnumeratorの実装.
        /// </summary>
        /// <param name="vertex">頂点情報.</param>
        /// <returns>System.Collections.IEnumerator.</returns>
        public static System.Collections.IEnumerator GetEnumerator(this Object.Vertex vertex)
            =>
            Enumerable.Range(0, vertex.GetLength(1))
            .Select(i =>
                new Math.Matrix(new double[] { vertex.Coordinate[0, i], vertex.Coordinate[1, i], vertex.Coordinate[2, i], vertex.Coordinate[3, i] }))
            .GetEnumerator();

        /// <summary>
        /// Foreachを使うためのGetEnumeratorの実装.
        /// </summary>
        /// <param name="matrix">4次元の行列情報.</param>
        /// <returns>System.Collections.IEnumerator.</returns>
        public static System.Collections.IEnumerator GetEnumerator(this Math.Matrix matrix)
            =>
            Enumerable.Range(0, matrix.GetLength(1))
            .Select(i =>
                new Math.Matrix(new double[] { matrix[0, i], matrix[1, i], matrix[2, i], matrix[3, i] }))
            .GetEnumerator();
    }
}
