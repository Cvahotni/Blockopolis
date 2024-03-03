[System.Serializable]
public struct BlockStateModel
{
    public uint backVertsStart;
    public uint backVertsEnd;
    public uint backTrisStart;
    public uint backTrisEnd;
    public bool cullBack;

    public uint frontVertsStart;
    public uint frontVertsEnd;
    public uint frontTrisStart;
    public uint frontTrisEnd;
    public bool cullFront;

    public uint upVertsStart;
    public uint upVertsEnd;
    public uint upTrisStart;
    public uint upTrisEnd;
    public bool cullUp;

    public uint bottomVertsStart;
    public uint bottomVertsEnd;
    public uint bottomTrisStart;
    public uint bottomTrisEnd;
    public bool cullBottom;

    public uint leftVertsStart;
    public uint leftVertsEnd;
    public uint leftTrisStart;
    public uint leftTrisEnd;
    public bool cullLeft;

    public uint rightVertsStart;
    public uint rightVertsEnd;
    public uint rightTrisStart;
    public uint rightTrisEnd;
    public bool cullRight;
}