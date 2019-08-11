//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    2b1f9a3e-e473-472e-8484-d3004709de6f
//        CLR Version:              4.0.30319.42000
//        Name:                     PositionItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                PositionItem
//
//        Created by Charley at 2019/8/11 14:07:12
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;


namespace ShiShiCai.Models
{
    public class PositionItem : INotifyPropertyChanged
    {
        private int mNumber;
        private string mName;
        private Brush mBrush;
        private bool mIsShow;

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

        public Brush Brush
        {
            get { return mBrush; }
            set { mBrush = value; OnPropertyChanged("Brush"); }
        }

        public bool IsShow
        {
            get { return mIsShow; }
            set { mIsShow = value; OnPropertyChanged("IsShow"); }
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
