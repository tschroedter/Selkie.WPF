using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class BitmapSourceConverter : IBitmapSourceConverter
    {
        internal const int Dpi = 96;
        private ImageSource m_BitmapSource = new BitmapImage();
        private List <List <int>> m_Data = new List <List <int>>();

        public List <List <int>> Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                m_Data = value;
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return m_BitmapSource;
            }
        }

        public void Convert()
        {
            m_BitmapSource = ToImageSource(m_Data);
        }

        internal ImageSource ToImageSource([NotNull] List <List <int>> data)
        {
            if ( data.Count == 0 ||
                 data [ 0 ].Count == 0 )
            {
                return new BitmapImage();
            }

            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgr32;
            int width = data.Count;
            int height = data [ 0 ].Count;
            int rawStride = ( width * pf.BitsPerPixel + 7 ) / 8;
            var rawImage = new byte[rawStride * height];

            InitializeImageWithData(data,
                                    rawImage);

            BitmapSource bitmap = BitmapSource.Create(width,
                                                      height,
                                                      Dpi,
                                                      Dpi,
                                                      pf,
                                                      null,
                                                      rawImage,
                                                      rawStride);

            bitmap.Freeze();

            return bitmap;
        }

        private void InitializeImageWithData(IEnumerable <List <int>> data,
                                             byte[] rawImage)
        {
            var index = 0;

            foreach ( int currentValue in data.SelectMany(current => current) )
            {
                rawImage [ index++ ] = ( byte ) ( currentValue % 256 );
                rawImage [ index++ ] = ( byte ) ( currentValue % 256 );
                rawImage [ index++ ] = ( byte ) ( currentValue % 256 );
                rawImage [ index++ ] = 255;
            }
        }
    }
}