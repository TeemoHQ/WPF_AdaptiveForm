using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Remoting.Contexts;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Mvvm;
using WaitingRoomBigScreen.Common;
using WaitingRoomBigScreen.Model;
using WaitingRoomBigScreen.WebService;

namespace WaitingRoomBigScreen
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            Init();
        }
        private ObservableCollection<DeptInfo> _deptInfos = new ObservableCollection<DeptInfo>();
        private ObservableCollection<Content> _contents = new ObservableCollection<Content>();
        private string _waitingAreaName;
        private string _time;
        private string _title;
        private string _currentDeptType;
        private string _currentName;
        private string _currentNumber;
        private string _currentGuide;
        private List<Content> _currentPatientsData;
        private string _scrollMessage = "其他患者请在候诊大厅耐心等候，可通过自助查询终端查询实时排队信息";
        public bool IsOneLevel { get; set; }
        public ObservableCollection<DeptInfo> DeptInfos
        {
            get
            {
                return _deptInfos;

            }
            set
            {
                _deptInfos = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Content> Contents
        {
            get
            {
                return _contents;

            }
            set
            {
                _contents = value;
                OnPropertyChanged();
            }
        }
        public string WaitingAreaName
        {
            get
            {
                return _waitingAreaName;
            }
            set
            {
                _waitingAreaName = value;
                OnPropertyChanged();
            }
        }
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        public string CurrentDeptType
        {
            get
            {
                return _currentDeptType;
            }
            set
            {
                _currentDeptType = value;
                OnPropertyChanged();
            }
        }
        public string CurrentName
        {
            get
            {
                return _currentName;
            }
            set
            {
                _currentName = value;
                OnPropertyChanged();
            }
        }
        public string CurrentNumber
        {
            get
            {
                return _currentNumber;
            }
            set
            {
                _currentNumber = value;
                OnPropertyChanged();
            }
        }
        public string CurrentGuide
        {
            get
            {
                return _currentGuide;
            }
            set
            {
                _currentGuide = value;
                OnPropertyChanged();
            }
        }
        public List<Content> CurrentPatientsData
        {
            get
            {
                return _currentPatientsData;
            }
            set
            {
                _currentPatientsData = value;
                OnPropertyChanged();
            }
        }
        public string ScrollMessage
        {
            get
            {
                return _scrollMessage;
            }
            set
            {
                _scrollMessage = value;
                OnPropertyChanged();
            }
        }
        private void Init()
        {
            SetTime();
            GetData();
            for (int i = 0; i < 3; i++)
            {
                DeptInfos.Add(new DeptInfo
                {
                    DeptType = "甲状腺乳腺外科",
                    Level = "（普通）",
                });
                DeptInfos.Add(new DeptInfo
                {
                    DeptType = "神经外科",
                    Level = "（专家）",
                });

            }
            for (int i = 0; i < 3; i++)
            {
                Contents.Add(new Content
                {
                    PatientName = "周驰书",
                    Number = "7号",
                    DeptName = "乳腺科2诊室",
                    DeptType = "甲状腺乳腺外科",
                    TwoLevelData = new List<Content> { new Content { PatientName = "周驰书", Number = "7号" }, new Content { PatientName = "李中南", Number = "7号" }, new Content { PatientName = "山本耀司", Number = "7号" } }
                });
                Contents.Add(new Content
                {
                    PatientName = "张三丰",
                    Number = "8号",
                    DeptName = "乳腺科2诊室阿萨德",
                    DeptType = "甲状腺乳腺外科",
                    TwoLevelData = new List<Content> { new Content { PatientName = "张三丰", Number = "8号" }, new Content { PatientName = "Justinbieber", Number = "8号" }, new Content { PatientName = "张三丰", Number = "8号" }, new Content { PatientName = "张三丰", Number = "8号" }, new Content { PatientName = "张三丰", Number = "8号" }, new Content { PatientName = "张三丰", Number = "8号" }, new Content { PatientName = "张三丰", Number = "8号" } }
                });
            }
          

            WaitingAreaName = "外科门诊集中候诊大厅";
            Title = IsOneLevel ? "请下列患者前往指定诊室就诊" : "请以下患者➨前往分诊台自助签到";
            CurrentDeptType = Contents?.FirstOrDefault()?.DeptType;
            CurrentName = Contents?.FirstOrDefault()?.PatientName;
            CurrentNumber = Contents?.FirstOrDefault()?.Number;
            CurrentGuide = $"请前往{Contents?.FirstOrDefault()?.DeptName}就诊";
            CurrentPatientsData = Contents?.FirstOrDefault()?.TwoLevelData;
        }
        private async void GetData()
        {
            //1.注册 2.获取密码 3.获取token 4.获取数据
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserData","Config.xml");
            var config = FileUtil.XMLLoadData<Config>(path);
            this.IsOneLevel = config.IsOneLevel;
            DataHandler datahandler=new DataHandler();
            datahandler.Uri = new Uri(config.url);
            var service = new WebService.WebService(datahandler);

            var mac = GetLocalMac();
            var reqInit = new ReqInitDevie {deviceMac = mac };
            var resInit =await service.GetInitDevie(reqInit);
            if (resInit.IsSuccess)
            {
                var reqGetSecret = new ReqGetSecret { deviceMac = mac };
                var resGetSecret = await service.GetGetSecret(reqGetSecret);
                if (resGetSecret.IsSuccess)
                {
                    var reqToken = new ReqToken
                    {
                        deviceSecret = resGetSecret.Value
                    };
                    var resToken = await service.GetToken(reqToken);
                    if (resToken.IsSuccess)
                    {
                        var reqQueue = new ReqQueue { corpId = config.corpId,token = resToken.Value };
                        var resQueue = await service.GetQueue(reqQueue);
                        if (resQueue.IsSuccess)
                        {
                            //Contents =resQueue.Value;
                        }
                    }
                }
            }
        }
        private void SetTime()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    this.Time = DateTime.Now.ToString("MM月dd日 hh:mm:ss");
                }
            });
        }
        private string GetLocalMac()
        {
            string mac = null;
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                    mac = mo["MacAddress"].ToString();
            }
            return (mac);
        }

    }

}
