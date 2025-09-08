using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MQTTnet;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Threading;
using WpfMqttSubApp.Models;

namespace WpfMqttSubApp.ViewModels
{
    // 클래스 구조와 상속
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        #region 내부 멤버변수

        private IMqttClient mqttClient;
        private MqttClientOptions mqttClientOptions;
        private readonly IDialogCoordinator dialogCoordinator;
        private readonly DispatcherTimer timer;
        private int lineCounter = 1;  // TODO : 나중에 텍스트가 너무 많아져서 느려지면 초기화시 사용

        private string connString = string.Empty;
        private MySqlConnection connection;
        private string mqttTopic;
        private string ClientId;

        #endregion

        #region MVVM용 멤버변수

        private string _brokerHost;
        private string _databaseHost;
        private string _logText;

        // 속성 BrokerHost, DatabaseHost
        // 메서드 ConnectBrokerCommand, ConnectDatabaseCommand       
        #endregion

        // 생성자
        public MainViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator;

            BrokerHost = App.Configuration.Mqtt.Broker;
            DatabaseHost = App.Configuration.Database.Server;
            mqttTopic = App.Configuration.Mqtt.Topic; // 설정파일로 작업 가능
            ClientId = App.Configuration.Mqtt.ClientId; // MQTT 클라이언트 ID

            connection = new MySqlConnection();  // 예외처리용 

