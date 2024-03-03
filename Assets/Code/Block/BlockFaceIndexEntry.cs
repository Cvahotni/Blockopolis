public struct BlockFaceIndexEntry
{
    public uint vertsStart;
    public uint vertsEnd;

    public uint trisStart;
    public uint trisEnd;

    public BlockFaceIndexEntry(uint vertsStart, uint vertsEnd, uint trisStart, uint trisEnd) {
        this.vertsStart = vertsStart;
        this.vertsEnd = vertsEnd;

        this.trisStart = trisStart;
        this.trisEnd = trisEnd;
    }
}
