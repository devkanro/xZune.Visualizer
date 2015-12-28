using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xZune.Visualizer.DataHandlers;

namespace xZune.Visualizer
{
    /// <summary>
    /// Zune style audio visualizer, support BASS's FFT sample data.
    /// </summary>
    public class AudioVisualizer : Control
    {
        public AudioVisualizer()
        {
            DataHandlers.Add(new DataConverter());
            DataHandlers.Add(new DataEasingHander());

            DataIndexers.Add(new DataLayouter());
        }

        static AudioVisualizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AudioVisualizer), new FrameworkPropertyMetadata(typeof(AudioVisualizer)));
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(AudioVisualizer), new PropertyMetadata(Color.FromRgb(0xec, 0x36, 0x4e),
                (o, args) =>
                {
                    (o as AudioVisualizer).ImageSource = new WriteableBitmap((o as AudioVisualizer).OutputDataLenght, 128, 96, 96,
                        PixelFormats.Indexed1, new BitmapPalette(new[] {Colors.Transparent, (Color) args.NewValue}));
                }));
        
        /// <summary>
        /// Get or set color of visualizer. 
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof (WriteableBitmap), typeof (AudioVisualizer), new PropertyMetadata(default(WriteableBitmap)));

        /// <summary>
        /// Visualize's image source.
        /// </summary>
        public WriteableBitmap ImageSource
        {
            get { return (WriteableBitmap) GetValue(ImageSourceProperty); }
            internal set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// Get lenght of output data.
        /// </summary>
        public int OutputDataLenght { get; private set; }

        private int _inputDataLenght;

        /// <summary>
        /// Get lenght of input data.
        /// </summary>
        public int InputDataLenght
        {
            get { return _inputDataLenght; }
            private set
            {
                if (_inputDataLenght == value) return;

                _inputDataLenght = value;

                var temp = DataHandlers.Aggregate(_inputDataLenght, (current, dataHandler) => dataHandler.OutputDataLength(current));
                temp = DataIndexers.Aggregate(temp, (current, dataIndexer) => dataIndexer.OutputDataLength(current));

                OutputDataLenght = temp;
            }
        }

        /// <summary>
        /// Sample data handlers.
        /// </summary>
        public DataHandlerCollection DataHandlers { get; } = new DataHandlerCollection();

        /// <summary>
        /// Sample data indexers.
        /// </summary>
        public DataIndexerCollection DataIndexers { get; } = new DataIndexerCollection();

        /// <summary>
        /// Sample data.
        /// </summary>
        public float[] Data { get; private set; }

        private bool _drawing = false;

        private ulong _lostFrameCount = 0;
        private ulong _totalFrame = 0;

        /// <summary>
        /// Set input data.
        /// </summary>
        /// <param name="data"></param>
        public async void InputData(float[] data)
        {
            _totalFrame++;
            if (_drawing)
            {
                _lostFrameCount++;
                Debug.WriteLine($"Frame lost {_lostFrameCount}/{_totalFrame}.");
                return;
            }

            _drawing = true;

            InputDataLenght = data.Length;

            if (ImageSource == null)
            {
                ImageSource = new WriteableBitmap(OutputDataLenght, 128, 96, 96,
                        PixelFormats.Indexed1, new BitmapPalette(new[] { Colors.Transparent, Color }));
            }

            IntPtr imageBuffer = IntPtr.Zero;
            int stride = 0;;

            foreach (var dataHandler in DataHandlers)
            {
                data = await dataHandler.HandleData(data, this);
            }

            Data = data;

            await ImageSource.Dispatcher.InvokeAsync(() =>
            {
                ImageSource.Lock();
                imageBuffer = ImageSource.BackBuffer;
                stride = ImageSource.BackBufferStride;
            });

            await Task.Run(() =>
            {
                DrawImage(imageBuffer, stride);
            });

            await ImageSource.Dispatcher.InvokeAsync(() =>
            {
                ImageSource.AddDirtyRect(new Int32Rect(0, 0, ImageSource.PixelWidth, ImageSource.PixelHeight));
                ImageSource.Unlock();
            });

            _drawing = false;
        }

        private int MapIndex(int index)
        {
            return DataIndexers.Aggregate(index, (current, t) => t.GetIndex(current, this));
        }

        private unsafe void DrawImage(IntPtr imageBackBufffer,int stride)
        {
            fixed (float* data = Data)
            {
                byte* imageBuffer = (byte*)imageBackBufffer;

                Parallel.For(0, OutputDataLenght/8, i =>
                {
                    int index = i*8;
                    byte pixelData = 0xFF;
                    byte[] caches = new byte[] {1,1,1,1,1,1,1,1};

                    float d0 = Data[MapIndex(index + 0)];
                    float d1 = Data[MapIndex(index + 1)];
                    float d2 = Data[MapIndex(index + 2)];
                    float d3 = Data[MapIndex(index + 3)];
                    float d4 = Data[MapIndex(index + 4)];
                    float d5 = Data[MapIndex(index + 5)];
                    float d6 = Data[MapIndex(index + 6)];
                    float d7 = Data[MapIndex(index + 7)];

                    int pixelIndex = 0;

                    for (int y = 127; y >= 0; y--)
                    {
                        pixelIndex = i + y * stride;

                        if (imageBuffer[pixelIndex] == 0x00 && pixelData == 0x00)
                        {
                            break;
                        }

                        if (caches[0] == 0 || 128 - d0 > y)
                        {
                            caches[0] = 0;
                            pixelData &= 0xFF - 0x1;
                        }

                        if (caches[1] == 0 || 128 - d1 > y)
                        {
                            caches[1] = 0;
                            pixelData &= 0xFF - 0x2;
                        }

                        if (caches[2] == 0 || 128 - d2 > y)
                        {
                            caches[2] = 0;
                            pixelData &= 0xFF - 0x4;
                        }

                        if (caches[3] == 0 || 128 - d3 > y)
                        {
                            caches[3] = 0;
                            pixelData &= 0xFF - 0x8;
                        }

                        if (caches[4] == 0 || 128 - d4 > y)
                        {
                            caches[4] = 0;
                            pixelData &= 0xFF - 0x10;
                        }

                        if (caches[5] == 0 || 128 - d5 > y)
                        {
                            caches[5] = 0;
                            pixelData &= 0xFF - 0x20;
                        }

                        if (caches[6] == 0 || 128 - d6 > y)
                        {
                            caches[6] = 0;
                            pixelData &= 0xFF - 0x40;
                        }

                        if (caches[7] == 0 || 128 - d7 > y)
                        {
                            caches[7] = 0;
                            pixelData &= 0xFF - 0x80;
                        }

                        imageBuffer[pixelIndex] = pixelData;
                    }
                });
            }

        } 
    }
}
