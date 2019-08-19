//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    ea38276b-ee48-4ae2-adf8-08fb9f393ced
//        CLR Version:              4.0.30319.42000
//        Name:                     PositionLargeSmallItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                PositionLargeSmallItem
//
//        Created by Charley at 2019/8/19 16:16:14
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class PositionLargeSmallItem : INotifyPropertyChanged
    {
        private int mPos;
        private string mName;
        private int mLargeNum;
        private int mSmallNum;
        private int mSingleNum;
        private int mDoubleNum;
        private int mLargeMaxNum;
        private int mSmallMaxNum;
        private int mSingleMaxNum;
        private int mDoubleMaxNum;

        private ObservableCollection<LargeSmallItem> mItems = new ObservableCollection<LargeSmallItem>();

        public int Pos
        {
            get { return mPos; }
            set { mPos = value; OnPropertyChanged("Pos"); }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
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

        public int LargeMaxNum
        {
            get { return mLargeMaxNum; }
            set { mLargeMaxNum = value; OnPropertyChanged("LargeMaxNum"); }
        }

        public int SmallMaxNum
        {
            get { return mSmallMaxNum; }
            set { mSmallMaxNum = value; OnPropertyChanged("SmallMaxNum"); }
        }

        public int SingleMaxNum
        {
            get { return mSingleMaxNum; }
            set { mSingleMaxNum = value; OnPropertyChanged("SingleMaxNum"); }
        }

        public int DoubleMaxNum
        {
            get { return mDoubleMaxNum; }
            set { mDoubleMaxNum = value; OnPropertyChanged("DoubleMaxNum"); }
        }

        public ObservableCollection<LargeSmallItem> Items
        {
            get { return mItems; }
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