            // RichTextBox 테스트용. 
            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(1);
            //timer.Tick += (sender, e) =>
            //{
            //    // RichTextBox 추가 내용
            //    LogText += $"Log [{DateTime.Now:HH:mm:ss}] - {counter++}\n";
            //    Debug.WriteLine($"Log [{DateTime.Now:HH:mm:ss}] - {counter++}");
            //};
            //timer.Start();
        }

        #region MVVM 속성들

        public string LogText
        {
            get => _logText;
            set => SetProperty(ref _logText, value);
        }

        public string BrokerHost
        {
            get => _brokerHost;
            set => SetProperty(ref _brokerHost, value);
        }

        public string DatabaseHost
        {
            get => _databaseHost;
            set => SetProperty(ref _databaseHost, value);
        }
        #endregion

        // MQTT 브로커 접속 및 구독처리 메서드
        private async Task ConnectMqttBroker()
        {
            // MQTT 클라이언트 생성
            var mqttFactory = new MqttClientFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            // MQTT 클라이언트접속 설정
            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(BrokerHost)
                //.WithClientId(ClientId) // 구독 시스템도 MQTT 클라이언트 ID가 필요할 수 있음
                .WithCleanSession(true)
                .Build();

            // mqtt publisher에서 받은 데이터를 처리하는 이벤트 핸들러 등록
            //이벤트 핸들러
            mqttClient.ConnectedAsync += async e =>
            {
                LogText += "MQTT 브로커 접속성공!\n";

                await mqttClient.SubscribeAsync(mqttTopic);
                LogText += $"{mqttTopic} 토픽 구독 완료!\n";

            };

            // 왜 실제로 메시지를 받는 곳이 Subscriber 쪽인가?
            // 시뮬에서는 메시지를 보냈다고(발행 로그)만 보내고, 실제 메시지를 받는 것은 Subscriber 쪽에서 처리(수신 로그)
            // MQTT 구독메시지 로그출력
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString(); // byte 데이터를 UTF-8 문자열로 변환

                // 강제로 예외 발생시키기
                //if (data.ClientId == null)
                //{
                //    throw new ArgumentNullException("ClientId가 null입니다");
                //}
                //return Task.CompletedTask;

                //---- JSON 파싱 이유 ----
                // 1. 구조화된 데이터 처리 2. 문자열 조작의 위험을 줄이고 안전하게 객체에 접근 

                //---- 역직렬화는 ----
                // 객체   -> JSON 문자열 : 직렬화(Serialization) 메모리 객체를 네트워크로 전송 가능한 문자열로 변환
                // JSON  -> 객체 : 역직렬화(Deserialization)

                // JSNON 파싱 예외처리
                try
                {
                    // json 데이터를 일반 객체로 다시 변환 -> 역직렬화(Deserialization)
                    var data = JsonConvert.DeserializeObject<CheckResult>(payload); //JSON으로 파싱된 데이터를 CheckResult 객체로 변환
                    Debug.WriteLine($"{data.ClientId} / {data.Timestamp} / {data.Result}");

                    //SaveSensingData(data);

                    LogText += $"LINENUMBER : {lineCounter++}\n";
                    LogText += $"{payload}\n";
                }
                catch (Exception ex)
                {
                    LogText += $"메시지 처리 오류: {ex.Message}\n";
                    LogText += $"원본 데이터: {payload}\n";
                }

                return Task.CompletedTask;
            };

            // mqtt Subscriber 접속이 끊겼을 때 이벤트 핸들러
            mqttClient.DisconnectedAsync += async e =>
            {
                LogText += "MQTT 브로커 접속이 끊겼습니다. 5초 후 재연결 시도...\n";
                // Publisher쪽에서 재연결을 담당하므로 여기서는 단순 알림만
            };

            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions);
            }
            catch (Exception ex)
            {
                LogText += $"MQTT 접속 실패: {ex.Message}\n";
            }


        }

        //db저장 메서드
        private async Task SaveSensingData(FakeInfo data)
        {
            string query = @"INSERT INTO fakedatas
                                    (sensing_dt, pub_id, count,
                                     temp, humid, light, human)
                            VALUES
                                    (@sensing_dt, @pub_id, @count, 
                                     @temp, @humid, @light, @human)";

            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    using var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@sensing_dt", data.Sensing_Dt);
                    cmd.Parameters.AddWithValue("@pub_id", data.Pub_Id);
                    cmd.Parameters.AddWithValue("@count", data.Count);
                    cmd.Parameters.AddWithValue("@temp", data.Temp);
                    cmd.Parameters.AddWithValue("@humid", data.Humid);
                    cmd.Parameters.AddWithValue("@light", data.Light);
                    cmd.Parameters.AddWithValue("@human", data.Human);

                    int result = await cmd.ExecuteNonQueryAsync();
                    //await cmd.ExecuteNonQueryAsync(); // 이전까지는 cmd.ExecuteNonQuery()
                    Debug.WriteLine($"DB 저장 결과 : {result}행 삽임됨");
                }
                else
                {
                    Debug.WriteLine("DB서버에 접속되지 않음");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB저장 오류 : {ex.Message}");
                LogText += $"DB저장 오류 : {ex.Message}\n";
                // TODO : 아무 예외처리 안해도 됨.
            }

        }

        private async Task ConnectDatabaseServer()
        {
            try
            {
                connection = new MySqlConnection(connString);
                connection.Open();
                LogText += $"{DatabaseHost} DB서버 접속성공! {connection.State}\n";

            }
            catch (Exception ex)
            {
                LogText += $"{DatabaseHost} DB서버 접속실패 : {ex.Message}\n";
            }
        }


        [RelayCommand]
        public async Task ConnectBroker()
        {
            if (string.IsNullOrEmpty(BrokerHost))
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "브로커연결", "브로커호스트를 입력하세요");
                return;
            }

            // 이미 연결된 상태인지 확인
            if(mqttClient != null && mqttClient.IsConnected)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "브로커연결", "이미 연결된 상태입니다");
                return;
            }

            // 【간소화】 기존 클라이언트 해제 로직 단순화
            mqttClient?.Dispose();
            mqttClient = null;

            // MQTT브로커에 접속해서 데이터를 가져오기
            await ConnectMqttBroker();
        }

          
        [RelayCommand]
        public async Task ConnectDatabase()
        {
            if (string.IsNullOrEmpty(DatabaseHost))
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "DB연결", "DB호스트를 입력하세요");
                return;
            }

            connString = $"Server={DatabaseHost};" + $"Database={App.Configuration.Database.Database};" +
                        $"Uid={App.Configuration.Database.User};" + $"Pwd={App.Configuration.Database.Password};" + "Charset=utf8";


            await ConnectDatabaseServer();
        }

        public void Dispose()
        {
            // 리소스 해제를 명시적으로 처리하는 기능 추가
            connection?.Close(); // DB접속 해제
        }
    }
}
