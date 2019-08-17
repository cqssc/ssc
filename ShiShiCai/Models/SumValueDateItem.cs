//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    4651999c-d3a6-4b39-bbde-c01835b6980c
//        CLR Version:              4.0.30319.42000
//        Name:                     SumValueDateItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                SumValueDateItem
//
//        Created by Charley at 2019/8/17 7:52:08
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class SumValueDateItem : INotifyPropertyChanged
    {
        private int mDate;

        public int Date
        {
            get { return mDate; }
            set { mDate = value; OnPropertyChanged("Date"); }
        }

        private ObservableCollection<SumValueItem> mItems = new ObservableCollection<SumValueItem>();

        public ObservableCollection<SumValueItem> Items
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
