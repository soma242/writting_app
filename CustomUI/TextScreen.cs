using PublishStructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using writting_app.MessageInstance;

namespace writting_app.CustomUI;

internal class TextScreen : SplitContainer
{
    private readonly int splitDistance = 40;

    private FlowLayoutPanel flow;

    public int screenIndex { get; private set; }

    public TextScreen()
    {
        screenIndex = GlobalScreenIndex.GetScreenIndex();

        InitializeSplitContainerSetting();

        flow = new FlowLayoutPanel();
        flow.Dock = DockStyle.Fill;
        Panel1.Controls.Add(flow);

        InitializeComboBox();


    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            //現在使用しているインデックスに追加
            GlobalScreenIndex.RemoveScreen(screenIndex);
        }

        base.Dispose(disposing);
    }
    private void InitializeSplitContainerSetting()
    {
        Orientation = Orientation.Horizontal;
        SplitterDistance = splitDistance;
        Panel1MinSize = splitDistance;
        FixedPanel = FixedPanel.Panel1;
        IsSplitterFixed = true;

    }

    private void InitializeComboBox()
    {
        ComboBox cmbFontSize = new ComboBox
        {
            Location = new System.Drawing.Point(0, 0),
            Width = 60,
            DropDownStyle = ComboBoxStyle.DropDownList // 選択のみ
        };

        // 選択肢（フォントサイズの例）
        float[] fontSizes = { 8f, 9f, 10f, 11f, 12f, 14f, 16f, 18f, 20f, 24f, 28f, 32f, 36f, 48f };
        foreach (var size in fontSizes)
        {
            cmbFontSize.Items.Add(size);
        }

        cmbFontSize.SelectedIndex = 5;

        // イベント登録
        cmbFontSize.SelectedIndexChanged += ComboBoxItemChanged;

        flow.Controls.Add(cmbFontSize);
    }

    private void ComboBoxItemChanged(object? sender, EventArgs e)
    {
        ComboBox cmb = sender as ComboBox;
        if (cmb?.SelectedItem is float size)
        {
            var pub = GlobalMessagePipe.GetPublisher<int, ChangeScreenFont>();
            pub.Publish(screenIndex, new ChangeScreenFont(size));
        }
    }

}
