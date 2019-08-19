//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    dcb26a1c-6b90-4e2e-b388-dababd27c95a
//        CLR Version:              4.0.30319.42000
//        Name:                     LargeSmallDateItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                LargeSmallDateItem
//
//        Created by Charley at 2019/8/19 14:08:37
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class LargeSmallDateItem : INotifyPropertyChanged
    {
        private int mDate;

        public int Date
        {
            get { return mDate; }
            set { mDate = value; OnPropertyChanged("Date"); }
        }

        private readonly ObservableCollection<LargeSmallItem> mItems = new ObservableCollection<LargeSmallItem>();

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
