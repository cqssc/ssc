//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    854edf14-bf0f-4156-8d95-9c0a5d74602c
//        CLR Version:              4.0.30319.42000
//        Name:                     PositionNumberHotItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                PositionNumberHotItem
//
//        Created by Charley at 2019/8/22 10:11:01
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class PositionNumberHotItem : INotifyPropertyChanged
    {
        private int mPos;
        private string mName;
        private double mItemWidth;

        private readonly ObservableCollection<NumberHotItem> mItems = new ObservableCollection<NumberHotItem>();
        private readonly ObservableCollection<int> mYAxisLabels = new ObservableCollection<int>();
        private readonly ObservableCollection<NumberHotNumberItem> mNumberItems = new ObservableCollection<NumberHotNumberItem>();

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

        public double ItemWidth
        {
            get { return mItemWidth; }
            set { mItemWidth = value; OnPropertyChanged("ItemWidth"); }
        }

        public ObservableCollection<NumberHotItem> Items
        {
            get { return mItems; }
        }

        public ObservableCollection<int> YAxisLabels
        {
            get { return mYAxisLabels; }
        }

        public ObservableCollection<NumberHotNumberItem> NumberItems
        {
            get { return mNumberItems; }
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
