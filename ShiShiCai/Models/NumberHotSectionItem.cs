//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    e622f58b-a3ac-4e5f-a93a-021a5e292096
//        CLR Version:              4.0.30319.42000
//        Name:                     NumberHotSectionItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                NumberHotSectionItem
//
//        Created by Charley at 2019/8/21 10:50:08
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class NumberHotSectionItem : INotifyPropertyChanged
    {
        private int mSection;
        private string mName;
        private bool mIsSelected;

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
