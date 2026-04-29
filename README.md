# 📡 통신 로그 뷰어 (TCP Log Viewer)

C#과 Avalonia UI를 활용하여 개발한 **크로스 플랫폼 데스크톱 TCP 클라이언트 애플리케이션**입니다. 
MVVM 디자인 패턴을 적용하여 UI와 비즈니스 로직을 완벽하게 분리하였으며, 비동기(Async) 네트워크 통신을 지원합니다.

## ✨ 주요 기능 (Features)
- **TCP/IP 소켓 통신:** 지정된 IP와 포트로 서버에 연결하여 데이터를 송수신합니다.
- **비동기 처리:** `async/await`를 활용하여 통신 중에도 UI가 멈추지 않고 부드럽게 작동합니다.
- **실시간 로그 모니터링:** 송신 및 수신된 메시지를 즉시 화면에 출력합니다.
- **자동 스크롤 (Auto-scroll):** 새로운 로그가 추가될 때마다 화면이 자동으로 맨 아래로 이동하여 편의성을 높였습니다.

## 🛠 기술 스택 (Tech Stack)
- **Language:** C# 
- **Framework:** .NET, Avalonia UI
- **Architecture:** MVVM (Model-View-ViewModel)
- **Library:** CommunityToolkit.Mvvm

## 🚀 실행 및 테스트 방법
1. 터미널에서 프로젝트 폴더로 이동한 후 아래 명령어를 실행합니다.
   ```bash
   dotnet run
