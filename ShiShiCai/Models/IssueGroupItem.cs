//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    fa9e4425-89a7-4d3d-be23-ff395e453eb0
//        CLR Version:              4.0.30319.42000
//        Name:                     IssueGroupItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                IssueGroupItem
//
//        Created by Charley at 2019/8/11 12:17:28
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;


namespace ShiShiCai.Models
{
    public class IssueGroupItem : INotifyPropertyChanged
    {
        private int mNumber;
        private PathGeometry mPath1;
        private PathGeometry mPath2;
        private PathGeometry mPath3;
        private PathGeometry mPath4;
        private PathGeometry mPath5;

        private ObservableCollection<IssueNumberItem> mValues = new ObservableCollection<IssueNumberItem>();

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        public PathGeometry Path1
        {
            get { return mPath1; }
            set { mPath1 = value; OnPropertyChanged("Path1"); }
        }

        public PathGeometry Path2
        {
            get { return mPath2; }
            set { mPath2 = value; OnPropertyChanged("Path2"); }
        }

        public PathGeometry Path3
        {
            get { return mPath3; }
            set { mPath3 = value; OnPropertyChanged("Path3"); }
        }

        public PathGeometry Path4
        {
            get { return mPath4; }
            set { mPath4 = value; OnPropertyChanged("Path4"); }
        }

        public PathGeometry Path5
        {
            get { return mPath5; }
            set { mPath5 = value; OnPropertyChanged("Path5"); }
        }

        public ObservableCollection<IssueNumberItem> Values
        {
            get { return mValues; }
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
