using System;
using System.Threading.Tasks;

namespace xZune.Visualizer
{
    /// <summary>
    /// Audio sample data handler, it is a abstract class.
    /// </summary>
    public abstract class SampleDataHandler
    {
        /// <summary>
        /// Handle the sample data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual Task<float[]> HandleData(float[] data, AudioVisualizer visualizer)
        {
            throw new NotImplementedException("HandleData(float[] data) should be overried.");
        }

        /// <summary>
        /// Get length of output data with input data length.
        /// </summary>
        /// <param name="inputDataLength"></param>
        /// <returns></returns>
        public virtual int OutputDataLength(int inputDataLength)
        {
            throw new NotImplementedException("OutputDataLength(int inputDataLength) should be overried.");
        }
    }
}