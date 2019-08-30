//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    c4ecf878-32f3-4aac-9021-3c823fe7f41d
//        CLR Version:              4.0.30319.42000
//        Name:                     TendencyDetailItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                TendencyDetailItem
//
//        Created by Charley at 2019/8/30 18:07:05
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class TendencyDetailItem : INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;

        private int mPos;
        private int mCategory;
        private int mTimes;
        private int mRange;

        public string Serial
        {
            get { return mSerial; }
            set { mSerial = value; OnPropertyChanged("Serial"); }
        }

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        public int Date
        {
            get { return mDate; }
            set { mDate = value; OnPropertyChanged("Date"); }
        }

        public int Pos
        {
            get { return mPos; }
            set { mPos = value; OnPropertyChanged("Pos"); }
        }

        public int Category
        {
            get { return mCategory; }
            set { mCategory = value; OnPropertyChanged("Category"); }
        }

        public int Times
        {
            get { return mTimes; }
            set { mTimes = value; OnPropertyChanged("Times"); }
        }

        public int Range
        {
            get { return mRange; }
            set { mRange = value; OnPropertyChanged("Range"); }
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
