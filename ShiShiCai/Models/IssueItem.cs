//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    ea9d3e10-eb94-4b7d-8ff9-18d95156a75f
//        CLR Version:              4.0.30319.42000
//        Name:                     IssueItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                IssueItem
//
//        Created by Charley at 2019/8/11 11:43:06
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;


namespace ShiShiCai.Models
{
    public class IssueItem : INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;
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

        private double mD1Height;
        private double mD2Height;
        private double mD3Height;
        private double mD4Height;
        private double mD5Height;

        private double mD1Width;
        private double mD2Width;
        private double mD3Width;
        private double mD4Width;
        private double mD5Width;


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

        public double D1Height
        {
            get { return mD1Height; }
            set { mD1Height = value; OnPropertyChanged("D1Height"); }
        }

        public double D2Height
        {
            get { return mD2Height; }
            set { mD2Height = value; OnPropertyChanged("D2Height"); }
        }

        public double D3Height
        {
            get { return mD3Height; }
            set { mD3Height = value; OnPropertyChanged("D3Height"); }
        }

        public double D4Height
        {
            get { return mD4Height; }
            set { mD4Height = value; OnPropertyChanged("D4Height"); }
        }

        public double D5Height
        {
            get { return mD5Height; }
            set { mD5Height = value; OnPropertyChanged("D5Height"); }
        }

        public double D1Width
        {
            get { return mD1Width; }
            set { mD1Width = value; OnPropertyChanged("D1Width"); }
        }

        public double D2Width
        {
            get { return mD2Width; }
            set { mD2Width = value; OnPropertyChanged("D2Width"); }
        }

        public double D3Width
        {
            get { return mD3Width; }
            set { mD3Width = value; OnPropertyChanged("D3Width"); }
        }

        public double D4Width
        {
            get { return mD4Width; }
            set { mD4Width = value; OnPropertyChanged("D4Width"); }
        }

        public double D5Width
        {
            get { return mD5Width; }
            set { mD5Width = value; OnPropertyChanged("D5Width"); }
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
