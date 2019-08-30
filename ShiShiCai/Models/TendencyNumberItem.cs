//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    519193b6-970e-44f7-a4dc-c33a70c0dead
//        CLR Version:              4.0.30319.42000
//        Name:                     TendencyNumberItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                TendencyNumberItem
//
//        Created by Charley at 2019/8/26 16:13:38
//        http://www.netinfo.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;


namespace ShiShiCai.Models
{
    public class TendencyNumberItem : INotifyPropertyChanged
    {
        private string mSerial;
        private int mNumber;
        private int mDate;

        private int mD1Range;
        private int mD2Range;
        private int mD3Range;
        private int mD4Range;
        private int mD5Range;

        private double mD1Height;
        private double mD2Height;
        private double mD3Height;
        private double mD4Height;
        private double mD5Height;

        private Brush mD1Color;
        private Brush mD2Color;
        private Brush mD3Color;
        private Brush mD4Color;
        private Brush mD5Color;

        private bool mD1Visible;
        private bool mD2Visible;
        private bool mD3Visible;
        private bool mD4Visible;
        private bool mD5Visible;

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

        public int D1Range
        {
            get { return mD1Range; }
            set { mD1Range = value; OnPropertyChanged("D1Range"); }
        }

        public int D2Range
        {
            get { return mD2Range; }
            set { mD2Range = value; OnPropertyChanged("D2Range"); }
        }

        public int D3Range
        {
            get { return mD3Range; }
            set { mD3Range = value; OnPropertyChanged("D3Range"); }
        }

        public int D4Range
        {
            get { return mD4Range; }
            set { mD4Range = value; OnPropertyChanged("D4Range"); }
        }

        public int D5Range
        {
            get { return mD5Range; }
            set { mD5Range = value; OnPropertyChanged("D5Range"); }
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

        public Brush D1Color
        {
            get { return mD1Color; }
            set { mD1Color = value; OnPropertyChanged("D1Color"); }
        }

        public Brush D2Color
        {
            get { return mD2Color; }
            set { mD2Color = value; OnPropertyChanged("D2Color"); }
        }

        public Brush D3Color
        {
            get { return mD3Color; }
            set { mD3Color = value; OnPropertyChanged("D3Color"); }
        }

        public Brush D4Color
        {
            get { return mD4Color; }
            set { mD4Color = value; OnPropertyChanged("D4Color"); }
        }

        public Brush D5Color
        {
            get { return mD5Color; }
            set { mD5Color = value; OnPropertyChanged("D5Color"); }
        }

        public bool D1Visible
        {
            get { return mD1Visible; }
            set { mD1Visible = value; OnPropertyChanged("D1Visible"); }
        }

        public bool D2Visible
        {
            get { return mD2Visible; }
            set { mD2Visible = value; OnPropertyChanged("D2Visible"); }
        }

        public bool D3Visible
        {
            get { return mD3Visible; }
            set { mD3Visible = value; OnPropertyChanged("D3Visible"); }
        }

        public bool D4Visible
        {
            get { return mD4Visible; }
            set { mD4Visible = value; OnPropertyChanged("D4Visible"); }
        }

        public bool D5Visible
        {
            get { return mD5Visible; }
            set { mD5Visible = value; OnPropertyChanged("D5Visible"); }
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
