using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class NativeArrayExtension 
{
    public static byte[] ToRawBytes<T>(this NativeArray<T> arr) where T : struct {
        var slice = new NativeSlice<T>(arr).SliceConvert<byte>();
        var bytes = new byte[slice.Length];
        
        slice.CopyTo(bytes);
        return bytes;
    }

    public static void CopyFromRawBytes<T>(this NativeArray<T> arr, byte[] bytes, int bytesLength) where T : struct {
        var byteArr = new NativeArray<byte>(bytesLength, Allocator.Persistent);
        NativeArray<byte>.Copy(bytes, byteArr, bytesLength);

        var slice = new NativeSlice<byte>(byteArr).SliceConvert<T>();
        slice.CopyTo(arr);
    }

    public static NativeArray<T> FromRawBytes<T>(byte[] bytes, int bytesLength, Allocator allocator) where T : struct {
        int structSize = UnsafeUtility.SizeOf<T>();
        UnityEngine.Debug.Assert(bytesLength % structSize == 0);

        int length = bytesLength / UnsafeUtility.SizeOf<T>();
        var arr = new NativeArray<T>(length, allocator);

        arr.CopyFromRawBytes(bytes, bytesLength);
        return arr;
    }
}