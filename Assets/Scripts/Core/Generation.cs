using System;
using System.Collections.Generic;
using Creatures;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Generation : MonoBehaviour
{
    public static Generation instance;
    public static GameObject[] Prefabs;

    [SerializeField] private GameObject[] prefabs;

    [SerializeField] private Camera minimapCamera;

    [SerializeField] private Material terrain;
    [SerializeField] private Texture2D noisemap;
    [SerializeField] private float scale;

    private Dictionary<Vector2Int, Chunk> chunks;
    private GameRandom random;
    public static float lastCheckTime;
    private static Vector2[] offsets;

    [ContextMenu("Regenerate")]
    public void Start()
    {
        Prefabs = prefabs;
        instance = this;
        chunks = new();
        random = new GameRandom((uint)UnityEngine.Random.Range(1,32000));

        offsets = new Vector2[6];
        offsets[0] = random.PointInCircle(10000);
        offsets[1] = random.PointInCircle(10000);
        offsets[2] = random.PointInCircle(10000);
        offsets[3] = random.PointInCircle(10000);
        offsets[4] = random.PointInCircle(10000);
        offsets[5] = random.PointInCircle(10000);

        terrain.SetVector("_1", offsets[0]);
        terrain.SetVector("_2", offsets[1]);
        terrain.SetVector("_3", offsets[2]);
        terrain.SetVector("_4", offsets[3]);
        terrain.SetVector("_5", offsets[4]);
        terrain.SetVector("_6", offsets[5]);

        for (int i = -64; i < 64; i++)
        {
            for (int j = -64; j < 64; j++)
            {
                CheckBiome(new Vector2Int(i,j));
            }
        }

        ValidateAround(Vector2Int.zero);
        minimapCamera.Render();
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
        PlaceChunk(ValidateChunk(position + Vector2Int.left*2), position + Vector2Int.left*2);
        PlaceChunk(ValidateChunk(position + Vector2Int.right*2), position + Vector2Int.right*2);
        PlaceChunk(ValidateChunk(position + Vector2Int.up + Vector2Int.left*2), position + Vector2Int.up + Vector2Int.left*2);
        PlaceChunk(ValidateChunk(position + Vector2Int.up + Vector2Int.right*2), position + Vector2Int.up + Vector2Int.right*2);
        PlaceChunk(ValidateChunk(position + Vector2Int.down + Vector2Int.left*2), position + Vector2Int.down + Vector2Int.left*2);
        PlaceChunk(ValidateChunk(position + Vector2Int.down + Vector2Int.right*2), position + Vector2Int.down + Vector2Int.right*2);
    }

    private void Update()
    {
        if (lastCheckTime < Time.time)
        {
            lastCheckTime = Time.time + 1.5f;
            Vector2Int position = Vector2Int.FloorToInt((PlayerController.Player.transform.position + Vector3.one * 8) / 16);
            ValidateAround(position);

            print(CheckBiome(position).ToShortString());

            if (chunks.Count > 60)
            {
                CleanUp(position);
            }

            minimapCamera.Render();
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
        GameObject chunkobj = new($"Chunk {position.ToShortString()}");

        foreach (var obj in chunkToPlace.objects)
        {
            var o = Instantiate(Prefabs[obj.id], position * 16 + obj.position, Quaternion.identity);
            o.transform.parent = chunkobj.transform;
            o.transform.position += Prefabs[obj.id].transform.position;
            o.transform.localScale = obj.flip ? new Vector3(1,1,1) : new Vector3(-1,1,1);
        }
        chunkToPlace.chunk = chunkobj;
        Minimap.DrawMapChunk(position);
    }

    public static Biome CheckBiome(Vector2Int position)
    {
        float s1 = GetPixel(position * 16 + offsets[0], 0.3f);
        float s2 = GetPixel(position * 16 + offsets[1]);
        float s = s1 * s2;
        float f = GetPixel(position * 16 + offsets[2], 0.3f) * GetPixel(position * 16 + offsets[3]);
        float h = Mathf.Clamp(Mathf.Clamp((GetPixel(position * 16 + offsets[4], 0.3f) + GetPixel(position * 16 + offsets[5])) * 0.7f, 0, 1) - (s1 + s2), 0, 1);

        print("S " + s);
        print("F " + f);
        print("H " + h);
        
        GameObject sq = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sq.transform.position = (Vector2)position * 16;
        sq.transform.localScale = Vector3.one * 4;

        if (h > 0) { 
            sq.GetComponent<MeshRenderer>().material.color = Color.red;
            return Biome.Forest;
        }
        if (s > 0.45f) 
        {
            sq.GetComponent<MeshRenderer>().material.color = Color.blue;
            return Biome.Snow;
        }
        if (f > 0.45f) {
            sq.GetComponent<MeshRenderer>().material.color = Color.black;
            return Biome.Forest;
        }
        sq.GetComponent<MeshRenderer>().material.color = Color.green;

        return Biome.Field;
    }

    public static float GetPixel(Vector2 position, float scale = 1)
    {
        int x = Mathf.FloorToInt(position.x * instance.scale * scale * instance.noisemap.width);
        int y = Mathf.FloorToInt(position.y * instance.scale * scale * instance.noisemap.height);
        return instance.noisemap.GetPixel(x % instance.noisemap.width, y % instance.noisemap.height).grayscale;
    }

    public void PlaceChunk(Chunk chunkToPlace, Vector2Int position) 
    {
        if (chunkToPlace.loaded == true) return;
        chunkToPlace.loaded = true;
        GameObject chunkobj = new($"Chunk {position.ToShortString()}");

        foreach (var obj in chunkToPlace.objects)
        {
            var o = Instantiate(Prefabs[obj.id], position * 16 + obj.position, Quaternion.identity);
            o.transform.parent = chunkobj.transform;
            o.transform.position += Prefabs[obj.id].transform.position;
            o.transform.localScale = obj.flip ? new Vector3(1,1,1) : new Vector3(-1,1,1);
        }
        chunkToPlace.chunk = chunkobj;
        Minimap.DrawMapChunk(position);
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

public enum Biome
{
    Field,
    Forest,
    Snow,
    Hell
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