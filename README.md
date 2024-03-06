# SharpHolder

这是一个用于生成占位图的C#服务器，使用SkiaShapr绘制图片
启动方式：
./SharpHolder #Linux或者macos
./SharpHolder.exe #windows

默认监听http://localhost:5000
可以使用启动参数"--urls {监听地址}"指定监听ip

图片链接http://localhost:5000/placeholder

默认图片大小1920*1080
可以使用http://localhost:5000/placeholder?w={宽度}&h={高度}指定大小

图片默认使用随机颜色，文字颜色为背景颜色的补色
可以使用http://localhost/placeholder?b={背景颜色}&f={文字颜色}
颜色格式使用3位或者6位HEX颜色

图片默认显示文字为图片的大小（如1920x1080）
可以使用http://localhost/placeholder?t={文字}设置文字内容

如果浏览器因为cache原因，可以自行添加一些随机参数来获取不同的图片
例如http://localhost/placeholder?r={随机id}