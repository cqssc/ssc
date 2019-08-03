using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ShiShiCai
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private bool mIsInited;

        private ObservableCollection<ModuleItem> mListModuleItems = new ObservableCollection<ModuleItem>();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {

        }


        #region Init and load

        private void Init()
        {
            WindowState = WindowState.Maximized;
            ListBoxModules.ItemsSource = mListModuleItems;

            InitModuleItems();
        }

        private void InitModuleItems()
        {
            mListModuleItems.Clear();
            ModuleItem item = new ModuleItem();
            item.Number = SscDefines.MODULE_BASIC;
            item.Name = SscDefines.MODULE_NAME_BASIC;
            item.Title = SscDefines.MODULE_NAME_BASIC;
            item.Icon = "Themes/Default/Images/00001.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_LOST;
            item.Name = SscDefines.MODULE_NAME_LOST;
            item.Title = SscDefines.MODULE_NAME_LOST;
            item.Icon = "Themes/Default/Images/00002.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_HOT;
            item.Name = SscDefines.MODULE_NAME_HOT;
            item.Title = SscDefines.MODULE_NAME_HOT;
            item.Icon = "Themes/Default/Images/00003.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_TREND;
            item.Name = SscDefines.MODULE_NAME_TREND;
            item.Title = SscDefines.MODULE_NAME_TREND;
            item.Icon = "Themes/Default/Images/00004.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_HISTORY;
            item.Name = SscDefines.MODULE_NAME_HISTORY;
            item.Title = SscDefines.MODULE_NAME_HISTORY;
            item.Icon = "Themes/Default/Images/00005.png";
            mListModuleItems.Add(item);
        }

        #endregion


        #region Event Handlers



        #endregion


        #region basic

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfomation(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        #region Log

        private void WriteLog(string category, string msg)
        {
            var app = App.Current as App;
            if (app != null)
            {
                app.WriteLog(category, msg);
            }
        }

        private void WriteLog(string msg)
        {
            WriteLog("SSC", msg);
        }

        #endregion

    }
}
