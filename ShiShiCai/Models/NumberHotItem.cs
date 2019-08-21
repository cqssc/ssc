//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    66e21dfc-f98c-45da-8e4a-ff836ebbf9ff
//        CLR Version:              4.0.30319.42000
//        Name:                     NumberHotItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                NumberHotItem
//
//        Created by Charley at 2019/8/21 10:31:06
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;


namespace ShiShiCai.Models
{
    public class NumberHotItem : INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;
        private int mPos;
        private int mSection;

        private int mNum0;
        private int mNum1;
        private int mNum2;
        private int mNum3;
        private int mNum4;
        private int mNum5;
        private int mNum6;
        private int mNum7;
        private int mNum8;
        private int mNum9;

        private double mItemWidth;
        private double mItemHeight;

        private double mNum0Height;
        private double mNum1Height;
        private double mNum2Height;
        private double mNum3Height;
        private double mNum4Height;
        private double mNum5Height;
        private double mNum6Height;
        private double mNum7Height;
        private double mNum8Height;
        private double mNum9Height;

        private Brush mNum0Color;
        private Brush mNum1Color;
        private Brush mNum2Color;
        private Brush mNum3Color;
        private Brush mNum4Color;
        private Brush mNum5Color;
        private Brush mNum6Color;
        private Brush mNum7Color;
        private Brush mNum8Color;
        private Brush mNum9Color;

        private bool mNum0Selected;
        private bool mNum1Selected;
        private bool mNum2Selected;
        private bool mNum3Selected;
        private bool mNum4Selected;
        private bool mNum5Selected;
        private bool mNum6Selected;
        private bool mNum7Selected;
        private bool mNum8Selected;
        private bool mNum9Selected;

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

        public int Section
        {
            get { return mSection; }
            set { mSection = value; OnPropertyChanged("Section"); }
        }

        public int Num0
        {
            get { return mNum0; }
            set { mNum0 = value; OnPropertyChanged("Num0"); }
        }

        public int Num1
        {
            get { return mNum1; }
            set { mNum1 = value; OnPropertyChanged("Num1"); }
        }

        public int Num2
        {
            get { return mNum2; }
            set { mNum2 = value; OnPropertyChanged("Num2"); }
        }

        public int Num3
        {
            get { return mNum3; }
            set { mNum3 = value; OnPropertyChanged("Num3"); }
        }

        public int Num4
        {
            get { return mNum4; }
            set { mNum4 = value; OnPropertyChanged("Num4"); }
        }

        public int Num5
        {
            get { return mNum5; }
            set { mNum5 = value; OnPropertyChanged("Num5"); }
        }

        public int Num6
        {
            get { return mNum6; }
            set { mNum6 = value; OnPropertyChanged("Num6"); }
        }

        public int Num7
        {
            get { return mNum7; }
            set { mNum7 = value; OnPropertyChanged("Num7"); }
        }

        public int Num8
        {
            get { return mNum8; }
            set { mNum8 = value; OnPropertyChanged("Num8"); }
        }

        public int Num9
        {
            get { return mNum9; }
            set { mNum9 = value; OnPropertyChanged("Num9"); }
        }

        public double ItemWidth
        {
            get { return mItemWidth; }
            set { mItemWidth = value; OnPropertyChanged("ItemWidth"); }
        }

        public double ItemHeight
        {
            get { return mItemHeight; }
            set { mItemHeight = value; OnPropertyChanged("ItemHeight"); }
        }

        public double Num0Height
        {
            get { return mNum0Height; }
            set { mNum0Height = value; OnPropertyChanged("Num0Height"); }
        }

        public double Num1Height
        {
            get { return mNum1Height; }
            set { mNum1Height = value; OnPropertyChanged("Num1Height"); }
        }

        public double Num2Height
        {
            get { return mNum2Height; }
            set { mNum2Height = value; OnPropertyChanged("Num2Height"); }
        }

        public double Num3Height
        {
            get { return mNum3Height; }
            set { mNum3Height = value; OnPropertyChanged("Num3Height"); }
        }

