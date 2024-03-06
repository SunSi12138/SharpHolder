# SharpHolder
这是一个用于生成占位图的C#服务器，使用SkiaSharp绘制图片。  网上的占位图网站不是关闭了就是速度太慢，遂自行写了这个小工具。  支持NativeAot，不用再安装依赖可直接运行

## 启动方式

- Linux或者macOS: `./SharpHolder`
- Windows: `./SharpHolder.exe`

默认监听地址：http://localhost:5000  
可以使用启动参数 `--urls {监听地址}` 指定监听IP。

## 图片链接

默认图片大小：1920x1080  
可以使用 `http://localhost:5000/placeholder?w={宽度}&h={高度}` 指定图片大小。

图片默认使用随机颜色，文字颜色为背景颜色的补色。  
可以使用 `http://localhost/placeholder?b={背景颜色}&f={文字颜色}` 指定颜色。
颜色格式使用3位或者6位HEX颜色。

图片默认显示文字为图片的大小（如1920x1080）。  
可以使用 `http://localhost/placeholder?t={文字}` 设置文字内容。

如果浏览器因为cache原因，可以自行添加一些随机参数来获取不同的图片。  
例如 `http://localhost/placeholder?r={随机id}`。

## 打包
请使用以下命令进行NativeAot打包：
`dotnet publish -c Release -r win-x64 /p:PublishAot=true`  
可选linux-x64，osx-arm64

| TODO | Description            |  Done |
|------|------------------------|-------|
| 1.   | 支持显示中文             |  ❌   |
| 2.   | 支持透明背景             |  ❌   |
| 3.   | 支持模拟延迟             |  ❌   |
| 4.   | 支持不同的图片格式        |  ❌   |
| 5.   | 添加不同平台的打包        |  ❌   |

