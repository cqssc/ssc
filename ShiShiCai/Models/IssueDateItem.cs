//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    86f6c13c-2bd2-4a48-b8ef-9e5c4bbbfe71
//        CLR Version:              4.0.30319.42000
//        Name:                     IssueDateItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                IssueDateItem
//
//        Created by Charley at 2019/8/11 12:14:26
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class IssueDateItem : INotifyPropertyChanged
    {
        private int mDate;

        private ObservableCollection<IssueItem> mIssues = new ObservableCollection<IssueItem>();
        private ObservableCollection<IssueGroupItem> mGroups = new ObservableCollection<IssueGroupItem>(); 

        public int Date
        {
            get { return mDate; }
            set { mDate = value; OnPropertyChanged("Date"); }
        }

        public ObservableCollection<IssueItem> Issues
        {
            get { return mIssues; }
        }

        public ObservableCollection<IssueGroupItem> Groups
        {
            get { return mGroups; }
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
