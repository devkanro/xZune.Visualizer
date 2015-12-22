using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xZune.Visualizer
{
    /// <summary>
    /// Mapping the index of image to sample data, it is a abstract class.
    /// </summary>
    public abstract class SampleDataIndexer
    {
        /// <summary>
        /// Get length of output data with input data length.
        /// </summary>
        /// <param name="inputDataLength"></param>
        /// <returns></returns>
        public virtual int OutputDataLength(int inputDataLength)
        {
            throw new NotImplementedException("OutputDataLength(int inputDataLength) should be overried.");
        }

        /// <summary>
        /// Get index of data with image pixel index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual int GetIndex(int i, AudioVisualizer visualizer)
        {
            throw new NotImplementedException("GetIndex(int i) should be overried.");
        }
    }
}