        public double Num4Height
        {
            get { return mNum4Height; }
            set { mNum4Height = value; OnPropertyChanged("Num4Height"); }
        }

        public double Num5Height
        {
            get { return mNum5Height; }
            set { mNum5Height = value; OnPropertyChanged("Num5Height"); }
        }

        public double Num6Height
        {
            get { return mNum6Height; }
            set { mNum6Height = value; OnPropertyChanged("Num6Height"); }
        }

        public double Num7Height
        {
            get { return mNum7Height; }
            set { mNum7Height = value; OnPropertyChanged("Num7Height"); }
        }

        public double Num8Height
        {
            get { return mNum8Height; }
            set { mNum8Height = value; OnPropertyChanged("Num8Height"); }
        }

        public double Num9Height
        {
            get { return mNum9Height; }
            set { mNum9Height = value; OnPropertyChanged("Num9Height"); }
        }

        public Brush Num0Color
        {
            get { return mNum0Color; }
            set { mNum0Color = value; OnPropertyChanged("Num0Color"); }
        }

        public Brush Num1Color
        {
            get { return mNum1Color; }
            set { mNum1Color = value; OnPropertyChanged("Num1Color"); }
        }

        public Brush Num2Color
        {
            get { return mNum2Color; }
            set { mNum2Color = value; OnPropertyChanged("Num2Color"); }
        }

        public Brush Num3Color
        {
            get { return mNum3Color; }
            set { mNum3Color = value; OnPropertyChanged("Num3Color"); }
        }

        public Brush Num4Color
        {
            get { return mNum4Color; }
            set { mNum4Color = value; OnPropertyChanged("Num4Color"); }
        }

        public Brush Num5Color
        {
            get { return mNum5Color; }
            set { mNum5Color = value; OnPropertyChanged("Num5Color"); }
        }

        public Brush Num6Color
        {
            get { return mNum6Color; }
            set { mNum6Color = value; OnPropertyChanged("Num6Color"); }
        }

        public Brush Num7Color
        {
            get { return mNum7Color; }
            set { mNum7Color = value; OnPropertyChanged("Num7Color"); }
        }

        public Brush Num8Color
        {
            get { return mNum8Color; }
            set { mNum8Color = value; OnPropertyChanged("Num8Color"); }
        }

        public Brush Num9Color
        {
            get { return mNum9Color; }
            set { mNum9Color = value; OnPropertyChanged("Num9Color"); }
        }

        public bool Num0Selected
        {
            get { return mNum0Selected; }
            set { mNum0Selected = value; OnPropertyChanged("Num0Selected"); }
        }

        public bool Num1Selected
        {
            get { return mNum1Selected; }
            set { mNum1Selected = value; OnPropertyChanged("Num1Selected"); }
        }

        public bool Num2Selected
        {
            get { return mNum2Selected; }
            set { mNum2Selected = value; OnPropertyChanged("Num2Selected"); }
        }

        public bool Num3Selected
        {
            get { return mNum3Selected; }
            set { mNum3Selected = value; OnPropertyChanged("Num3Selected"); }
        }

        public bool Num4Selected
        {
            get { return mNum4Selected; }
            set { mNum4Selected = value; OnPropertyChanged("Num4Selected"); }
        }

        public bool Num5Selected
        {
            get { return mNum5Selected; }
            set { mNum5Selected = value; OnPropertyChanged("Num5Selected"); }
        }

        public bool Num6Selected
        {
            get { return mNum6Selected; }
            set { mNum6Selected = value; OnPropertyChanged("Num6Selected"); }
        }

        public bool Num7Selected
        {
            get { return mNum7Selected; }
            set { mNum7Selected = value; OnPropertyChanged("Num7Selected"); }
        }

        public bool Num8Selected
        {
            get { return mNum8Selected; }
            set { mNum8Selected = value; OnPropertyChanged("Num8Selected"); }
        }

        public bool Num9Selected
        {
            get { return mNum9Selected; }
            set { mNum9Selected = value; OnPropertyChanged("Num9Selected"); }
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
