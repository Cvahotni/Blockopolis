public struct BlockModifyData 
{
    public float x;
    public float y;
    public float z;

    public BlockID block;
    public ushort amount;

    public BlockModifyData(float x, float y, float z, BlockID block, ushort amount) {
        this.x = x;
        this.y = y;
        this.z = z;

        this.block = block;
        this.amount = amount;
    }
}
