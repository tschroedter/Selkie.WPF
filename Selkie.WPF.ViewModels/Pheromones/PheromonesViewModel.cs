using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.Pheromones
{
    public class PheromonesViewModel
        : ViewModel,
          IPheromonesViewModel
    {
        public PheromonesViewModel([NotNull] ISelkieInMemoryBus bus,
                                   [NotNull] IApplicationDispatcher dispatcher,
                                   [NotNull] IPheromonesModel model,
                                   [NotNull] IGrayscaleConverter grayscaleConverter,
                                   [NotNull] IBitmapSourceConverter bitmapSourceConverter)
        {
            m_Bus = bus;
            m_Model = model;
            m_Dispatcher = dispatcher;
            m_GrayscaleConverter = grayscaleConverter;
            m_BitmapSourceConverter = bitmapSourceConverter;

            string subscriptionId = GetType().ToString();

            bus.SubscribeAsync <PheromonesModelChangedMessage>(subscriptionId,
                                                               PheromonesHandler);
        }

        public ImageSource ImageSource
        {
            get
            {
                return m_ImageSource;
            }
        }

        private readonly IBitmapSourceConverter m_BitmapSourceConverter;
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly IApplicationDispatcher m_Dispatcher;
        private readonly IGrayscaleConverter m_GrayscaleConverter;
        private readonly IPheromonesModel m_Model;
        private readonly object m_Padlock = new object();
        private string m_Average = string.Empty;
        private ImageSource m_ImageSource = new BitmapImage();
        private bool m_IsShowPheromones;
        private string m_Maximum = string.Empty;
        private string m_Minimum = string.Empty;

        public string Minimum
        {
            get
            {
                return m_Minimum;
            }
        }

        public string Maximum
        {
            get
            {
                return m_Maximum;
            }
        }

        public string Average
        {
            get
            {
                return m_Average;
            }
        }

        public bool IsShowPheromones
        {
            get
            {
                return m_IsShowPheromones;
            }
            set
            {
                m_IsShowPheromones = value;

                m_Bus.PublishAsync(new PheromonesModelsSetMessage
                                   {
                                       IsShowPheromones = m_IsShowPheromones
                                   });
            }
        }

        internal void GenerateImageSource()
        {
            lock ( m_Padlock )
            {
                m_GrayscaleConverter.Minimum = m_Model.Minimum;
                m_GrayscaleConverter.Maximum = m_Model.Maximum;
                m_GrayscaleConverter.Pheromones = m_Model.Values;
                m_GrayscaleConverter.Convert();

                m_BitmapSourceConverter.Data = m_GrayscaleConverter.Grayscale;
                m_BitmapSourceConverter.Convert();
            }
        }

        internal void PheromonesHandler(PheromonesModelChangedMessage message)
        {
            GenerateImageSource();

            m_Dispatcher.BeginInvoke(Update);
        }

        internal void Update()
        {
            m_Minimum = m_Model.Minimum.ToString(CultureInfo.InvariantCulture);
            m_Maximum = m_Model.Maximum.ToString(CultureInfo.InvariantCulture);
            m_Average = m_Model.Average.ToString(CultureInfo.InvariantCulture);
            m_IsShowPheromones = m_Model.IsShowPheromones;

            m_ImageSource = m_BitmapSourceConverter.ImageSource;

            NotifyPropertyChanged("");
        }
    }
}