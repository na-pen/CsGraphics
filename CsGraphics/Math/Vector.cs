using Microsoft.Maui.Controls.Shapes;
using System;
namespace CsGraphics.Math
{
    public class Vector
    {
        private double[] reference = { 0, 0, 0 };

        private int dimension { get; set; }

        internal Matrix data;
        private int vertex;

        public Vector(int dimension, int vertex, double[]? reference = null)
        {
            if (reference != null)
            {
                this.reference = reference;
            }
            this.dimension = dimension;
            this.vertex = vertex;
            data = new Matrix(4, vertex);
            data.Initialize((i, j) =>
            {
                if (i == dimension)
                {
                    return 1;  // 最終行の要素には1を代入
                }
                else
                {
                    return 0;  // 他の行の要素には0を代入
                }
            });


        }

        public double this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= data.Rows || column < 0 || column >= data.Columns)
                {
                    throw new IndexOutOfRangeException("Invalid index.");
                }

                return data[row, column];
            }

            set
            {
                if (row < 0 || row >= data.Rows || column < 0 || column >= data.Columns)
                {
                    throw new IndexOutOfRangeException("Invalid index.");
                }

                data[row, column] = value;
            }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.dimension != b.dimension || a.vertex != b.vertex)
            {
                throw new ArgumentException("The vectors to be added must have the same number of dimensions and the same number of data");
            }
            Vector result = new(a.dimension, a.vertex)
            {
                data = a.data + b.data,
            };

            return result;
        }

        /*
        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="matrix">移動量</param>
        /// <returns>移動後のベクトル</returns>
        public void Translation(Matrix matrix)
        {
            matrix.Resize(4);
            Matrix temp = new(dimension + 1);
            temp.Identity();
            temp.Resize(4, 4);

            temp[0, dimension] = matrix[0, 0];
            temp[1, dimension] = matrix[1, 0];
            temp[2, dimension] = matrix[2, 0];

            data = temp * data;
        }

        /// <summary>
        /// 拡大・縮小
        /// </summary>
        /// <param name="matrix">拡大・縮小後のベクトル</param>
        public void Scale(Matrix matrix)
        {
            matrix.Resize(4);
            Matrix temp = new(dimension + 1);
            temp.Identity();
            temp.Resize(4, 4);

            for (int i = 0; i < dimension; i++)
            {
                temp[i, i] = matrix[i, 0];
            }

            data = temp * data;
        }
        */
        public void RotationZdeg(double deg)
        {
            RotationZ(float.DegreesToRadians((float)deg));
        }

        public void RotationZ(double rad)
        {
            Matrix temp = new(dimension + 1);
            temp.Identity();
            temp.Resize(4, 4);

            temp[0, 0] = System.Math.Cos(rad);
            temp[0, 1] = -1 * System.Math.Sin(rad);
            temp[1, 0] = System.Math.Sin(rad);
            temp[1, 1] = System.Math.Cos(rad);
            string t = temp.ToString();
            data = temp * data;
        }

        /// <summary>
        /// 座標を代入する(最終行は代入しない)
        /// </summary>
        /// <param name="vertices">座標</param>
        /// <exception cref="ArgumentException">The vertex is incorrectly defined.</exception>
        public new void Initialize(double[,] vertices)
        {
            if (vertices.GetLength(0) != dimension || vertices.GetLength(1) != data.Columns)
            {
                throw new ArgumentException("The vertex is incorrectly defined.");
            }

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < data.Columns; j++)
                {
                    data[i, j] = vertices[i, j];
                }
            }
        }

        public override string ToString()
        {
            return data.ToString();
        }

    }
}
