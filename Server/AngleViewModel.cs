using System.ComponentModel;
using System.Windows.Input;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;

namespace Server
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
            Thread angleCalculation = new Thread(new ThreadStart(AngleCalculation));
            angleCalculation.IsBackground = true;
            angleCalculation.Start();
        }
        #endregion

        #region Members
        Angle _angleValue;
        int _clickDirection = 0;
        byte[] packet = new byte[7];

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
        
        private void AngleCalculation()
        {
            while(true)
            {
                //after 33 cycles of increasing/decreasing AngleValue by 0.152
                //AngleValue is off by 0.016 due to rounding error
                //compensationCounter helps make sure AngleValue decreases/increases 3 degrees per second
                int compensationCounter = 0;
                switch (_clickDirection)
                {
                    case 0:
                        if (AngleValue > 0.1m)
                        {
                            AngleValue -= 0.152m;
                            if (compensationCounter == 33)
                            {
                                AngleValue += 0.016m;
                            }
                            compensationCounter++;
                        }
                        else if (AngleValue < -0.1m)
                        {
                            AngleValue += 0.152m;
                            if (compensationCounter == 33)
                            {
                                AngleValue -= 0.016m;
                            }
                            compensationCounter++;
                        }
                        else 
                        {
                            AngleValue = 0.0m;
                        }
                        MakePackets();
                        UDPSend.Send(packet);
                        Thread.Sleep(50);
                        break;
                    case 1:
                        AngleValue -= 5.000m;
                        MakePackets();
                        UDPSend.Send(packet);
                        _clickDirection = 0;
                        Thread.Sleep(50);
                        break;
                    case 2:
                        AngleValue += 5.000m;
                        MakePackets();
                        UDPSend.Send(packet);
                        _clickDirection = 0;
                        Thread.Sleep(50);
                        break;
                    default:
                        System.Console.WriteLine("Unexpected _clickDirection value!");
                        break;                    
                }                
            }            
        }

        private void MakePackets()
        {
            //0x255 to bytes
            byte[] head = BitConverter.GetBytes(0x255);
            byte[] headTwoByte = new byte[2];
            headTwoByte[0] = head[0];
            headTwoByte[1] = head[1];

            //revese if system littleEndian
            if (BitConverter.IsLittleEndian)
                Array.Reverse(headTwoByte);

            //cast AngleValue*10 decimal to int loosing all extra decimal values
            int decimalToInt = (int)(AngleValue*10);

            //get bytes from int
            byte[] bytesAngleValue = BitConverter.GetBytes(decimalToInt);

            //int is big-endian, reverse if needed
            if(BitConverter.IsLittleEndian)         
                Array.Reverse(bytesAngleValue);   
         
            //create checksum
            byte checksum = 0;
            foreach(byte data in bytesAngleValue)
            {
                checksum += data;
            }

            packet[0] = headTwoByte[0];
            packet[1] = headTwoByte[1];
            packet[2] = bytesAngleValue[0];
            packet[3] = bytesAngleValue[1];
            packet[4] = bytesAngleValue[2];
            packet[5] = bytesAngleValue[3];
            packet[6] = checksum;
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

        #region Commands
        void ClickDecreaseAngleExecute()
        {
            _clickDirection = 1;
        }

        bool CanClickDecreaseAngleExecute()
        {
            return true;
        }

        public ICommand ClickDecreaseAngle 
        { 
            get 
            { 
                return new RelayCommand(ClickDecreaseAngleExecute, CanClickDecreaseAngleExecute); 
            } 
        }

        void ClickIncreaseAngleExecute()
        {
            _clickDirection = 2;
        }

        bool CanClickIncreaseAngleExecute()
        {
            return true;
        }

        public ICommand ClickIncreaseAngle
        {
            get
            {
                return new RelayCommand(ClickIncreaseAngleExecute, CanClickIncreaseAngleExecute);
            }
        }

        #endregion
    }
}
