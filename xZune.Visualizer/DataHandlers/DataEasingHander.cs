using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xZune.Visualizer.DataHandlers
{
    /// <summary>
    /// Make the sample data easing with time, it will fix sometime visualizer fast flicker.
    /// </summary>
    public class DataEasingHander : SampleDataHandler
    {
        private List<float[]> _cacheList = new List<float[]>(9);

        /// <summary>
        /// Add easing data to cacheList.
        /// </summary>
        /// <param name="data"></param>
        private void AddCache(float[] data)
        {
            for (int i = 0; i < 8; i++)
            {
                _cacheList[i + 0] = _cacheList[i + 1];
            }
            _cacheList[8] = data;
        }

        private float GetResult(int index, float data)
        {
            return (float)(data * 0.3 +
                               _cacheList[8][index] * 0.2 +
                               _cacheList[7][index] * 0.15 +
                               _cacheList[6][index] * 0.12 +
                               _cacheList[5][index] * 0.08 +
                               _cacheList[4][index] * 0.065 +
                               _cacheList[3][index] * 0.035 +
                               _cacheList[2][index] * 0.025 +
                               _cacheList[1][index] * 0.015 +
                               _cacheList[0][index] * 0.01);
        }

        /// <summary>
        /// Get length of output data with input data length, we will not change this.
        /// </summary>
        /// <param name="inputDataLength"></param>
        /// <returns></returns>
        public override int OutputDataLength(int inputDataLength)
        {
            return inputDataLength;
        }

        /// <summary>
        /// Convert the sample data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Task<float[]> HandleData(float[] data, AudioVisualizer visualizer)
        {
            return Task.Run(() =>
            {
                while (_cacheList.Count < 9)
                {
                    _cacheList.Add(new float[data.Length]);
                }

                var result = new float[data.Length];

                // Parallel convert data.
                Parallel.For(0, data.Length, i =>
                {
                    result[i] = GetResult(i, data[i]);
                });

                AddCache(result);
                return result;
            });
        }
    }
}
