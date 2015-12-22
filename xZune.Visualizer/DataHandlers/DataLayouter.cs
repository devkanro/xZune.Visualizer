using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace xZune.Visualizer.DataHandlers
{
    /// <summary>
    /// Layout the sample data, make left channel data at left.
    /// </summary>
    public class DataLayouter : SampleDataIndexer
    {
        /// <summary>
        /// Get length of output data with input data length, we lost 1/4 of data.
        /// </summary>
        /// <param name="inputDataLength"></param>
        /// <returns></returns>
        public override int OutputDataLength(int inputDataLength)
        {
            if (inputDataLength % 4 != 0)
            {
                throw new NotImplementedException("DataLayouter need input data length is multiple of 2.");
            }
            else
            {
                return inputDataLength / 4 * 3;
            }
        }

        /// <summary>
        /// Mapping layout the sample data.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override int GetIndex(int i, AudioVisualizer visualizer)
        {
            // The format of sample data is
            // LRLRLRLRLR.........LRLRLRLRLR (L is left channel, R is right channel)
            // ^- Lower          Heighter -^
            //
            // We should map it to
            // LLLLLLLLLL.........RRRRRRRRRR
            //           ^-Lower-^
            // ^----------Heighter---------^

            int result = 0;

            if (i < visualizer.OutputDataLenght / 2)
            {
                // Left channel.
                result = visualizer.Data.Length - (i + 1) * 2;

                if (i >= visualizer.OutputDataLenght / 3)
                {
                    if (i % 2 == 0)
                    {
                        int tmp = visualizer.OutputDataLenght/2 - i;
                        if (visualizer.Data[result] < visualizer.Data[tmp])
                        {
                            result = tmp;
                        }
                    }
                    else
                    {
                        int tmp = visualizer.OutputDataLenght / 2 - i - 1 + visualizer.Data.Length / 8;
                        if (visualizer.Data[result] < visualizer.Data[tmp])
                        {
                            result = tmp;
                        }
                    }
                }

                return result;
            }
            else
            {
                // Right channel.
                result = (i - visualizer.OutputDataLenght / 2)* 2 + 1 + visualizer.Data.Length / 4;

                if (i < visualizer.OutputDataLenght / 3 * 2)
                {
                    if (i % 2 == 0)
                    {
                        int tmp = i - visualizer.OutputDataLenght / 2 + 1;
                        if (visualizer.Data[result] < visualizer.Data[tmp])
                        {
                            result = tmp;
                        }
                    }
                    else
                    {
                        int tmp = i - visualizer.OutputDataLenght / 2 + 1 + visualizer.Data.Length / 8;
                        if (visualizer.Data[result] < visualizer.Data[tmp])
                        {
                            result = tmp;
                        }
                    }
                }

                return result;
            }
        }
    }
}
