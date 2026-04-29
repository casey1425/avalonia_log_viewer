using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace csharp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _ipAddress = "127.0.0.1";

    [ObservableProperty]
    private string _port = "8080";

    [ObservableProperty]
    private string _messageToSend = "";

    // 화면의 리스트박스와 연결될 관찰 가능한 리스트.
    public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();

    public MainWindowViewModel()
    {
        LogMessages.Add("프로그램이 시작되었습니다. 연결을 준비하세요.");
    }

    // [RelayCommand]를 붙이면 화면의 버튼 클릭(Command)과 연결할 수 있습니다.
    [RelayCommand]
    private void Connect()
    {
        LogMessages.Add($"[{IpAddress}:{Port}] 연결을 시도합니다...");
        // 나중에 여기에 실제 네트워크 연결 코드가 들어감.
    }

    [RelayCommand]
    private void Disconnect()
    {
        LogMessages.Add("연결이 안전하게 해제되었습니다.");
    }

    [RelayCommand]
    private void Send()
    {
        // 빈 메시지가 아닐 때만 전송
        if (!string.IsNullOrWhiteSpace(MessageToSend))
        {
            LogMessages.Add($"[송신] {MessageToSend}");
            MessageToSend = ""; // 전송 후 입력창을 깔끔하게 비워줍니다.
        }
    }
}