using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xZune.Visualizer.DataHandlers
{
    /// <summary>
    /// Converter the sample data form BASS FFT data, we will strengthen intermediate frequency data and low frequency data.
    /// </summary>
    public class DataConverter : SampleDataHandler
    {
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
                // Parallel convert data.
                
                Parallel.For(0, data.Length, i =>
                {
                    if (i < data.Length / 8)
                    {
                        data[i] = Convert(data[i])(25)(90);
                    }
                    else if (i < data.Length / 4)
                    {
                        data[i] = Convert(data[i])(40)(130);
                    }
                    else if (i < data.Length / 2)
                    {
                        data[i] = Convert(data[i])(22)(80);
                    }
                    else
                    {
                        data[i] = Convert(data[i])(20)(75);
                    }
                    data[i] = data[i] < 0 ? 0 : data[i];
                });
                return data;
            });
        }

        /// <summary>
        /// Convert a data, use it by Convert(data)(sensitivity)(compensation)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Func<float,Func<float, float>> Convert(float data)
        {
            // result = log10(data) * sensitivity + compensation;
            return s => c => (float)(Math.Log10(data) * s + c);
        }
    }
}
