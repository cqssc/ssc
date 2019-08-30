//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    93c1fd2c-ac10-4eab-a753-5f0909970521
//        CLR Version:              4.0.30319.42000
//        Name:                     TendencyPositionItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                TendencyPositionItem
//
//        Created by Charley at 2019/8/28 17:13:12
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;


namespace ShiShiCai.Models
{
    public class TendencyPositionItem : INotifyPropertyChanged
    {
        private int mPos;
        private string mName;
        private Brush mColor;
        private bool mVisible;
        private PathGeometry mPath;

        private int mSum1;
        private int mSum2;
        private int mSum3;
        private int mSum4;

        private int mMax1;
        private int mMax2;
        private int mMax3;
        private int mMax4;

        public int Pos
        {
            get { return mPos; }
            set { mPos = value; OnPropertyChanged("Pos"); }
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

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; OnPropertyChanged("Visible"); }
        }

        public PathGeometry Path
        {
            get { return mPath; }
            set { mPath = value; OnPropertyChanged("Path"); }
        }

        private readonly ObservableCollection<TendencyDetailItem> mItems = new ObservableCollection<TendencyDetailItem>();
        private readonly ObservableCollection<TendencyNumberItem> mNumberItems = new ObservableCollection<TendencyNumberItem>();

        public ObservableCollection<TendencyDetailItem> Items
        {
            get { return mItems; }
        }

        public ObservableCollection<TendencyNumberItem> NumberItems
        {
            get { return mNumberItems; }
        }

        public int Sum1
        {
            get { return mSum1; }
            set { mSum1 = value; OnPropertyChanged("Sum1"); }
        }

        public int Sum2
        {
            get { return mSum2; }
            set { mSum2 = value; OnPropertyChanged("Sum2"); }
        }

        public int Sum3
        {
            get { return mSum3; }
            set { mSum3 = value; OnPropertyChanged("Sum3"); }
        }

        public int Sum4
        {
            get { return mSum4; }
            set { mSum4 = value; OnPropertyChanged("Sum4"); }
        }

        public int Max1
        {
            get { return mMax1; }
            set { mMax1 = value; OnPropertyChanged("Max1"); }
        }

        public int Max2
        {
            get { return mMax2; }
            set { mMax2 = value; OnPropertyChanged("Max2"); }
        }

        public int Max3
        {
            get { return mMax3; }
            set { mMax3 = value; OnPropertyChanged("Max3"); }
        }

        public int Max4
        {
            get { return mMax4; }
            set { mMax4 = value; OnPropertyChanged("Max4"); }
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
