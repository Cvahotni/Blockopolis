using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EncodedChunkMeta
{
    public long key;
    public long currentByteCount;
    public long bytesCount;

    public EncodedChunkMeta(long key, long currentByteCount, long bytesCount) {
        this.key = key;
        this.currentByteCount = currentByteCount;
        this.bytesCount = bytesCount;
    }
}
