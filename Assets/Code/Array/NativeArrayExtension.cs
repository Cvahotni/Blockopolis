using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class NativeArrayExtension 
{
    private static readonly int chunkElementsSize = VoxelProperties.chunkWidth * VoxelProperties.chunkHeight * VoxelProperties.chunkWidth;

    private static NativeArray<ushort> chunkArray = new NativeArray<ushort>(chunkElementsSize, Allocator.Persistent);
    private static NativeSlice<byte> chunkBytes = new NativeSlice<ushort>(chunkArray).SliceConvert<byte>();

    private static NativeArray<byte> byteArr = new NativeArray<byte>(chunkElementsSize * sizeof(ushort), Allocator.Persistent);
    private static bool disposed = false;

    public static void ToRawBytes(this NativeArray<ushort> arr, byte[] rawBytes) {
        arr.CopyTo(chunkArray);
        chunkBytes.CopyTo(rawBytes);
    }

    public static void CopyFromRawBytes<T>(this NativeArray<T> arr, byte[] bytes, int bytesLength) where T : struct {
        NativeArray<byte>.Copy(bytes, byteArr, bytesLength);

        var slice = new NativeSlice<byte>(byteArr).SliceConvert<T>();
        slice.CopyTo(arr);
    }

    public static void FromRawBytes<T>(byte[] bytes, int bytesLength, NativeArray<T> nativeArray) where T : struct {
        int structSize = UnsafeUtility.SizeOf<T>();
        UnityEngine.Debug.Assert(bytesLength % structSize == 0);

        int length = bytesLength / UnsafeUtility.SizeOf<T>();
        nativeArray.CopyFromRawBytes(bytes, bytesLength);
    }

    public static void OnDestroy() {
        if(disposed) return;

        chunkArray.Dispose();
        disposed = true;
    }
}