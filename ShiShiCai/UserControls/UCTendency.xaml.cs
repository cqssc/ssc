//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    7fb9d498-41b5-40a7-9e7b-5191dc3c6a3b
//        CLR Version:              4.0.30319.42000
//        Name:                     UCTendency
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCTendency
//
//        Created by Charley at 2019/8/23 9:12:22
//        http://www.netinfo.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ShiShiCai.Common;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCTendency.xaml 的交互逻辑
    /// </summary>
    public partial class UCTendency : IModuleView
    {

        #region Property

        public static readonly DependencyProperty PageParentProperty =
            DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCTendency), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

        public ObservableCollection<DateItem> DateItems
        {
            get { return mListDateItems; }
        }

        public ObservableCollection<TendencyTypeItem> TypeItems
        {
            get { return mListTypeItems; }
        }

        public ObservableCollection<TendencyItem> TendencyItems
        {
            get { return mListTendencyItems; }
        }

        #endregion


        #region Members

        private readonly List<TendencyItem> mListTendencyData = new List<TendencyItem>();
        private readonly ObservableCollection<DateItem> mListDateItems = new ObservableCollection<DateItem>();
        private readonly ObservableCollection<TendencyTypeItem> mListTypeItems = new ObservableCollection<TendencyTypeItem>();
        private readonly ObservableCollection<TendencyItem> mListTendencyItems = new ObservableCollection<TendencyItem>();

        private bool mIsInited;

        #endregion


        public UCTendency()
        {
            InitializeComponent();

            Loaded += UCTendency_Loaded;
            ComboDate.SelectionChanged += ComboDate_SelectionChanged;
        }

        void UCTendency_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and Load

        public void Reload()
        {
            Init();
        }

        private void Init()
        {
            InitTypesItems();
            InitDateItems();
            LoadTendencyData();

            ComboDate.SelectedItem = mListDateItems.FirstOrDefault();
        }

        private void InitDateItems()
        {
            mListDateItems.Clear();
            var pageParent = PageParent;
            if (pageParent == null) { return; }
            var issueItems = pageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var dateGroups = issueItems.GroupBy(g => g.Date);
            foreach (var dateGroup in dateGroups)
            {
                int date = dateGroup.Key;
                DateItem item = new DateItem();
                item.Date = date;
                mListDateItems.Add(item);
            }
        }

        private void InitTypesItems()
        {
            mListTypeItems.Clear();
            TendencyTypeItem item = new TendencyTypeItem();
            item.Number = 1;
            item.Name = "重复";
            item.Color = Brushes.DarkRed;
            item.IsChecked = true;
            mListTypeItems.Add(item);
            item = new TendencyTypeItem();
            item.Number = 2;
            item.Name = "振荡";
            item.Color = Brushes.DarkGreen;
            mListTypeItems.Add(item);
            item = new TendencyTypeItem();
            item.Number = 3;
            item.Name = "递增减";
            item.Color = Brushes.DarkOrange;
            mListTypeItems.Add(item);
            item = new TendencyTypeItem();
            item.Number = 4;
            item.Name = "其他";
            item.Color = Brushes.DarkBlue;
            mListTypeItems.Add(item);
        }

        private void LoadTendencyData()
        {
            mListTendencyData.Clear();
            if (PageParent == null) { return; }
            var issueItems = PageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var systemConfig = PageParent.SystemConfig;
            if (systemConfig == null) { return; }
            var db = systemConfig.Database;
            if (db == null) { return; }
            string strConn = db.GetConnectionString();
            var first = issueItems.FirstOrDefault();
            if (first == null) { return; }
            long begin = long.Parse(first.Serial);
            long end = long.Parse(first.Serial);
            for (int i = 0; i < issueItems.Count; i++)
            {
                var item = issueItems[i];
                long serial = long.Parse(item.Serial);
                begin = Math.Min(serial, begin);
                end = Math.Max(serial, end);
            }
            string strSql = string.Format(
                "SELECT * FROM T_103_19 WHERE C001 >= {0} AND C001 <= {1} ORDER BY C001, C004", begin, end);
            OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
            if (!optReturn.Result)
            {
                ShowException(string.Format("load number hot data fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                return;
            }
            DataSet objDataSet = optReturn.Data as DataSet;
            if (objDataSet == null) { return; }
            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
            {
                DataRow dr = objDataSet.Tables[0].Rows[i];
                TendencyItem item = new TendencyItem();
                item.Serial = dr["C001"].ToString();
                item.Date = Convert.ToInt32(dr["C002"]);
                item.Number = Convert.ToInt32(dr["C003"]);
                item.Pos = Convert.ToInt32(dr["C004"]);
                item.Repeat = dr["C005"].ToString() == "1";
                item.Osillation = dr["C006"].ToString() == "1";
                item.Increase = dr["C007"].ToString() == "1";
                item.Other = dr["C008"].ToString() == "1";
                item.Times = Convert.ToInt32(dr["C009"]);
                item.Range = Convert.ToInt32(dr["C010"]);
                mListTendencyData.Add(item);
            }
        }

        private void InitTendencyItems()
        {
            mListTendencyItems.Clear();
            var dateItem = ComboDate.SelectedItem as NumberHotDateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;

        }

        #endregion


        #region 事件处理

        void ComboDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitTendencyItems();
        }

        #endregion


        #region Basic

        private void ShowException(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error));

        }

        private void ShowInfomation(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information));
        }

        #endregion

    }
}
