using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SwitchSM
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // レジストリの状態を取得
            bool isEmptyRegistry = LoadReg();

            // レジストリが存在する場合
            if (isEmptyRegistry)
            {
                // レジストリキーを取得
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", false);
                // 値を取得 
                int Start_ShowClassicMode = (int)regkey.GetValue("Start_ShowClassicMode");
                regkey.Close();
                // ONの場合
                if (Start_ShowClassicMode == 1)
                {
                    // OFFを描画
                    loadButton_OFF();
                }
                // OFFの場合
                else
                {
                    // ONを描画
                    loadButton_ON();
                }

            }
            // レジストリが存在しない場合
            else
            {
                // ONを描画
                loadButton_ON();
            }
            Console.WriteLine(isEmptyRegistry);
        }


        private void loadButton_ON()
        {
            button1.Background = Brushes.Green;
            button1.Content = "ON";
            button1.Tag = 1;
            osLabel.Content = "Now: Windows 11";
            message1.Text = "現在はWindows11のスタートメニューになっています。\n" +
                "Windows10のスタートメニューに変更する場合は「ON」をクリックしてください。";
            BitmapImage bi = new BitmapImage(new Uri("pack://application:,,,/images/w11.png"));
            this.logo.Source = bi;

        }

        private void loadButton_OFF()
        {

            button1.Background = Brushes.Red;
            button1.Content = "OFF";
            button1.Tag = 0;
            osLabel.Content = "Now: Windows 10";
            message1.Text = "現在はWindows10のスタートメニューになっています。\n" +
                "Windows11のスタートメニューに変更する場合は「OFF」をクリックしてください。";
            BitmapImage bi = new BitmapImage(new Uri("pack://application:,,,/images/w10.png"));
            this.logo.Source = bi;
        }

        private void logOff_Msg()
        {
            MessageBoxResult result = MessageBox.Show("変更を適用する為に一度ログオフし、再度ログインしてください。\n\nログオフするまで設定は有効になりません。",
                "メッセージ",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }




        private static bool LoadReg()
        {
            // コンピューター\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advancedを指定
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", false);

            // Start_ShowClassicModeが存在するかしないかをチェック
            var Start_ShowClassicMode = regkey.GetValue("Start_ShowClassicMode");
            // レジストリを閉じる
            regkey.Close();
            // 存在する場合
            if (Start_ShowClassicMode != null)
            {
                return true;
            }
            // 存在しない場合
            else
            {
                return false;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(button1.Tag);
            int tag = (int)button1.Tag;
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
            if (tag == 1)
            {
                Int32 writedata = BitConverter.ToInt32(BitConverter.GetBytes(0x00000001), 0);
                regkey.SetValue("Start_ShowClassicMode", writedata, Microsoft.Win32.RegistryValueKind.DWord);
                // OFFを描画
                loadButton_OFF();
                // ログオフメッセージを表示
                logOff_Msg();

            }
            else if (tag == 0)
            {
                //キーの値の削除
                regkey.DeleteValue("Start_ShowClassicMode");
                // ONを描画
                loadButton_ON();
                // ログオフメッセージを表示
                logOff_Msg();

            }

        }
    }
}
