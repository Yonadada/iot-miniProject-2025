namespace WpfIoTSimulatorApp.Models
{
    //json용 클래스이고, 다른곳에는 사용하지 않음
    public class PrcMsg
    {
        public string ClientId { get; set; }

        public string PlantCode { get; set; }

        public string FaclityId { get; set; }

        public string Timestamp { get; set; }

        public string Flag { get; set; }

    }
}
