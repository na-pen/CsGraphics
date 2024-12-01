namespace CsGraphics.Math
{
    using System;

    /// <summary>
    /// 行列の定義.
    /// </summary>
    internal class Matrix : ICloneable
    {
        private double[,] data; // 行列データ

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// 行列のサイズを指定して初期化.
        /// 列数の指定がなかった場合は正方行列.
        /// </summary>
        /// <param name="rows">行数.</param>
        /// <param name="columns">列数.</param>
        internal Matrix(int rows, int columns = 0)
        {
            if (columns == 0)
            {
                columns = rows;
            }

            if (rows <= 0 || columns <= 0)
            {
                throw new ArgumentException("Rows and columns must be positive integers.");
            }

            this.Rows = rows;
            this.Columns = columns;
            this.data = new double[rows, columns];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// 1行の配列から行列を作成.
        /// </summary>
        /// <param name="array">配列.</param>
        internal Matrix(double[] array)
        {
            double[,] result = new double[array.Length, 1];

            // 1次元配列の各要素を2次元配列にコピー
            for (int i = 0; i < array.Length; i++)
            {
                result[i, 0] = array[i];
            }

            this.Rows = array.Length;
            this.Columns = 1;
            this.data = new double[this.Rows, this.Columns];

            this.Initialize(result);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// 2次元配列から行列を作成.
        /// </summary>
        /// <param name="array">配列.</param>
        internal Matrix(double[,] array)
        {
            this.Rows = array.GetLength(0);
            this.Columns = array.GetLength(1);
            this.data = new double[this.Rows, this.Columns];

            this.Initialize(array);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// Cloneするとき専用.
        /// </summary>
        /// <param name="data">行列のデータ.</param>
        /// <param name="rows">行数.</param>
        /// <param name="columns">列数.</param>
        private Matrix(double[,] data, int rows, int columns)
        {
            this.data = data;
            this.Rows = rows;
            this.Columns = columns;
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// 行列の行数.
        /// </summary>
        internal int Rows { get; private set; }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// 行列の列数.
        /// </summary>
        internal int Columns { get; private set; }

        /// <summary>
        /// 要素をインデックスで取得するための設定.
        /// </summary>
        /// <param name="row">行.</param>
        /// <param name="column">列.</param>
        /// <returns>値.</returns>
        /// <exception cref="IndexOutOfRangeException">インデックスがアクセス範囲外の場合.</exception>
        internal double this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= this.Rows || column < 0 || column >= this.Columns)
                {
                    throw new IndexOutOfRangeException("Invalid index.");
                }

                return this.data[row, column];
            }

            set
            {
                if (row < 0 || row >= this.Rows || column < 0 || column >= this.Columns)
                {
                    throw new IndexOutOfRangeException("Invalid index.");
                }

                this.data[row, column] = value;
            }
        }

        /// <summary>
        /// 加算記号を利用して行列の和を計算できるようにする設定.
        /// </summary>
        /// <param name="a">１つ目の行列.</param>
        /// <param name="b">２つ目の行列.</param>
        /// <returns>2つの行列の和.</returns>
        /// <exception cref="InvalidOperationException">対象の行列の行数と列数はそれぞれ一致する必要があります.</exception>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
            {
                throw new InvalidOperationException("Matrix dimensions must match for addition.");
            }

            Matrix result = new (a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// 加算記号を利用して行列と整数の和を計算できるようにする設定.
        /// </summary>
        /// <param name="matrix">行列.</param>
        /// <param name="value">整数.</param>
        /// <returns>行列と整数の和.</returns>
        public static Matrix operator +(Matrix matrix, double value)
        {
            Matrix result = new (matrix.Rows, matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result[i, j] = matrix[i, j] + value;
                }
            }

            return result;
        }

        /// <summary>
        /// 加算記号を利用して整数と行列の和を計算できるようにする設定.
        /// </summary>
        /// <param name="value">整数.</param>
        /// <param name="matrix">行列.</param>
        /// <returns>行列と整数の和.</returns>
        public static Matrix operator +(double value, Matrix matrix)
        {
            return matrix + value; // 順序を統一して処理
        }

        /// <summary>
        /// 減算記号を利用して行列の差を計算できるようにする設定.
        /// </summary>
        /// <param name="a">１つ目の行列.</param>
        /// <param name="b">２つ目の行列.</param>
        /// <returns>２つの行列の和.</returns>
        /// <exception cref="InvalidOperationException">対象の行列の行数と列数はそれぞれ一致する必要があります.</exception>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
            {
                throw new InvalidOperationException("Matrix dimensions must match for addition.");
            }

            Matrix result = new (a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// 減算記号を利用して行列と整数の差を計算できるようにする設定.
        /// </summary>
        /// <param name="matrix">行列.</param>
        /// <param name="value">整数.</param>
        /// <returns>行列と整数の差.</returns>
        public static Matrix operator -(Matrix matrix, double value)
        {
            Matrix result = new Matrix(matrix.Rows, matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result[i, j] = matrix[i, j] - value;
                }
            }

            return result;
        }

        /// <summary>
        /// 加算記号を利用して整数と行列の差を計算できるようにする設定.
        /// </summary>
        /// <param name="value">整数.</param>
        /// <param name="matrix">行列.</param>
        /// <returns>行列と整数の差.</returns>
        public static Matrix operator -(double value, Matrix matrix)
        {
            return matrix - value; // 順序を統一して処理
        }

        /// <summary>
        /// 乗算記号を利用して行列の積を計算できるようにする設定.
        /// </summary>
        /// <param name="a">１つ目の行列.</param>
        /// <param name="b">２つ目の行列.</param>
        /// <returns>２つの行列の和.</returns>
        /// <exception cref="InvalidOperationException">１つ目行列の行数と２つ目の行列の列数は一致する必要があります.</exception>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Columns != b.Rows)
            {
                throw new InvalidOperationException("Matrix dimensions are not valid for multiplication.");
            }

            Matrix result = new Matrix(a.Rows, b.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < a.Columns; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }

        /// <summary>
        /// 行列の中身を文字列に変換する.
        /// </summary>
        /// <returns>行列(String).</returns>
        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    result += $"{this.data[i, j]:0.##}\t";
                }

                result += "\n";
            }

            return result;
        }

        /// <summary>
        /// Cloneの実装.
        /// </summary>
        /// <returns>ディープコピーされた行列.</returns>
        public object Clone()
        {
            return new Matrix(
                (double[,])this.data.Clone(),
                this.Rows,
                this.Columns);
        }

        /// <summary>
        /// 行列を初期値で埋める.
        /// </summary>
        /// <param name="initializer">initializer.</param>
        internal void Initialize(Func<int, int, double> initializer)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    this.data[i, j] = initializer(i, j);
                }
            }
        }

        /// <summary>
        /// 2次元配列で行列を初期化する.
        /// </summary>
        /// <param name="array">array.</param>
        internal void Initialize(double[,] array)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    this.data[i, j] = array[i, j];
                }
            }
        }

        /// <summary>
        /// 単位行列を生成する.
        /// </summary>
        /// <exception cref="ArgumentException">行列の長さは1以上の整数である必要があります.</exception>
        internal void Identity()
        {
            if (this.Rows != this.Columns)
            {
                throw new IndexOutOfRangeException("The identity matrix must have equal row and column sizes.");
            }

            this.Initialize((i, j) => 0);

            for (int i = 0; i < this.Rows; i++)
            {
                this[i, i] = 1; // 対角成分を1に設定
            }
        }

        /// <summary>
        /// 行列を転置する.
        /// </summary>
        internal void Transpose()
        {
            Matrix transposed = new Matrix(this.Columns, this.Rows); // 行と列を入れ替えたサイズの行列を作成
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    transposed[j, i] = this[i, j]; // 行と列を入れ替えてコピー
                }
            }

            this.data = transposed.data;
            int temp = this.Rows;
            this.Rows = this.Columns;
            this.Columns = temp;
        }

        /// <summary>
        /// 行列を指定したサイズにリサイズする.
        /// </summary>
        /// <param name="row">行数.</param>
        /// <param name="column">列数.</param>
        /// <param name="value">追加する行に代入したい値.</param>
        /// <exception cref="ArgumentException">The number of rows and columns in the resized matrix must be equal to or greater than before the resizing, respectively.</exception>
        internal void Resize(int row, int column = 0, double[]? value = null)
        {
            int temp = -1;
            if (value == null)
            {
                value = new double[row - this.Rows - 1];
                Array.Fill(value, 0, 0, row - this.Rows - 1);
            }

            if (column == 0)
            {
                column = this.Columns;
            }

            if (row < this.Rows || column < this.Columns)
            {
                throw new ArgumentException("The number of rows and columns in the resized matrix must be equal to or greater than before the resizing, respectively.");
            }

            Matrix result = new (row, column);

            // 行列のサイズを変更する（n行m列）
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    if (i < this.Rows && j < this.Columns)
                    {
                        result[i, j] = this.data[i, j];
                    }
                    else
                    {
                        if (temp == -1)
                        {
                            temp = i;
                        }

                        result[i, j] = value[i - temp];  // 拡張部分の値は0
                    }
                }
            }

            this.Columns = result.Columns;
            this.Rows = result.Rows;
            this.data = result.data;
        }

        /// <summary>
        /// 指定した方向の長さを取得する.
        /// </summary>
        /// <param name="dimension">dimension.</param>
        /// <returns>長さ.</returns>
        internal int GetLength(int dimension)
        {
            return this.data.GetLength(dimension);
        }
    }
}
