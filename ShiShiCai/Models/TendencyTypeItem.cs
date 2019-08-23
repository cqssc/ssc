//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    0ca8d1a9-fd1b-4870-b97b-c68ed22cbc0d
//        CLR Version:              4.0.30319.42000
//        Name:                     TendencyTypeItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                TendencyTypeItem
//
//        Created by Charley at 2019/8/23 10:16:33
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;


namespace ShiShiCai.Models
{
    public class TendencyTypeItem : INotifyPropertyChanged
    {
        private int mNumber;
        private string mName;
        private bool mIsChecked;
        private Brush mColor;

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        public Brush Color
        {
            get { return mColor; }
            set { mColor = value; OnPropertyChanged("Color"); }
        }

        public bool IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; OnPropertyChanged("IsChecked"); }
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
