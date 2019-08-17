//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    0d21a85a-9bce-4693-87d7-6f25b1784918
//        CLR Version:              4.0.30319.42000
//        Name:                     LargeSmallItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                LargeSmallItem
//
//        Created by Charley at 2019/8/16 14:19:03
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class LargeSmallItem : INotifyPropertyChanged
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

        private int mLargeNum;
        private int mSmallNum;
        private int mSingleNum;
        private int mDoubleNum;

        private double mItemWidth;
        private double mItemHeight;

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

        public int LargeNum
        {
            get { return mLargeNum; }
            set { mLargeNum = value; OnPropertyChanged("LargeNum"); }
        }

        public int SmallNum
        {
            get { return mSmallNum; }
            set { mSmallNum = value; OnPropertyChanged("SmallNum"); }
        }

        public int SingleNum
        {
            get { return mSingleNum; }
            set { mSingleNum = value; OnPropertyChanged("SingleNum"); }
        }

        public int DoubleNum
        {
            get { return mDoubleNum; }
            set { mDoubleNum = value; OnPropertyChanged("DoubleNum"); }
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
