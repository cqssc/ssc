//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    ed3b2fa2-0cd9-4664-8745-6547a9e31313
//        CLR Version:              4.0.30319.42000
//        Name:                     ModuleItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                ModuleItem
//
//        Created by Charley at 2019/8/11 11:45:41
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class ModuleItem : INotifyPropertyChanged
    {
        private int mNumber;
        private string mName;
        private string mTitle;
        private string mIcon;

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

        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; OnPropertyChanged("Title"); }
        }

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
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
