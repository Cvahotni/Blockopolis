using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;

public class ChunkEncoderDecoder
{
    private static readonly ProfilerMarker encodeMarker = new ProfilerMarker("ChunkEncoderDecoder.Encode");
    private static readonly ProfilerMarker decodeMarker = new ProfilerMarker("ChunkEncoderDecoder.Decode");

    public static void Encode(NativeArray<ushort> voxelMap, NativeList<EncodedVoxelMapEntry> encodedVoxelMap) {
        encodeMarker.Begin();

        var runLengthEncoderJob = new ChunkRunLengthEncoderJob() {
            voxelMap = voxelMap,
            encodedVoxelMap = encodedVoxelMap
        };

        JobHandle runLengthEncoderJobHandle = runLengthEncoderJob.Schedule();
        runLengthEncoderJobHandle.Complete();

        encodeMarker.End();
    }

    public static NativeArray<ushort> Decode(NativeList<EncodedVoxelMapEntry> encodedVoxelMap, ref ChunkBuilder chunkBuilder) {
        decodeMarker.Begin();

        NativeArray<ushort> voxelMap = chunkBuilder.CreateNewVoxelMap();

        var runLengthDecoderJob = new ChunkRunLengthDecoderJob() {
            decodedVoxelMap = voxelMap,
            encodedVoxelMap = encodedVoxelMap
        };

        JobHandle runLengthDecoderJobHandle = runLengthDecoderJob.Schedule();
        runLengthDecoderJobHandle.Complete();

        encodedVoxelMap.Dispose();

        decodeMarker.End();
        return voxelMap;
    }
}
