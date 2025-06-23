using MahApps.Metro.Controls.Dialogs;

namespace WpfMrpSimulatorApp.Helpers
{
    /// <summary>
    /// 프로젝트 내에서 공통으로 사용하는 정적 클래스
    /// 클래스 자체가 static일 필요는 없지만 사용할 변수들이 static으로 선언되어 있어야 함!!!!!!!
    /// </summary>
    public class Common
    {
        public static readonly string CONNSTR = "Server=localhost;Database=miniproject;Uid=root;Password=12345;Charset=utf8";

        //MahApps.Metro 다이얼로그 코디네이터(MVVM에서 사용하기 위해서 !)
        public static IDialogCoordinator DIALOGCOORDINATOR;
    }
}
