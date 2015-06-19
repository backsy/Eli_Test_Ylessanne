using System.ComponentModel;
using System.Windows.Input;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    public class AngleViewModel : INotifyPropertyChanged
    {
        #region Construction
        /// <summary>
        /// Constructs the default instance of AngleViewModel
        /// </summary>

        public AngleViewModel()
        {
            _angleValue = new Angle { AngleValue = 0 };
            Thread angleCalculation = new Thread(new ThreadStart(StartListener));
            angleCalculation.IsBackground = true;
            angleCalculation.Start();
        }
        #endregion

        #region Members
        Angle _angleValue;
        private const int listenPort = 22333;
        public static byte[] PacketValue = new byte[7];
        #endregion       

        #region Properties
        public Angle Angle
        {
            get 
            { 
                return _angleValue; 
            }
            set 
            { 
                _angleValue = value; 
            }
        }
        
        public decimal AngleValue
        {
            get 
            {
                return Angle.AngleValue;
            }
            set 
            {
                if (Angle.AngleValue != value)
                {
                    Angle.AngleValue = value;
                    RaisePropertyChanged("AngleValue");
                }
            }
        }
     
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods
        
        private void StartListener()
        {
            bool done = false;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    byte[] angleValueByte = new byte[4] { bytes[2], bytes[3], bytes[4], bytes[5] };

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(angleValueByte);

                    int angleValueInt = BitConverter.ToInt32(angleValueByte, 0);

                    AngleValue = ((decimal)angleValueInt / 10);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
