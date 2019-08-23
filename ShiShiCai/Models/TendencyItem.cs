//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    33197ba1-4b1f-4273-be1d-5cf953687bd4
//        CLR Version:              4.0.30319.42000
//        Name:                     TendencyItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                TendencyItem
//
//        Created by Charley at 2019/8/23 10:06:28
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class TendencyItem:INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;
        private int mPos;

        private bool mRepeat;
        private bool mOscillation;
        private bool mIncrease;
        private bool mOther;

        private int mTimes;
        private int mRange;

        public string Serial
        {
            get { return mSerial; }
            set { mSerial = value; OnPropertyChanged("Serial"); }
        }

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        public int Date
        {
            get { return mDate; }
            set { mDate = value; OnPropertyChanged("Date"); }
        }

        public int Pos
        {
            get { return mPos; }
            set { mPos = value; OnPropertyChanged("Pos"); }
        }

        public bool Repeat
        {
            get { return mRepeat; }
            set { mRepeat = value; OnPropertyChanged("Repeat"); }
        }

        public bool Osillation
        {
            get { return mOscillation; }
            set { mOscillation = value; OnPropertyChanged("Osillation"); }
        }

        public bool Increase
        {
            get { return mIncrease; }
            set { mIncrease = value; OnPropertyChanged("Increase"); }
        }

        public bool Other
        {
            get { return mOther; }
            set { mOther = value; OnPropertyChanged("Other"); }
        }

        public int Times
        {
            get { return mTimes; }
            set { mTimes = value; OnPropertyChanged("Times"); }
        }

        public int Range
        {
            get { return mRange; }
            set { mRange = value; OnPropertyChanged("Range"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
