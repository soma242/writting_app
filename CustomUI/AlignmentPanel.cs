using PublishStructure;
using System;
using writting_app.MessageInstance;

namespace writting_app.CustomUI;

public class AlignmentPanel: Panel
{

    private readonly int indent = 20;

    //GlobalFilePathのFastQueueから空いているインデックスをもらい、MessagePipeのKeyに用いる。
    //disposeもここから行う
    public int alignmentIndex {  get; private set; }

    //自分の属するTextScreenControlのインデックス
    private int screenIndex;

    public AlignmentData alignmentData { get; private set; }

    //Pub先でint生成するよりこっちで一回判定するほうが安いはず
    private int pastWidth;

    //public System.Drawing.Font font { get; private set; }


    //IPublisher<int, AlignmentWidth> widthPub;

    private IAlignable[]? alignables;

    private IDisposable disposableChangeFont;
    private IDisposable disposableAligneCall;

    public AlignmentPanel(int indexKey, int screenIndex, DrawKind kind)
    {

        //Dock化されてwidthが変更された後に描写される(この時点のwidthはこのあと変更される)ので、各コンポーネントの初期化においてwidthは使用せず、変更時に受け取るための準備だけする。

        this.Resize += ResizeAlignment;
        //test用
        alignmentIndex = indexKey;
        this.screenIndex = screenIndex;
        //alignmentIndex = GlobalFilePath.alignmentIndex

        this.alignmentData = new AlignmentData(new System.Drawing.Font(GlobalFontSetting.fontName, 15f));


        //widthPub = GlobalMessagePipe.GetPublisher<int, AlignmentWidth>();

        var fontSub = GlobalMessagePipe.GetSubscriber<int, ChangeScreenFont>();
        disposableChangeFont = fontSub.Subscribe(screenIndex, get =>
        {
            FontSizeChange(get.size);
        });
        var callSub = GlobalMessagePipe.GetSubscriber<int, AlignCall>();
        disposableAligneCall = callSub.Subscribe(alignmentIndex, get =>
        {
            AligneAlignables();
        });


        this.SuspendLayout();
        switch (kind) { 
            case DrawKind.all:
                SetAllAlignment();
                break;

            case DrawKind.mainText:

                alignables = new IAlignable[2];

                var mainTextButton = new UserPaintButton(alignmentIndex,ControlButtonKind.maintext, alignmentData);
                mainTextButton.Location = new Point(0, 0);
                mainTextButton.Height = 500;
                    //this.Width;
                this.Controls.Add(mainTextButton);
                alignables[0] = mainTextButton;

                var panel = new TreePaintPanel(alignmentIndex, DrawKind.mainText, alignmentData, true);
                panel.Location = new Point(0, 0);
                panel.Height = 500;


                alignables[1] = panel;

               this.Controls.Add(panel);

                AligneAlignables();
                break;

            default:
                break;
        }
        this.ResumeLayout();
    }

    private void SetAllAlignment()
    {
        alignables = new IAlignable[3];

        var mainTextButton = new UserPaintButton(alignmentIndex, ControlButtonKind.maintext, alignmentData);
        //mainTextButton.Location = new Point(0, 0);
        //mainTextButton.Height = 500;
        //this.Width;
        this.Controls.Add(mainTextButton);
        alignables[0] = mainTextButton;

        var panel = new TreePaintPanel(alignmentIndex, DrawKind.mainText, alignmentData, true);
        //panel.Location = new Point(0, 0);
        // panel.Height = 500;


        alignables[1] = panel;

        this.Controls.Add(panel);

        //test
        var button2 = new UserPaintButton(alignmentIndex, ControlButtonKind.maintext, alignmentData);
        this.Controls.Add(button2);
        alignables[2] = button2;

        AligneAlignables();
    }

    private void DisposeSubscribes()
    {
        disposableChangeFont?.Dispose();
        disposableAligneCall?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) {
            //現在使用しているインデックスに追加
            GlobalFilePath.RemoveAlignment(alignmentIndex);
            DisposeSubscribes();
        }

        base.Dispose(disposing);
    }

    private void ResizeAlignment(object? sender, EventArgs e)
    {
        //Widthが変わったときのみ処理が必要(Heightはどこからも参照していない)

        if (pastWidth == Width)
        {
            //var pub = GlobalMessagePipe.GetPublisher<string>();
            //pub.Publish("return");
            return;
        }
        var resizePub = GlobalMessagePipe.GetPublisher<string>();
        resizePub.Publish("resize");

        int width = this.Width - 20;
        foreach(var alignable in alignables)
        {
            alignable.IResizeWidth(width);
        }

        pastWidth = Width;
        //widthPub.Publish(alignmentIndex, new AlignmentWidth(this.Width - 20));
    }

    private void FontSizeChange(float fontSize)
    {
        alignmentData.FontSizeChange(fontSize);
        AligneAlignables();

        var pub = GlobalMessagePipe.GetPublisher<string>();
        pub.Publish("fontCHange");
    }

    private void AligneAlignables()
    {
        int y = 0;
        //UIスレッド外なので必要ない？
        //this.SuspendLayout();
        foreach (var alignable in alignables)
        {
            y += alignable.IAligne(y);
        }
        //this.ResumeLayout();


        //Invalidiateでは振る舞いがおかしかった。
        this.Refresh();
    }

    private void AligneAlignables(int index)
    {

    }

}



public class AlignmentData
{
    public AlignmentData(Font font)
    {
        this.font = font; 
        fontHeight = font.Height;

    }

    public System.Drawing.Font font { get; private set; }

    public int fontHeight { get; private set; }


    public void FontSizeChange(float fontSize)
    {
        this.font?.Dispose();
        this.font = new Font(GlobalFontSetting.fontName, fontSize);
        fontHeight = font.Height;

    }
}
