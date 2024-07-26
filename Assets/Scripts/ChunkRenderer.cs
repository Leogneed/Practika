using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 32;
    public const int ChunkHeight = 128;
    public const float BlockScale = .125f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    private Mesh chunkMesh;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        chunkMesh = new Mesh();
        RegenerateMesh();

        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    private void RegenerateMesh()
    {
        verticies.Clear();
        uvs.Clear();
        triangles.Clear();

        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.uv = uvs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Stone;
        RegenerateMesh();
    }

    public void DestroyBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }

    private void GenerateBlock(int x, int y, int z)
    {
        var blockPosition = new Vector3Int(x, y, z);
        BlockType blockType = GetBlockAtPosition(blockPosition);
        if (blockType == BlockType.Air) return;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateRightSide(blockPosition);
            AddUvs(blockType);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(blockType);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(blockType);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(blockType);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(blockType);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(blockType);
        }
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight) return BlockType.Air;


            Vector2Int adjacentChunkPosition = ChunkData.ChunkPosition;
            if(blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if (blockPosition.x >=ChunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }

            if (blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if (blockPosition.z >= ChunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }

            if(ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockType.Air;
            }
        }
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);


        AddLastVerticiesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateTopSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void AddLastVerticiesSquare()
    {
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }

    private void AddUvs(BlockType blockType)
    {
        Vector2 uv;
        if(blockType == BlockType.Dirt)
        {
            uv = new Vector2(32f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Stone)
        {
            uv = new Vector2(16f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Bedrock)
        {
            uv = new Vector2(64f / 256, 240f / 256);
        }
        else
        {
            uv = new Vector2(160f / 256, 224f / 256);
        }

        for (int i = 0; i<4; i++)
        {
            uvs.Add(uv);
        }
    }
}

