using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WpfIoTSimulatorApp.ViewModels;

namespace WpfIoTSimulatorApp.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : MetroWindow
    {
        private readonly MainViewModel _viewModel;
        public MainView()
        {
            InitializeComponent();

        }

        
        // 뷰 상에 있는 이벤트핸들러를 전부 제거
        // WPF 상의 객체 애니메이션 추가
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
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true
            };

            SortingSensor.BeginAnimation(OpacityProperty, sa);


            //Thread.Sleep(2000); // 1초 딜레이
            //// 랜덤으로 색상을 결정짓는 작업
            //Random rand = new Random();
            //int result = rand.Next(1, 3); // 1 ~ 3 중 하나 선별

            //switch (result)
            //{
            //    case 1:
            //        Product.Fill = new SolidColorBrush(Colors.Green); // 양품
            //        break;
            //    case 2:
            //        Product.Fill = new SolidColorBrush(Colors.Crimson); // 불량
            //        break;
            //    case 3:
            //        Product.Fill = new SolidColorBrush(Colors.Gray); // 선별 실패
            //        break;
            //}
        }
    }
}
