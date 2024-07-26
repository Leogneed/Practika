using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataBase : ScriptableObject
{
    [SerializeField] private BlockInfo[] Blocks;

    private readonly Dictionary<BlockType, BlockInfo> blocksCached = new Dictionary<BlockType, BlockInfo>();

    private void OnEnable()
    {
        blocksCached.Clear();

        foreach (var blockInfo in Blocks)
        {
            blocksCached.Add(blockInfo.Type, blockInfo);
        }
    }

    public BlockInfo GetInfo(BlockType type)
    {
        if(blocksCached.TryGetValue(type, out var blockInfo))
        {
            return blockInfo;
        }

        return null;
    }
}
