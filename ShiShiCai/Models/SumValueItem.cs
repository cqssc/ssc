//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    e269296a-b3be-4077-a5ea-ee9bc878df06
//        CLR Version:              4.0.30319.42000
//        Name:                     SumValueItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                SumValueItem
//
//        Created by Charley at 2019/8/15 14:38:34
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class SumValueItem : INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;
        private int mSumValue;

        private bool mLargeValue;
        private bool mSingleValue;

        private double mItemWidth;
        private double mItemHeight;

        private bool mIsOver;

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

        public int SumValue
        {
            get { return mSumValue; }
            set { mSumValue = value; OnPropertyChanged("SumValue"); }
        }

        public bool LargeValue
        {
            get { return mLargeValue; }
            set { mLargeValue = value; OnPropertyChanged("LargeValue"); }
        }

        public bool SingleValue
        {
            get { return mSingleValue; }
            set { mSingleValue = value; OnPropertyChanged("SingleValue"); }
        }

        public double ItemWidth
        {
            get { return mItemWidth; }
            set { mItemWidth = value; OnPropertyChanged("ItemWidth"); }
        }

        public double ItemHeight
        {
            get { return mItemHeight; }
            set { mItemHeight = value; OnPropertyChanged("ItemHeight"); }
        }

        public bool IsOver
        {
            get { return mIsOver; }
            set { mIsOver = value; OnPropertyChanged("IsOver"); }
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
