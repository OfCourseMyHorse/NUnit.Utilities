using System;
using System.Collections.Generic;
using System.Text;

namespace TestImages
{
    struct _VarianceAccumulator
    {
        private ulong _Count;
        private double _Sum;
        private double _Value;

        public void Clear()
        {
            _Count = 0;
            _Sum = 0;
            _Value = 0;
        }

        public void AddSample(double sample)
        {
            if (_Count == 0)
            {
                _Count++;
                _Sum = sample;
            }
            else
            {
                _Count++;
                _Sum += sample;

                double x = (double)_Count * sample - _Sum;
                _Value += x * x / (double)(_Count * (_Count - 1));
            }
        }

        public double Variance
        {
            get
            {
                if (_Count <= 1) return double.NaN;
                return _Value / (double)(_Count - 1);
            }
        }

        public double StandardDeviation => Math.Sqrt(Variance);

    }
}
