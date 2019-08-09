//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    c4e153d0-966a-4683-a62a-13cd78b8b338
//        CLR Version:              4.0.30319.42000
//        Name:                     IssueItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai
//        File Name:                IssueItem
//
//        Created by Charley at 2019/8/8 11:37:50
//        http://www.netinfo.com 
//
//======================================================================

using System;
using System.ComponentModel;


namespace ShiShiCai
{
    public class IssueItem : INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private DateTime mDate;
        private int mWeekDay;

        private int mD1;
        private int mD2;
        private int mD3;
        private int mD4;
        private int mD5;

        private string mFullValue;  //奖号
        private bool mLargeValue;   //大小
        private bool mDoubleValue;  //单双
        private int mSumValue;      //和
        private bool mRepeatValue;  //重复
        private bool mIntervalValue;//间隔
        private bool mLarger20;     //>20期
        private bool mAllOne20;     //20期老1
        private bool mPairsValue;   //一对
        private bool mSameValue;    //豹子


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

        public DateTime Date
        {
            get { return mDate; }
            set { mDate = value; OnPropertyChanged("Date"); }
        }

        public int WeekDay
        {
            get { return mWeekDay; }
            set { mWeekDay = value; OnPropertyChanged("WeekDay"); }
        }

        public int D1
        {
            get { return mD1; }
            set { mD1 = value; OnPropertyChanged("D1"); }
        }

        public int D2
        {
            get { return mD2; }
            set { mD2 = value; OnPropertyChanged("D2"); }
        }

        public int D3
        {
            get { return mD3; }
            set { mD3 = value; OnPropertyChanged("D3"); }
        }

        public int D4
        {
            get { return mD4; }
            set { mD4 = value; OnPropertyChanged("D4"); }
        }

        public int D5
        {
            get { return mD5; }
            set { mD5 = value; OnPropertyChanged("D5"); }
        }

        public string FullValue
        {
            get { return mFullValue; }
            set { mFullValue = value; OnPropertyChanged("FullValue"); }
        }

        public bool LargeValue
        {
            get { return mLargeValue; }
            set { mLargeValue = value; OnPropertyChanged("LargeValue"); }
        }

        public bool DoubleValue
        {
            get { return mDoubleValue; }
            set { mDoubleValue = value; OnPropertyChanged("DoubleValue"); }
        }

        public int SumValue
        {
            get { return mSumValue; }
            set { mSumValue = value; OnPropertyChanged("SumValue"); }
        }

        public bool RepeatValue
        {
            get { return mRepeatValue; }
            set { mRepeatValue = value; OnPropertyChanged("RepeatValue"); }
        }

        public bool IntervalValue
        {
            get { return mIntervalValue; }
            set { mIntervalValue = value; OnPropertyChanged("IntervalValue"); }
        }

        public bool Larger20
        {
            get { return mLarger20; }
            set { mLarger20 = value; OnPropertyChanged("Larger20"); }
        }

        public bool AllOne20
        {
            get { return mAllOne20; }
            set { mAllOne20 = value; OnPropertyChanged("AllOne20"); }
        }

        public bool PairsVaue
        {
            get { return mPairsValue; }
            set { mPairsValue = value; OnPropertyChanged("PairsVaue"); }
        }

        public bool SameValue
        {
            get { return mSameValue; }
            set { mSameValue = value; OnPropertyChanged("SameValue"); }
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
