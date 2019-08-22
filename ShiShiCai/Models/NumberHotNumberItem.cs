//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    b9166961-1e54-4fa2-9b0d-3193b4fb9e6e
//        CLR Version:              4.0.30319.42000
//        Name:                     NumberHotNumberItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                NumberHotNumberItem
//
//        Created by Charley at 2019/8/21 11:19:11
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;


namespace ShiShiCai.Models
{
    public class NumberHotNumberItem : INotifyPropertyChanged
    {
        private int mNumber;
        private Brush mColor;
        private PathGeometry mPath;
        private int mSum;
        private int mMax;
        private int mMin;
        private double mAvg;
        private bool mIsSelected;

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        public Brush Color
        {
            get { return mColor; }
            set { mColor = value; OnPropertyChanged("Color"); }
        }

        public PathGeometry Path
        {
            get { return mPath; }
            set { mPath = value; OnPropertyChanged("Path"); }
        }

        public int Sum
        {
            get { return mSum; }
            set { mSum = value; OnPropertyChanged("Sum"); }
        }

        public int Max
        {
            get { return mMax; }
            set { mMax = value; OnPropertyChanged("Max"); }
        }

        public int Min
        {
            get { return mMin; }
            set { mMin = value; OnPropertyChanged("Min"); }
        }

        public double Avg
        {
            get { return mAvg; }
            set { mAvg = value; OnPropertyChanged("Avg"); }
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
