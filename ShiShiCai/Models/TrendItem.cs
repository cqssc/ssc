//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    5dc84d41-dd04-4be3-ab77-0741224b9c2c
//        CLR Version:              4.0.30319.42000
//        Name:                     TrendItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                TrendItem
//
//        Created by Charley at 2019/8/14 16:51:23
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class TrendItem : INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;
        private int mPos;
        private int mLarge;
        private int mSmall;
        private int mSingle;
        private int mDouble;
        private int mLargeSmallNum;
        private int mSingleDoubleNum;

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

        public int Large
        {
            get { return mLarge; }
            set { mLarge = value; OnPropertyChanged("Large"); }
        }

        public int Small
        {
            get { return mSmall; }
            set { mSmall = value; OnPropertyChanged("Small"); }
        }

        public int Single
        {
            get { return mSingle; }
            set { mSingle = value; OnPropertyChanged("Single"); }
        }

        public int Double
        {
            get { return mDouble; }
            set { mDouble = value; OnPropertyChanged("Double"); }
        }

        public int LargeSmallNum
        {
            get { return mLargeSmallNum; }
            set { mLargeSmallNum = value; OnPropertyChanged("LargeSmallNum"); }
        }

        public int SingleDoubleNum
        {
            get { return mSingleDoubleNum; }
            set { mSingleDoubleNum = value; OnPropertyChanged("SingleDoubleNum"); }
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
