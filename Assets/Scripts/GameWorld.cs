using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public int ViewRadius = 10;

    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer ChunkPrefab;

    private Camera Camera;
    private Vector2Int currentPlayerChunk;
    public TerrainGenerator Generator;


    void Start()
    {
        Camera = Camera.main;
        StartCoroutine(Generate(false));
    }

    private IEnumerator Generate(bool wait)
    {
        for (int x = currentPlayerChunk.x - ViewRadius; x < currentPlayerChunk.x + ViewRadius; x++) 
        {
            for (int y = currentPlayerChunk.y - ViewRadius; y < currentPlayerChunk.y + ViewRadius; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if (ChunkDatas.ContainsKey(chunkPosition)) continue;
                LoadChunkAt(chunkPosition);

                if (wait) yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }
    [ContextMenu("Regenerate world")]
    public void Regenerate()
    {
        Generator.Init();
        foreach (var chunkData in ChunkDatas)
        {
            Destroy(chunkData.Value.Renderer.gameObject);
        }

        ChunkDatas.Clear();

        StartCoroutine(Generate(false));
    }

    private void LoadChunkAt(Vector2Int chunkPosition)
    {
        int x;
        int y;
        float xPos = chunkPosition.x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
        float zPos = chunkPosition.y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;

        ChunkData chunkData = new ChunkData();
        chunkData.ChunkPosition = chunkPosition;
        chunkData.Blocks = Generator.GenerateTerrain((int)xPos, (int)zPos);
        ChunkDatas.Add(chunkPosition, chunkData);

        var chunk = Instantiate(ChunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
        chunk.ChunkData = chunkData;
        chunk.ParentWorld = this;

        chunkData.Renderer = chunk;
    }

    private void Update()
    {
        Vector3Int playerWorldPos = Vector3Int.FloorToInt(Camera.transform.position / ChunkRenderer.BlockScale);
        Vector2Int playerChunk = GetChunkContainingBlock(playerWorldPos);
        if (playerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = playerChunk;
            StartCoroutine(Generate(true));
        }

        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            bool isDestroying = Input.GetMouseButtonDown(0);

            Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out var hitInfo))
            {
                Vector3 blockCenter;
                if (isDestroying)
                {
                    blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.BlockScale / 2;
                }
                else
                {
                    blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2;
                }

                Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale);
                Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
                if (ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                {
                    Vector3Int chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkRenderer.ChunkWidth;
                    if (isDestroying)
                    {
                        chunkData.Renderer.DestroyBlock(blockWorldPos - chunkOrigin);
                    }
                    else
                    {
                        chunkData.Renderer.SpawnBlock(blockWorldPos - chunkOrigin);
                    }

                }
            }
        }
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        Vector2Int chunkPosition = new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth, blockWorldPos.z / ChunkRenderer.ChunkWidth);

        if (blockWorldPos.x < 0) chunkPosition.x--;
        if (blockWorldPos.z < 0) chunkPosition.y--;

        return chunkPosition;



    } 
}
