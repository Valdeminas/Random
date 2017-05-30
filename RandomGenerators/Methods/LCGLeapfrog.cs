using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Random.Methods
{
    class LCGLeapfrog
    {
        public ulong a = 16807;
        public ulong m = (ulong)Math.Pow(2, 31) - 1;
        public ulong c = 0;


        public ulong N { get { return _n; } }
        private ulong _n;

        public int CalcCount { get { return _CalcCount; } }
        private int _CalcCount;

        public ulong[] Result { get { return _Result; } }
        private ulong[] _Result;

        public double[] Output { get { return _Output; } }
        private double[] _Output;

        private ManualResetEvent _doneEvent;

        public LCGLeapfrog(ulong n, int calcCount, int threadcount, ManualResetEvent doneEvent)
        {
            _n = n;
            _CalcCount = calcCount;
            _Result = new ulong[_CalcCount + 1];
            _Output = new double[_CalcCount];
            _doneEvent = doneEvent;
            a = powermod(a, threadcount,m);
        }

        public LCGLeapfrog(ulong n, int threads, int threadload)
        {
            _n = n;
            _CalcCount = threads;
            _Result = new ulong[_CalcCount + 1];
            _Output = new double[_CalcCount];
            a = powermod(a, threadload, m);
        }

        public LCGLeapfrog(ulong n, int calcCount)
        {
            _n = n;
            _CalcCount = calcCount;
            _Result = new ulong[_CalcCount + 1];
            _Output = new double[_CalcCount];
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...", threadIndex);
            _Result[0] = _n;
            for (int i = 1; i < _CalcCount + 1; i++)
            {
                _Result[i] = Calculate(_Result[i - 1]);
                _Output[i - 1] = (double)_Result[i] / (double)m;
            }
            Console.WriteLine("thread {0} result calculated...", threadIndex);
            _doneEvent.Set();
        }
        public void Calculateinitial(int count)
        {
            _Result[0] = _n;
            for (int i = 1; i < count + 1; i++)
            {
                _Result[i] = Calculateinitial(_Result[i - 1]);
                _Output[i - 1] = (double)_Result[i] / (double)m;
            }
        }


        // Recursive method that calculates the Nth Fibonacci number.
        public ulong Calculateinitial(ulong n)
        {

            ulong temp = (a * n + c) % m;
            if (temp < 0)
            {
                return temp + n;
            }
            else
            {
                return temp;
            }
        }

        public ulong Calculate(ulong n)
        {

            ulong temp = (a * n + c) % m;
            if (temp < 0)
            {
                return temp + n;
            }
            else
            {
                return temp;
            }
        }

        public ulong powermod(ulong baseNo, int exponent,ulong modulus)
        {
            if (baseNo < 1 || exponent < 0 || modulus < 1)
                throw new System.ArgumentException("Parameter cannot be null", "original");

            ulong result = 1;
            while (exponent > 0)
            {
                if ((exponent % 2) == 1)
                {
                    result = (result * baseNo) % modulus;
                }
                baseNo = (baseNo * baseNo) % modulus;
                exponent = (int)Math.Floor((double)exponent / 2);
            }
            return result;
        }

    }
}
