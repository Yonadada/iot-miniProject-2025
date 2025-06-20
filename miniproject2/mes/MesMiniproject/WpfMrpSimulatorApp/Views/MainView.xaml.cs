using MahApps.Metro.Controls;

namespace WpfMrpSimulatorApp.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 창 닫기 이벤트 취소,
            e.Cancel = true; // X버튼을 눌러도 창이 닫히지 않도록 설정!
        }
    }
}
