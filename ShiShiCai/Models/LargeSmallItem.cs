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

        private int mD1;
        private int mD2;
        private int mD3;
        private int mD4;
        private int mD5;

        private int mSumValue;

        private bool mLarge;
        private bool mSmall;
        private bool mSingle;
        private bool mDouble;
        private int mLargeSmallNum;
        private int mSingleDoubleNum;

        private int mLargeNum;
        private int mSmallNum;
        private int mSingleNum;
        private int mDoubleNum;

        private double mItemWidth;
        private double mItemHeight;

        private bool mIsSelected;

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

        public int D1
        {
            get { return mD1; }
            set { mD1 = value; OnPropertyChanged("D1"); }
        }

        public int D2
        {
            get { return mD2; }
            set { mD2 = value; OnPropertyChanged("D2"); }
        }

        public int D3
        {
            get { return mD3; }
            set { mD3 = value; OnPropertyChanged("D3"); }
        }

        public int D4
        {
            get { return mD4; }
            set { mD4 = value; OnPropertyChanged("D4"); }
        }

        public int D5
        {
            get { return mD5; }
            set { mD5 = value; OnPropertyChanged("D5"); }
        }

        public int SumValue
        {
            get { return mSumValue; }
            set { mSumValue = value; OnPropertyChanged("SumValue"); }
        }

        public bool Large
        {
            get { return mLarge; }
            set { mLarge = value; OnPropertyChanged("Large"); }
        }

        public bool Small
        {
            get { return mSmall; }
            set { mSmall = value; OnPropertyChanged("Small"); }
        }

        public bool Single
        {
            get { return mSingle; }
            set { mSingle = value; OnPropertyChanged("Single"); }
        }

        public bool Double
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

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
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
