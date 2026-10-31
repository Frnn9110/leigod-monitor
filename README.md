# 雷神加速器监控

一个运行在 Windows 桌面右上角的雷神加速器状态监控工具。

## 功能

- 每 10 秒检测一次 `leigod.exe` 是否运行
- 显示“雷神：已开启”或“雷神：未开启”
- 透明背景、白色文字，窗口始终置顶
- 鼠标穿透，不影响其他窗口操作
- 常驻系统托盘
- 托盘右键菜单支持调整字体大小和字体颜色
- 可配置开机自动启动

## 使用方法

双击 `LeigodMonitor.exe` 即可运行。程序启动后会显示在主屏幕右上角。

右击系统托盘中的“雷神监控”图标，可以调整字体大小、字体颜色、开机自动启动，或退出程序。

## 编译

在 Windows PowerShell 中运行：

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

编译脚本使用 Windows 自带的 C# 编译器和 WinForms 组件，不需要安装 Python 或 .NET SDK。

## 兼容性说明

适用于 Windows 7/8/10/11（需要系统具备 .NET Framework 4.x）。程序通过检测 `leigod.exe` 判断雷神加速器是否开启。
