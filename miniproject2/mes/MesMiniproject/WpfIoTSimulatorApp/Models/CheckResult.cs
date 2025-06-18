using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WpfIoTSimulatorApp.Models
{
   public class CheckResult
    {
        public string ClientId { get; set; } // MQTT 클라이언트 ID
        public string Timestamp { get; set; } // 타임스탬프
        public string Result { get; set; } // 검사 결과 (OK, NG)
    }
}
