namespace CsGraphics.Object.Camera
{
    using CsGraphics.Math;

    internal class Camera : Object
    {
        public Camera(string name)
            : base(name)
        {
            this.ViewCamTranslation.Identity();

            this.ViewCamRotation.Identity();

        }

        private float _fovY = 3.14f;
        internal float FovY
        {
            get { return _fovY; }
            set
            {
                _fovY = float.DegreesToRadians(value);
                CalcConvertMatrix();
            }
        }

        internal bool Mode = true;

        private float _width = 1;
        private float _height = 1;
        private float[] _vertexCoordinate = { 0, 0, 0, 0 }; // {left, right, top, bottom}
        private float _aspect = 1;

        internal float Width
        {
            get { return this._width; }
            set
            {
                if (value > 0)
                {
                    this._width = value;
                    _vertexCoordinate[0] = -_width / 2;
                    _vertexCoordinate[1] = _width / 2;
                    _aspect = _width / _height;
                    CalcConvertMatrix();
                }
                else
                {
                    throw new ArgumentException("Widthは0より大きい数値である必要があります");
                }
            }
        }

        internal float Height
        {
            get { return this._height; }
            set
            {
                if (value > 0)
                {
                    this._height = value;
                    _vertexCoordinate[3] = -_height / 2;
                    _vertexCoordinate[4] = _height / 2;
                    _aspect = _width / _height;
                    CalcConvertMatrix();
                }
                else
                {
                    throw new ArgumentException("Heightは0より大きい数値である必要があります");
                }
            }
        }

        internal Matrix cam2view = new Matrix(4);

        internal Math.Matrix ViewCamTranslation { get; set; } = new Matrix(4);

        internal Math.Matrix ViewCamRotation { get; set; } = new Matrix(4);

        private float[] camRotate = new float[3] { 0, 0, 0 };

        private const float scaleParallelProjection = 32;
        private int _far = -5000;
        private float _near = -1f;

        internal int Far
        {
            get { return -this._far; }
            set
            {
                if (value > 0 && value > _near)
                {
                    this._far = -value;
                    CalcConvertMatrix();
                }
                else
                {
                    throw new ArgumentException("Farは0より大きいかつNearより大きい必要があります");
                }
            }
        }
        internal float Near
        {
            get { return -this._near; }
            set
            {
                if (value > 0 && value < _far)
                {
                    this._near = -value;
                    CalcConvertMatrix();
                }
                else
                {
                    throw new ArgumentException("Nearは0より大きいかつFarより小さい必要があります");
                }
            }
        }

        protected override void RotationChanged()
        {
            IsUpdated = true;
            Matrix xAxis = new(4);
            xAxis.Identity();
            xAxis[1, 1] = System.MathF.Cos(Angle[0] * System.MathF.PI / 180f);
            xAxis[2, 1] = System.MathF.Sin(Angle[0] * System.MathF.PI / 180f);
            xAxis[1, 2] = -1 * System.MathF.Sin(Angle[0] * System.MathF.PI / 180f);
            xAxis[2, 2] = System.MathF.Cos(Angle[0] * System.MathF.PI / 180f);

            Matrix yAxis = new(4);
            yAxis.Identity();
            yAxis[0, 0] = System.MathF.Cos(Angle[1] * System.MathF.PI / 180f);
            yAxis[2, 0] = -1 * System.MathF.Sin(Angle[1] * System.MathF.PI / 180f);
            yAxis[0, 2] = System.MathF.Sin(Angle[1] * System.MathF.PI / 180f);
            yAxis[2, 2] = System.MathF.Cos(Angle[1] * System.MathF.PI / 180f);

            Matrix zAxis = new(4);
            zAxis.Identity();
            zAxis[0, 0] = System.MathF.Cos(Angle[2] * System.MathF.PI / 180f);
            zAxis[0, 1] = -1 * System.MathF.Sin(Angle[2] * System.MathF.PI / 180f);
            zAxis[1, 0] = System.MathF.Sin(Angle[2] * System.MathF.PI / 180f);
            zAxis[1, 1] = System.MathF.Cos(Angle[2] * System.MathF.PI / 180f);

            this.ViewCamRotation = yAxis * xAxis * zAxis;
        }

        protected override void TranslationChanged()
        {
            IsUpdated = true;

            this.ViewCamTranslation[0, 3] += Origin[0, 0];
            this.ViewCamTranslation[1, 3] += Origin[1, 0];
            this.ViewCamTranslation[2, 3] += Origin[2, 0];
        }

        private void CalcConvertMatrix()
        {
            if (Mode) // 透視投影のとき
            {
                float f = (float)(1f / System.MathF.Tan(_fovY / 2f));
                cam2view = new(new float[,]
                {
                    { -f / _aspect, 0, 0, 0 },
                    { 0, f, 0, 0 },
                    { 0, 0, -1f * (_far + _near) / (_far - _near), -2f * (_far * _near) / (_far - _near) },
                    { 0, 0, -1f, 0 },
                });
            }
            else // 平行投影のとき
            {
                _vertexCoordinate[0] = -_width / scaleParallelProjection;
                _vertexCoordinate[1] = _width / scaleParallelProjection;
                _vertexCoordinate[3] = _height / scaleParallelProjection;
                _vertexCoordinate[2] = -_height / scaleParallelProjection;

                cam2view = new(new float[,]
                {
                    { 2f / (_vertexCoordinate[1] - _vertexCoordinate[0]), 0, 0, -(_vertexCoordinate[1] + _vertexCoordinate[0]) / (_vertexCoordinate[1] - _vertexCoordinate[0]) },
                    { 0, 2f / (_vertexCoordinate[2] - _vertexCoordinate[3]), 0, -(_vertexCoordinate[2] + _vertexCoordinate[3]) / (_vertexCoordinate[2] - _vertexCoordinate[3]) },
                    { 0, 0, -2f / (_far - _near), -(_far + _near) / (_far - _near)},
                    { 0, 0, 0, 1 },
                });
            }
        }
    }
}
