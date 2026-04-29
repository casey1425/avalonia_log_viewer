using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
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

    public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();

    // 진짜 네트워크 통신을 위한 객체들
    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;

    public MainWindowViewModel()
    {
        LogMessages.Add("프로그램이 시작되었습니다. 연결을 준비하세요.");
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (_tcpClient != null && _tcpClient.Connected)
        {
            LogMessages.Add("이미 연결되어 있습니다.");
            return;
        }

        LogMessages.Add($"[{IpAddress}:{Port}] 서버로 연결을 시도합니다...");

        try
        {
            _tcpClient = new TcpClient();
            int portNumber = int.Parse(Port); // 입력된 포트를 숫자로 변환

            // 🌟 진짜 인터넷 망을 타고 서버에 연결을 시도합니다!
            await _tcpClient.ConnectAsync(IpAddress, portNumber);
            _networkStream = _tcpClient.GetStream();

            LogMessages.Add("✅ 연결에 성공했습니다!");
            
            // 연결에 성공하면 상대방이 보내는 메시지를 계속 기다리는 작업을 시작.
            _ = ReceiveMessagesAsync();
        }
        catch (Exception ex)
        {
            // 연결할 서버가 없는경우
            LogMessages.Add($"❌ 연결 실패: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Disconnect()
    {
        _networkStream?.Close();
        _tcpClient?.Close();
        _tcpClient = null;
        LogMessages.Add("연결이 안전하게 해제되었습니다.");
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(MessageToSend)) return;

        if (_tcpClient == null || !_tcpClient.Connected || _networkStream == null)
        {
            LogMessages.Add("⚠️ 먼저 서버에 연결해 주세요.");
            return;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(MessageToSend);
            await _networkStream.WriteAsync(data, 0, data.Length);
            
            LogMessages.Add($"[송신] {MessageToSend}");
            MessageToSend = ""; 
        }
        catch (Exception ex)
        {
            LogMessages.Add($"❌ 전송 실패: {ex.Message}");
        }
    }

    private async Task ReceiveMessagesAsync()
    {
        if (_networkStream == null) return;
        byte[] buffer = new byte[1024]; // 데이터를 받을 바구니

        try
        {
            while (_tcpClient != null && _tcpClient.Connected)
            {
                // 상대방이 데이터를 보낼 때까지 여기서 대기.
                int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // 0바이트면 연결이 끊어짐

                string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Dispatcher.UIThread.Post(() => 
                {
                    LogMessages.Add($"[수신] {receivedText}");
                });
            }
        }
        catch
        {
            // 통신 중단 예외 무시
        }
        finally
        {
            Dispatcher.UIThread.Post(() => LogMessages.Add("서버와의 연결이 끊어졌습니다."));
            Disconnect();
        }
    }
}