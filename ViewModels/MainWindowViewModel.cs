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
            int portNumber = int.Parse(Port); 

            await _tcpClient.ConnectAsync(IpAddress, portNumber);
            _networkStream = _tcpClient.GetStream();

            LogMessages.Add("✅ 연결에 성공했습니다!");
            
            _ = ReceiveMessagesAsync();
        }
        catch (Exception ex)
        {
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
            string textToLog = MessageToSend; 
            string messageWithNewLine = MessageToSend + "\n";
            byte[] data = Encoding.UTF8.GetBytes(messageWithNewLine);
            
            await _networkStream.WriteAsync(data, 0, data.Length);
            LogMessages.Add($"[송신] {textToLog}");
            
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
        byte[] buffer = new byte[1024]; 

        try
        {
            while (_tcpClient != null && _tcpClient.Connected)
            {
                int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                
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