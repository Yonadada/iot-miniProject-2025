using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfMrpSimulatorApp.Views
{
    /// <summary>
    /// SettingView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitoringView : UserControl
    {
        public MonitoringView()
        {
            InitializeComponent();
        }

        public void StartHmiAni()
        {

            // 기어 애니메이션
            DoubleAnimation ga = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(2), // 계획 로드 타임(Schedules의 LoadTime 값이 들어가야 함)

            };

            RotateTransform rt = new RotateTransform();
            GearStart.RenderTransform = rt;
            GearStart.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            GearEnd.RenderTransform = rt;
            GearEnd.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, ga);

            // 제품 애니메이션
            DoubleAnimation pa = new DoubleAnimation
            {
                From = 127,
                To = 417, // x축 : 센서 아래 위치
                Duration = TimeSpan.FromSeconds(2),// 계획 로드 타임(Schedules의 LoadTime 값이 들어가야 함)
            }; // 이런 초기화가 조금 더 최신 트랜드에 맞는 코드

            // -- 구식 코딩 방법 --
            //pa.From = 127;
            //pa.To = 417; // x축 : 센서 아래 위치
            //pa.Duration = TimeSpan.FromSeconds(5); 
            //-----

            Product.BeginAnimation(Canvas.LeftProperty, pa);

        }


        public void StartSensorCheck()
        {
            // 센서 애니메이션
            DoubleAnimation sa = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true
            };

            SortingSensor.BeginAnimation(OpacityProperty, sa);


           
        }
    }
}
