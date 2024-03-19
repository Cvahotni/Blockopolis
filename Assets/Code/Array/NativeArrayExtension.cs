using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class NativeArrayExtension 
{
    private static readonly int chunkElementsSize = VoxelProperties.chunkWidth * VoxelProperties.chunkHeight * VoxelProperties.chunkWidth;

    private static NativeArray<ushort> chunkVoxelArray = new NativeArray<ushort>(chunkElementsSize, Allocator.Persistent);
    private static NativeSlice<byte> chunkVoxelBytes = new NativeSlice<ushort>(chunkVoxelArray).SliceConvert<byte>();

    private static NativeArray<byte> chunkLightArray = new NativeArray<byte>(chunkElementsSize, Allocator.Persistent);
    private static NativeSlice<byte> chunkLightBytes = new NativeSlice<byte>(chunkLightArray).SliceConvert<byte>();

    private static NativeArray<byte> voxelByteArray = new NativeArray<byte>(chunkElementsSize * sizeof(ushort), Allocator.Persistent);
    private static NativeArray<byte> lightByteArray = new NativeArray<byte>(chunkElementsSize * sizeof(byte), Allocator.Persistent);

    private static bool disposed = false;

    public static void ToRawBytes(this NativeArray<byte> arr, byte[] rawBytes) {
        arr.CopyTo(chunkLightArray);
        chunkLightBytes.CopyTo(rawBytes);
    }

    public static void ToRawBytes(this NativeArray<ushort> arr, byte[] rawBytes) {
        arr.CopyTo(chunkVoxelArray);
        chunkVoxelBytes.CopyTo(rawBytes);
    }

    public static void CopyFromRawBytes<T>(this NativeArray<T> arr, byte[] bytes, int bytesLength) where T : struct {
        if(typeof(T) == typeof(ushort)) {
            NativeArray<byte>.Copy(bytes, voxelByteArray, bytesLength);
            var slice = new NativeSlice<byte>(voxelByteArray).SliceConvert<T>();

            slice.CopyTo(arr);
        }

        else if(typeof(T) == typeof(byte)) {
            NativeArray<byte>.Copy(bytes, lightByteArray, bytesLength);
            var slice = new NativeSlice<byte>(lightByteArray).SliceConvert<T>();

            slice.CopyTo(arr);
        }
    }

    public static void FromRawBytes<T>(byte[] bytes, int bytesLength, NativeArray<T> nativeArray) where T : struct {
        int structSize = UnsafeUtility.SizeOf<T>();
        UnityEngine.Debug.Assert(bytesLength % structSize == 0);
        
        nativeArray.CopyFromRawBytes(bytes, bytesLength);
    }

    public static void OnDestroy() {
        if(disposed) return;

        chunkVoxelArray.Dispose();
        disposed = true;
    }
}