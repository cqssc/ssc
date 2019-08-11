//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    d34257e3-aff1-407f-8fe0-baa550bb7d18
//        CLR Version:              4.0.30319.42000
//        Name:                     IssueNumberItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                IssueNumberItem
//
//        Created by Charley at 2019/8/11 12:15:01
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class IssueNumberItem : INotifyPropertyChanged
    {
        private int mNumber;
        private int mValue;

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        public int Value
        {
            get { return mValue; }
            set { mValue = value; OnPropertyChanged("Value"); }
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
