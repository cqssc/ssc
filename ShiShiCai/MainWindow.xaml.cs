using System.ComponentModel;
using System.Windows;

namespace ShiShiCai
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private bool mIsInited;

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

        private void Init()
        {
            WindowState = WindowState.Maximized;
        }
    }
}
