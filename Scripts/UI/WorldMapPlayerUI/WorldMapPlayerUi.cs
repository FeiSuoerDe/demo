using Godot;

namespace TimelapseInvoices.Scripts.UI.WorldMapPlayerUI;

public partial class WorldMapPlayerUi : Control
{
    // 音乐播放模块
    // 歌曲名称lab
    public Label musicName;
    [Export]
    // 下一个按钮
    public Button nextButton;
    [Export]
    // 上一个按钮
    public Button previousButton;
    [Export]
    // 播放按钮
    public Button playButton;
    // 音乐播放节点
    [Export]
    public AudioStreamPlayer musicPlayer;



}