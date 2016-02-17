# xZune.Visualizer
Zune style audio visualizer.   
Now we support FFT sample data of [BASS](http://www.un4seen.com/) and [xZune.Bass](https://github.com/higankanshi/xZune.Bass).

Zune 风格的音频可视化控件。  
现在我们支持 [BASS](http://www.un4seen.com/) 与 [xZune.Bass](https://github.com/higankanshi/xZune.Bass) 的 FFT 采样数据。

# Sample
![xZune](http://higan.me/xZune.png)

## xZune Media Suit  

**[xZune.Bass](https://github.com/higankanshi/xZune.Bass)**  
xZune.Bass 是 [Bass](http://www.un4seen.com/bass.html) 库的 .NET 封装实现，用于多种格式的音频播放与解码。  
xZune.Bass is a Bass library wrapper for .NET, used to play/decode mutil format audio.   

**[xZune.Vlc](https://github.com/higankanshi/xZune.Vlc)**  
xZune.Vlc 是一个 LibVlc 封装库的 .NET 实现,封装了大部分的 LibVlc 的功能,该项目主要是为了寻求一个在 WPF 上使用 Vlc 的完美的解决方案,xZune.Vlc 提供一个原生的 WPF 播放控件(xZune.Vlc.Wpf),该控件采用 InteropBitmap 与共享内存进行播放视频,是一个原生的 WPF 控件,不存在 HwndHost 的空域问题.  
_xZune.Vlc is an LibVlc solution for .NET, it has encapsulated most of functionalities of LibVlc. This project aims to find a perfect solution for using Vlc on WPF. xZune.Vlc provides an native WPF control(xZune.Vlc.Wpf), this control achieves video playback by utilizing InteropBitmap and shared memory. Since it’s a native WPF control, it doesn't suffer from HwndHost’s airspace issue._  