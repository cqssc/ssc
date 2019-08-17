//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    3acb0c4e-3890-460e-90c2-e5e858c75338
//        CLR Version:              4.0.30319.42000
//        Name:                     SectionDataItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                SectionDataItem
//
//        Created by Charley at 2019/8/16 17:24:47
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class SectionDataItem : INotifyPropertyChanged
    {
        private int mNumber;
        private int mSection;
        private string mName;

        private int mLargeNum;
        private int mLargeMaxNum;
        private int mSmallNum;
        private int mSmallMaxNum;

        private ObservableCollection<SectionLargeSmallItem> mItems = new ObservableCollection<SectionLargeSmallItem>();

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        public int Section
        {
            get { return mSection; }
            set { mSection = value; OnPropertyChanged("Section"); }
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

        public int LargeMaxNum
        {
            get { return mLargeMaxNum; }
            set { mLargeMaxNum = value; OnPropertyChanged("LargeMaxNum"); }
        }

        public int SmallNum
        {
            get { return mSmallNum; }
            set { mSmallNum = value; OnPropertyChanged("SmallNum"); }
        }

        public int SmallMaxNum
        {
            get { return mSmallMaxNum; }
            set { mSmallMaxNum = value; OnPropertyChanged("SmallMaxNum"); }
        }

        public ObservableCollection<SectionLargeSmallItem> Items
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
