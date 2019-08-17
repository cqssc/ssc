//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    15fc0c75-6206-4669-b816-6dc63cd72151
//        CLR Version:              4.0.30319.42000
//        Name:                     SectionLargeSmallItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                SectionLargeSmallItem
//
//        Created by Charley at 2019/8/17 8:25:41
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class SectionLargeSmallItem:INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;

        private bool mLargeValue;
        private int mTimes;

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

        public bool LargeValue
        {
            get { return mLargeValue; }
            set { mLargeValue = value; OnPropertyChanged("LargeValue"); }
        }

        public int Times
        {
            get { return mTimes; }
            set { mTimes = value; OnPropertyChanged("Times"); }
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
