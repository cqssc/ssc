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
        private Brush mColor;
        private bool mVisible;
        private PathGeometry mPath;

        public int Pos
        {
            get { return mPos; }
            set { mPos = value; OnPropertyChanged("Pos"); }
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

        private readonly ObservableCollection<TendencyNumberItem> mItems = new ObservableCollection<TendencyNumberItem>();

        public ObservableCollection<TendencyNumberItem> Items
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
