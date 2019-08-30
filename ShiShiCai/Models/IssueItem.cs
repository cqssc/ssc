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

        #region 基本

        private string mSerial;
        private int mNumber;
        private int mDate;
        private int mWeekDay;

        #endregion


        #region 奖号

        private int mD1;
        private int mD2;
        private int mD3;
        private int mD4;
        private int mD5;
        private string mFullValue;  //奖号
        private int mSumValue;      //和

        #endregion


        #region 特征

        private bool mLargeValue;   //大小
        private bool mSingleValue;  //单双
        private int mRepeatValue;  //重复
        private int mIntervalValue;//间隔
        private int mLarger20;     //>20期
        private int mAllOne20;     //20期老1
        private int mPairsValue;   //一对
        private int mSameValue;    //豹子

        #endregion


        #region 高度宽度

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

        #endregion


        #region 遗漏

        private int mD1Lost0;
        private int mD1Lost1;
        private int mD1Lost2;
        private int mD1Lost3;
        private int mD1Lost4;
        private int mD1Lost5;
        private int mD1Lost6;
        private int mD1Lost7;
        private int mD1Lost8;
        private int mD1Lost9;

        private int mD2Lost0;
        private int mD2Lost1;
        private int mD2Lost2;
        private int mD2Lost3;
        private int mD2Lost4;
        private int mD2Lost5;
        private int mD2Lost6;
        private int mD2Lost7;
        private int mD2Lost8;
        private int mD2Lost9;

        private int mD3Lost0;
        private int mD3Lost1;
        private int mD3Lost2;
        private int mD3Lost3;
        private int mD3Lost4;
        private int mD3Lost5;
        private int mD3Lost6;
        private int mD3Lost7;
        private int mD3Lost8;
        private int mD3Lost9;

        private int mD4Lost0;
        private int mD4Lost1;
        private int mD4Lost2;
        private int mD4Lost3;
        private int mD4Lost4;
        private int mD4Lost5;
        private int mD4Lost6;
        private int mD4Lost7;
        private int mD4Lost8;
        private int mD4Lost9;

        private int mD5Lost0;
        private int mD5Lost1;
        private int mD5Lost2;
        private int mD5Lost3;
        private int mD5Lost4;
        private int mD5Lost5;
        private int mD5Lost6;
        private int mD5Lost7;
        private int mD5Lost8;
        private int mD5Lost9;

        #endregion


        #region 基本

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

        #endregion


        #region 奖号

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

        public int SumValue
        {
            get { return mSumValue; }
            set { mSumValue = value; OnPropertyChanged("SumValue"); }
        }

        #endregion


        #region 特征

        public bool LargeValue
        {
            get { return mLargeValue; }
            set { mLargeValue = value; OnPropertyChanged("LargeValue"); }
        }

        public bool SingleValue
        {
            get { return mSingleValue; }
            set { mSingleValue = value; OnPropertyChanged("SingleValue"); }
        }

        public int RepeatValue
        {
            get { return mRepeatValue; }
            set { mRepeatValue = value; OnPropertyChanged("RepeatValue"); }
        }

        public int IntervalValue
        {
            get { return mIntervalValue; }
            set { mIntervalValue = value; OnPropertyChanged("IntervalValue"); }
        }

        public int Larger20
        {
            get { return mLarger20; }
            set { mLarger20 = value; OnPropertyChanged("Larger20"); }
        }

        public int AllOne20
        {
            get { return mAllOne20; }
            set { mAllOne20 = value; OnPropertyChanged("AllOne20"); }
        }

        public int PairsVaue
        {
            get { return mPairsValue; }
            set { mPairsValue = value; OnPropertyChanged("PairsVaue"); }
        }

        public int SameValue
        {
            get { return mSameValue; }
            set { mSameValue = value; OnPropertyChanged("SameValue"); }
        }

        #endregion


        #region 高度宽度

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

        #endregion


        #region 遗漏

        #region D1

        public int D1Lost0
        {
            get { return mD1Lost0; }
            set { mD1Lost0 = value; OnPropertyChanged("D1Lost0"); }
        }

        public int D1Lost1
        {
            get { return mD1Lost1; }
            set { mD1Lost1 = value; OnPropertyChanged("D1Lost1"); }
        }

        public int D1Lost2
        {
            get { return mD1Lost2; }
            set { mD1Lost2 = value; OnPropertyChanged("D1Lost2"); }
        }

        public int D1Lost3
        {
            get { return mD1Lost3; }
            set { mD1Lost3 = value; OnPropertyChanged("D1Lost3"); }
        }

        public int D1Lost4
        {
            get { return mD1Lost4; }
            set { mD1Lost4 = value; OnPropertyChanged("D1Lost4"); }
        }

        public int D1Lost5
        {
            get { return mD1Lost5; }
            set { mD1Lost5 = value; OnPropertyChanged("D1Lost5"); }
        }

        public int D1Lost6
        {
            get { return mD1Lost6; }
            set { mD1Lost6 = value; OnPropertyChanged("D1Lost6"); }
        }

        public int D1Lost7
        {
            get { return mD1Lost7; }
            set { mD1Lost7 = value; OnPropertyChanged("D1Lost7"); }
        }
        public int D1Lost8
        {
            get { return mD1Lost8; }
            set { mD1Lost8 = value; OnPropertyChanged("D1Lost8"); }
        }
        public int D1Lost9
        {
            get { return mD1Lost9; }
            set { mD1Lost9 = value; OnPropertyChanged("D1Lost9"); }
        }

        #endregion


        #region D2

        public int D2Lost0
        {
            get { return mD2Lost0; }
            set { mD2Lost0 = value; OnPropertyChanged("D2Lost0"); }
        }

        public int D2Lost1
        {
            get { return mD2Lost1; }
            set { mD2Lost1 = value; OnPropertyChanged("D2Lost1"); }
        }

        public int D2Lost2
        {
            get { return mD2Lost2; }
            set { mD2Lost2 = value; OnPropertyChanged("D2Lost2"); }
        }

        public int D2Lost3
        {
            get { return mD2Lost3; }
            set { mD2Lost3 = value; OnPropertyChanged("D2Lost3"); }
        }

        public int D2Lost4
        {
            get { return mD2Lost4; }
            set { mD2Lost4 = value; OnPropertyChanged("D2Lost4"); }
        }

        public int D2Lost5
        {
            get { return mD2Lost5; }
            set { mD2Lost5 = value; OnPropertyChanged("D2Lost5"); }
        }

        public int D2Lost6
        {
            get { return mD2Lost6; }
            set { mD2Lost6 = value; OnPropertyChanged("D2Lost6"); }
        }

        public int D2Lost7
        {
            get { return mD2Lost7; }
            set { mD2Lost7 = value; OnPropertyChanged("D2Lost7"); }
        }
        public int D2Lost8
        {
            get { return mD2Lost8; }
            set { mD2Lost8 = value; OnPropertyChanged("D2Lost8"); }
        }
        public int D2Lost9
        {
            get { return mD2Lost9; }
            set { mD2Lost9 = value; OnPropertyChanged("D2Lost9"); }
        }

        #endregion


        #region D3

        public int D3Lost0
        {
            get { return mD3Lost0; }
            set { mD3Lost0 = value; OnPropertyChanged("D3Lost0"); }
        }

        public int D3Lost1
        {
            get { return mD3Lost1; }
            set { mD3Lost1 = value; OnPropertyChanged("D3Lost1"); }
        }

        public int D3Lost2
        {
            get { return mD3Lost2; }
            set { mD3Lost2 = value; OnPropertyChanged("D3Lost2"); }
        }

        public int D3Lost3
        {
            get { return mD3Lost3; }
            set { mD3Lost3 = value; OnPropertyChanged("D3Lost3"); }
        }

        public int D3Lost4
        {
            get { return mD3Lost4; }
            set { mD3Lost4 = value; OnPropertyChanged("D3Lost4"); }
        }

        public int D3Lost5
        {
            get { return mD3Lost5; }
            set { mD3Lost5 = value; OnPropertyChanged("D3Lost5"); }
        }

        public int D3Lost6
        {
            get { return mD3Lost6; }
            set { mD3Lost6 = value; OnPropertyChanged("D3Lost6"); }
        }

        public int D3Lost7
        {
            get { return mD3Lost7; }
            set { mD3Lost7 = value; OnPropertyChanged("D3Lost7"); }
        }
        public int D3Lost8
        {
            get { return mD3Lost8; }
            set { mD3Lost8 = value; OnPropertyChanged("D3Lost8"); }
        }
        public int D3Lost9
        {
            get { return mD3Lost9; }
            set { mD3Lost9 = value; OnPropertyChanged("D3Lost9"); }
        }

        #endregion


        #region D4

        public int D4Lost0
        {
            get { return mD4Lost0; }
            set { mD4Lost0 = value; OnPropertyChanged("D4Lost0"); }
        }

        public int D4Lost1
        {
            get { return mD4Lost1; }
            set { mD4Lost1 = value; OnPropertyChanged("D4Lost1"); }
        }

        public int D4Lost2
        {
            get { return mD4Lost2; }
            set { mD4Lost2 = value; OnPropertyChanged("D4Lost2"); }
        }

        public int D4Lost3
        {
            get { return mD4Lost3; }
            set { mD4Lost3 = value; OnPropertyChanged("D4Lost3"); }
        }

        public int D4Lost4
        {
            get { return mD4Lost4; }
            set { mD4Lost4 = value; OnPropertyChanged("D4Lost4"); }
        }

        public int D4Lost5
        {
            get { return mD4Lost5; }
            set { mD4Lost5 = value; OnPropertyChanged("D4Lost5"); }
        }

        public int D4Lost6
        {
            get { return mD4Lost6; }
            set { mD4Lost6 = value; OnPropertyChanged("D4Lost6"); }
        }

        public int D4Lost7
        {
            get { return mD4Lost7; }
            set { mD4Lost7 = value; OnPropertyChanged("D4Lost7"); }
        }
        public int D4Lost8
        {
            get { return mD4Lost8; }
            set { mD4Lost8 = value; OnPropertyChanged("D4Lost8"); }
        }
        public int D4Lost9
        {
            get { return mD4Lost9; }
            set { mD4Lost9 = value; OnPropertyChanged("D4Lost9"); }
        }

        #endregion


        #region D5

        public int D5Lost0
        {
            get { return mD5Lost0; }
            set { mD5Lost0 = value; OnPropertyChanged("D5Lost0"); }
        }

        public int D5Lost1
        {
            get { return mD5Lost1; }
            set { mD5Lost1 = value; OnPropertyChanged("D5Lost1"); }
        }

        public int D5Lost2
        {
            get { return mD5Lost2; }
            set { mD5Lost2 = value; OnPropertyChanged("D5Lost2"); }
        }

        public int D5Lost3
        {
            get { return mD5Lost3; }
            set { mD5Lost3 = value; OnPropertyChanged("D5Lost3"); }
        }

        public int D5Lost4
        {
            get { return mD5Lost4; }
            set { mD5Lost4 = value; OnPropertyChanged("D5Lost4"); }
        }

        public int D5Lost5
        {
            get { return mD5Lost5; }
            set { mD5Lost5 = value; OnPropertyChanged("D5Lost5"); }
        }

        public int D5Lost6
        {
            get { return mD5Lost6; }
            set { mD5Lost6 = value; OnPropertyChanged("D5Lost6"); }
        }

        public int D5Lost7
        {
            get { return mD5Lost7; }
            set { mD5Lost7 = value; OnPropertyChanged("D5Lost7"); }
        }
        public int D5Lost8
        {
            get { return mD5Lost8; }
            set { mD5Lost8 = value; OnPropertyChanged("D5Lost8"); }
        }
        public int D5Lost9
        {
            get { return mD5Lost9; }
            set { mD5Lost9 = value; OnPropertyChanged("D5Lost9"); }
        }

        #endregion

        #endregion


        #region 通知

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

    }
}
