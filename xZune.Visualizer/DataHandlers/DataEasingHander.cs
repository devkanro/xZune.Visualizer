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
            return (float)(data * 0.5 +
                           _cacheList[08][index] * 0.235 +
                           _cacheList[07][index] * 0.11 +
                           _cacheList[06][index] * 0.05 +
                           _cacheList[05][index] * 0.03 +
                           _cacheList[04][index] * 0.025 +
                           _cacheList[03][index] * 0.02 +
                           _cacheList[02][index] * 0.015 +
                           _cacheList[01][index] * 0.01 +
                           _cacheList[00][index] * 0.005);
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
