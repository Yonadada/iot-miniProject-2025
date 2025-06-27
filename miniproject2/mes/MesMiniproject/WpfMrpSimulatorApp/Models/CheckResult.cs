namespace WpfMrpSimulatorApp.Models
{
    //json용 클래스이고, 다른곳에는 사용하지 않음. MQTT 메시지로 검사 결과를 전송하기 위한 클래스
    public class CheckResult
    {
        public string ClientId { get; set; } // MQTT 클라이언트 ID
        public string Timestamp { get; set; } // 타임스탬프
        public string Result { get; set; } // 검사 결과 (OK, NG)
    }
}
