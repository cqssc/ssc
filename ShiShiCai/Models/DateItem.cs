//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    3d12c792-f5db-4907-9cc0-10317c76f4aa
//        CLR Version:              4.0.30319.42000
//        Name:                     DateItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                DateItem
//
//        Created by Charley at 2019/8/23 9:29:51
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class DateItem:INotifyPropertyChanged
    {
        private int mDate;

        public int Date
        {
            get { return mDate; }
            set { mDate = value; OnPropertyChanged("Date"); }
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
