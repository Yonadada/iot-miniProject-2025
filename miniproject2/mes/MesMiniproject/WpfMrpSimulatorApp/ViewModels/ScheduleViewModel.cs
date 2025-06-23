using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls.Crypto;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using WpfMrpSimulatorApp.Helpers;
using WpfMrpSimulatorApp.Models;

namespace WpfMrpSimulatorApp.ViewModels
{
    public partial class ScheduleViewModel : ObservableObject
    {
        // readonly 생성자에서 할당하고 나면 그 이후에 값 변경 불가
        private readonly IDialogCoordinator dialogCoordinator;

        #region View와 연동할 멤버변수들

        private string _basicCode;
        private string _codeName;
        private string? _codeDesc;
        private DateTime? _regDt;
        private DateTime? _modDt;

        private ObservableCollection<Setting> _settings;
        private Setting _selectedSetting;
        private bool _isUpdate; // 신규인지 수정인지 구분하는 플래그 변수

        private bool _canSave; // 저장 가능 여부를 판단하는 플래그 변수
        private bool _canRemove; // 삭제 가능 여부를 판단하는 플래그 변수

        #endregion

        #region View와 연동할 속성

        public bool CanSave
        {
            get => _canSave;
            set => SetProperty(ref _canSave, value);
        }

        public bool CanRemove
        {
            get => _canRemove;
            set => SetProperty(ref _canRemove, value);
        }


        public bool IsUpdate
        {
            get => _isUpdate;
            set => SetProperty(ref _isUpdate, value);
        }

        // View와 연동될 데이터/컬렉션
        public ObservableCollection<Setting> Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public Setting SelectedSetting
        {
            get => _selectedSetting;
            set {
                SetProperty(ref _selectedSetting, value);
            
                //최초에 BasicCode에 값이 있는 상태만 수정상태
                if(SelectedSetting != null) // 삭제 후에는 _selectedSetting 자체가 null이 됨
                {
                    if(!string.IsNullOrEmpty(_selectedSetting.BasicCode))   
                    {
                        CanSave = true; 
                        CanRemove = true; 
                    }
                }
            }
        }

        /// <summary>
        /// 기본코드
        /// 
        /// </summary>
        public string BasicCode
        {
            get => _basicCode;
            set => SetProperty(ref _basicCode, value);
        }

        /// <summary>
        /// 코드명
        /// </summary>
        public string CodeName
        {
            get => _codeName;
            set => SetProperty(ref _codeName, value);
        }

        /// <summary>
        /// 코드 설명
        /// </summary>
        public string? CodeDesc
        {
            get => _codeDesc;
            set => SetProperty(ref _codeDesc, value);
        }

        public DateTime? RegDt
        {
            get => _regDt;
            set => SetProperty(ref _regDt, value);
        }

        public DateTime? ModDt
        {
            get => _modDt;
            set => SetProperty(ref _modDt, value);
        }
        #endregion

        public ScheduleViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator; //파라미터 값으로 초기화
            LoadGridFromDb(); // DB에서 그리드 데이터 로드해서 그리드에 출력
            IsUpdate = true;


            // 최초에는 저장 버튼, 삭제버튼이 비활성화
            CanSave = CanRemove = false; 
        }

        private async Task LoadGridFromDb()
        {
            try
            {
                string query = @"SELECT basicCode
                                      , codeName
                                      , codeDesc
                                      , regDt     
                                      , modDt  
                                FROM settings";

                ObservableCollection<Setting> settings = new ObservableCollection<Setting>();

                //DB  연결 방식 1
                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var basicCode = reader.GetString("basicCode");
                        var codeName = reader.GetString("codeName");
                        var codeDesc = reader.GetString("codeDesc");
                        var regDt = reader.GetDateTime("regDt");
                        //modDt는 최초에 입력후 항상 null. NULL타입 체크 필수~!
                        var modDt = reader.IsDBNull(reader.GetOrdinal("modDt")) ? (DateTime?)null : reader.GetDateTime("modDt");

                        settings.Add(new Setting
                        {
                            BasicCode = basicCode,
                            CodeName = codeName,
                            CodeDesc = codeDesc,
                            RegDt = regDt,
                            ModDt = modDt,
                        });
                    }
                }
                Settings = settings;
            }
            catch (Exception ex)
            {

                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
        }


        private void InitVariable()
        {
            SelectedSetting = new Setting();

            //IsUpdate 가 False 면 신규,  True면 수정
            IsUpdate = false;
        }

        #region View 버튼클릭 메서드

        [RelayCommand]
        public void NewData()
        {
            InitVariable();
            IsUpdate = false; // DoubleCheck. 확실하게 동작을 하면 지워도 되는 로직
            CanSave = true; // 저장 버튼 활성화
        }

        [RelayCommand]
        public async void SaveData()
        {
            // INSERT, UPDATE 기능을 모두 수행
            try
            {
                string query = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    if (IsUpdate)
                    {
                        query = "UPDATE settings SET codeName = @codeName," +
                                                                "codeDesc = @codeDesc," +
                                                                "modDt = now()" +
                                          "WHERE basicCode = @basicCode"; //UPDATE 쿼리

                    }
                    else
                    {
                        query = "INSERT INTO settings (basicCode, codeName, codeDesc, regDt)" +
                                 "VALUES (@basicCode, @codeName, @codeDesc, now());"; //INSERT 쿼리
                    
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue(@"basicCode", SelectedSetting.BasicCode); // 파라미터 추가
                    cmd.Parameters.AddWithValue(@"codeName", SelectedSetting.CodeName); 
                    cmd.Parameters.AddWithValue(@"codeDesc", SelectedSetting.CodeDesc);
                    
                    var resultCnt = cmd.ExecuteNonQuery(); // 쿼리 실행
                    if(resultCnt > 0)
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "기본 설정 저장", "데이터 저장 완료");

                    }
                    else
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "기본 설정 저장", "데이터 저장 실패");
                    }

                }
            }
            catch (Exception ex)
            {

                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            LoadGridFromDb(); // 재조회
            IsUpdate = true; // 다시 입력 안되도록 막기

        }
        [RelayCommand]
        public async Task RemoveData()
        {
            var result = await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative) return; // Enter 누르면 메서드 탈출
          
            try
            {
                string query = @"DELETE FROM settings WHERE basicCode = @basicCode";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@basicCode", SelectedSetting.BasicCode);

                    int resultCnt = cmd.ExecuteNonQuery(); // 한건이 삭제되면 쿼리 행수가 리턴, 안지워졌으면 0 리턴 지워졌으면 1

                   if(resultCnt == 1)
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "기본설정 삭제", "데이터가 삭제되었습니다");
                    }
                    else
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "기본설정 삭제", "데이터 삭제 과정 중 문제발생~!");
                    }
                }
            }
            catch (Exception ex)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
            
            LoadGridFromDb(); // DB를 다시 불러서 그리드에 출력 
        }

        #endregion
    }
}
