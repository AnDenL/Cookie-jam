using System.Collections.Generic;
using Creatures;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Generation : MonoBehaviour
{
    public static GameObject[] Prefabs;

    [SerializeField] private GameObject[] prefabs;

    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private Camera MinimapCamera;

    private Dictionary<Vector2Int, Chunk> chunks;
    private GameRandom random;
    public static float lastCheckTime;

    public void Start()
    {
        Prefabs = prefabs;
        chunks = new();
        random = new GameRandom((uint)Random.Range(1,32000));

        ValidateAround(Vector2Int.zero);
        MinimapCamera.Render();
    }

    public void ValidateAround(Vector2Int position)
    {
        PlaceChunk(ValidateChunk(position), position);
        PlaceChunk(ValidateChunk(position + Vector2Int.up), position + Vector2Int.up);
        PlaceChunk(ValidateChunk(position + Vector2Int.up + Vector2Int.left), position + Vector2Int.up + Vector2Int.left);
        PlaceChunk(ValidateChunk(position + Vector2Int.up + Vector2Int.right), position + Vector2Int.up + Vector2Int.right);
        PlaceChunk(ValidateChunk(position + Vector2Int.down), position + Vector2Int.down);
        PlaceChunk(ValidateChunk(position + Vector2Int.down + Vector2Int.left), position + Vector2Int.down + Vector2Int.left);
        PlaceChunk(ValidateChunk(position + Vector2Int.down + Vector2Int.right), position + Vector2Int.down + Vector2Int.right);
        PlaceChunk(ValidateChunk(position + Vector2Int.left), position + Vector2Int.left);
        PlaceChunk(ValidateChunk(position + Vector2Int.right), position + Vector2Int.right);
    }

    private void Update()
    {
        if (lastCheckTime < Time.time)
        {
            lastCheckTime = Time.time + 5f;
            Vector2Int position = Vector2Int.FloorToInt((PlayerController.Player.transform.position + Vector3.one * 8) / 16);
            ValidateAround(position);

            if (chunks.Count > 60)
            {
                CleanUp(position);
            }

            MinimapCamera.Render();
        }
    }

    public void PlaceChunk(Vector2Int position)
    {
        if (!chunks.TryGetValue(position, out Chunk chunkToPlace))
        {
            chunkToPlace = new Chunk(random);
            chunks.Add(position, chunkToPlace);
        }

        if (chunkToPlace.loaded == true) return;
        chunkToPlace.loaded = true;
        GameObject chunkobj = Instantiate(chunkPrefab, (Vector2)position * 16, Quaternion.identity);

        foreach (var obj in chunkToPlace.objects)
        {
            var o = Instantiate(Prefabs[obj.id], position * 16 + obj.position, Quaternion.identity);
            o.transform.parent = chunkobj.transform;
            o.transform.position += Prefabs[obj.id].transform.position;
            o.transform.localScale = obj.flip ? new Vector3(1,1,1) : new Vector3(-1,1,1);
        }
        chunkToPlace.chunk = chunkobj;
        chunkobj.GetComponent<SpriteRenderer>().sprite = ChunkRender.Render(position);
    }

    public void PlaceChunk(Chunk chunkToPlace, Vector2Int position)
    {
        if (chunkToPlace.loaded == true) return;
        chunkToPlace.loaded = true;
        GameObject chunkobj = Instantiate(chunkPrefab, (Vector2)position * 16, Quaternion.identity);

        foreach (var obj in chunkToPlace.objects)
        {
            var o = Instantiate(Prefabs[obj.id], position * 16 + obj.position, Quaternion.identity);
            o.transform.parent = chunkobj.transform;
            o.transform.position += Prefabs[obj.id].transform.position;
            o.transform.localScale = obj.flip ? new Vector3(1,1,1) : new Vector3(-1,1,1);
        }
        chunkToPlace.chunk = chunkobj;
        chunkobj.GetComponent<SpriteRenderer>().sprite = ChunkRender.Render(position);
    }

    public Chunk ValidateChunk(Vector2Int position)
    {
        if (chunks.TryGetValue(position, out Chunk chunk)) return chunk;

        chunk = new(random);
        chunks.Add(position, chunk);
        return chunk;
    }

    public void CleanUp(Vector2Int position)
    {
        foreach (var item in chunks)
        {
            if (new Vector2(item.Key.x - position.x, item.Key.y - position.y).sqrMagnitude > 100)
            {
                item.Value.loaded = false;
                Destroy(item.Value.chunk);
            }
        }
    }
}

public class Chunk
{
    public List<GenObject> objects;

    public bool loaded;

    public GameObject chunk;

    public Chunk(GameRandom rnd)
    {
        int count = rnd.Range(10,24);
        loaded = false;

        objects = new();

        for (int i = 0; i < count; i++)
        {
            var position = new Vector2(rnd.Range(-8f,8f), rnd.Range(-8f,8f));

            bool overlaps = false;

            foreach(var obj in objects)
            {
                if ((obj.position - position).SqrMagnitude() < 4)
                {
                    overlaps = true;
                    break;
                }
            }
            if (overlaps) continue;

            objects.Add(new GenObject {
                position = position,
                id = rnd.Range(0, Generation.Prefabs.Length),
                flip = rnd.Chance(0.5f),
            });
        }
    }
}

public struct GenObject
{
    public Vector2 position;
    public int id;
    public bool flip;
}