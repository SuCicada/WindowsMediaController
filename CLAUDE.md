# 需求
## 技术：
- 运行平台windows
- ASP.NET Core

## 功能
媒体控制 - PostMessageW
	Play/Pause, Stop, Next, Previous


音量控制 - IAudioEndpointVolume
	Get/Set Volume, Mute/Unmute
获取媒体播放状态:
    如果windows的播放器注册到系统媒体会话
	可以检索播放状态、音量、当前曲目、封面等
开机自启动

系统托盘图标 + 右键菜单
	退出
	About

配置管理：通过配置文件
    port: 8080
    autoStart: true
    logLevel: Information
    logFile: path

 
Web API - ASP.NET Core

日志管理：
Serilog：File Sink
Event Viewer

防止多开
优雅退出

## API
✅ 媒体控制 API
 - GET /api/media/status : 获取媒体播放信息
  - POST /api/media/play-pause
  - POST /api/media/stop  
  - POST /api/media/next
  - POST /api/media/previous

✅ 音量控制 API
  - GET  /api/volume
  - POST /api/volume/set/{level}
  - POST /api/volume/up
  - POST /api/volume/down
  - POST /api/volume/mute

 /swagger： 自动生成的Swagger API 

API 响应格式统一
json// 成功响应
{
  "success": true,
  "data": { "volume": 0.5 },
  "message": "操作成功"
}

// 错误响应
{
  "success": false,
  "error": "未找到音频设备",
  "code": "NO_AUDIO_DEVICE"
}



## 系统托盘
  - 图标
  - 右键菜单：
    ├── 打开设置文件目录（explorer）
    ├── ───
    ├── About（版本、GitHub）
    └── 退出

