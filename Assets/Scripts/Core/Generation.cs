using System;
using System.Collections.Generic;
using Creatures;
using Unity.VisualScripting;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public static Generation instance;
    public static Biome CurrentBiome;

    [SerializeField] private GenStruct[] genOPrefabs;

    private int[] fieldPrefabs;
    private int[] forestPrefabs;
    private int[] snowPrefabs;
    private int[] hellPrefabs;

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

        List<int> field = new();
        List<int> forest = new();
        List<int> snow = new();
        List<int> hell = new();

        for (int i = 0; i < genOPrefabs.Length; i++)
        {
            if (genOPrefabs[i].Biomes.HasFlag(Biome.Field)) field.Add(i);
            if (genOPrefabs[i].Biomes.HasFlag(Biome.Forest)) forest.Add(i);
            if (genOPrefabs[i].Biomes.HasFlag(Biome.Snow)) snow.Add(i);
            if (genOPrefabs[i].Biomes.HasFlag(Biome.Hell)) hell.Add(i);
        }

        fieldPrefabs = field.ToArray();
        forestPrefabs = forest.ToArray();
        snowPrefabs = snow.ToArray();
        hellPrefabs = hell.ToArray();

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
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
            print(CheckBiome(Vector2Int.FloorToInt((Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) + Vector3.one * 8) / 16)));
    #endif

        if (lastCheckTime < Time.time)
        {
            lastCheckTime = Time.time + 1.5f;
            Vector2Int position = Vector2Int.FloorToInt((PlayerController.Player.transform.position + Vector3.one * 8) / 16);
            ValidateAround(position);

            CurrentBiome = CheckBiome(position);

            if (CurrentBiome == Biome.Snow) 
                ParticleManager.PlayParticle("Breath", PlayerController.Player.transform.position, 1);

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
            chunkToPlace = new Chunk(position, random);
            chunks.Add(position, chunkToPlace);
        }

        if (chunkToPlace.loaded == true) return;
        chunkToPlace.loaded = true;
        GameObject chunkobj = new($"Chunk {position.ToShortString()}");

        foreach (var obj in chunkToPlace.objects)
        {
            var o = Instantiate(genOPrefabs[obj.id].prefab, position * 16 + obj.position, Quaternion.identity);
            o.transform.parent = chunkobj.transform;
            o.transform.position += genOPrefabs[obj.id].prefab.transform.position;
            o.transform.localScale = obj.flip ? new Vector3(1,1,1) : new Vector3(-1,1,1);
        }
        chunkToPlace.chunk = chunkobj;
        Minimap.DrawMapChunk(position);
    }

    public static Biome CheckBiome(Vector2Int position)
    {
        float w = GetPixel(position * 16 + offsets[0], 0.3f) * GetPixel(position * 16 + offsets[1]);;
        float t = GetPixel(position * 16 + offsets[2], 0.3f) * GetPixel(position * 16 + offsets[3]);
        float h = GetPixel(position * 16 + offsets[4], 0.3f) * GetPixel(position * 16 + offsets[5]);
        
        // GameObject sq = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // sq.transform.position = (Vector2)position * 16;
        // sq.transform.localScale = Vector3.one * 4;

        if (w < 0.09f && Vector2.Distance(Vector2.zero, position * 16) > 60) { 
            //sq.GetComponent<MeshRenderer>().material.color = Color.red;
            return Biome.Hell;
        }
        if (w > 0.45f) 
        {
            //sq.GetComponent<MeshRenderer>().material.color = Color.blue;
            return Biome.Snow;
        }
        if (t > 0.45f) {
            //sq.GetComponent<MeshRenderer>().material.color = Color.black;
            return Biome.Forest;
        }
        //sq.GetComponent<MeshRenderer>().material.color = Color.green;

        return Biome.Field;
    }


    public static float GetPixel(Vector2 position, float scale = 1)
    {
        int x = Mathf.FloorToInt(position.x * instance.scale * scale * instance.noisemap.width);
        int y = Mathf.FloorToInt(position.y * instance.scale * scale * instance.noisemap.height);
        return instance.noisemap.GetPixel(x % instance.noisemap.width, y % instance.noisemap.height).grayscale;
    }

    public static int[] BiomeToIds(Biome biome)
    {
        return biome switch
        {
            Biome.Field => instance.fieldPrefabs,
            Biome.Forest => instance.forestPrefabs,
            Biome.Snow => instance.snowPrefabs,
            Biome.Hell => instance.hellPrefabs,
            _ => instance.fieldPrefabs,
        };
    }

    public void PlaceChunk(Chunk chunkToPlace, Vector2Int position) 
    {
        if (chunkToPlace.loaded == true) return;
        chunkToPlace.loaded = true;
        GameObject chunkobj = new($"Chunk {position.ToShortString()}");

        foreach (var obj in chunkToPlace.objects)
        {
            var o = Instantiate(genOPrefabs[obj.id].prefab, position * 16 + obj.position, Quaternion.identity);
            o.transform.parent = chunkobj.transform;
            o.transform.position += genOPrefabs[obj.id].prefab.transform.position;
            o.transform.localScale = obj.flip ? new Vector3(1,1,1) : new Vector3(-1,1,1);
        }
        chunkToPlace.chunk = chunkobj;
        Minimap.DrawMapChunk(position);
    }

    public Chunk ValidateChunk(Vector2Int position)
    {
        if (chunks.TryGetValue(position, out Chunk chunk)) return chunk;

        chunk = new(position, random);
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

[Serializable]
public struct GenStruct
{
    public GameObject prefab;

    public Biome Biomes;
}

[Flags]
public enum Biome
{
    None = 0,
    Field = 1 << 0,
    Forest = 1 << 1,
    Snow = 1 << 2,
    Hell = 1 << 3
}

public class Chunk
{
    public List<GenObject> objects;

    public GameObject chunk;
    public Biome biome;

    public bool loaded;
    
    public Chunk(Vector2Int position, GameRandom rnd)
    {
        int count = rnd.Range(10,24);
        loaded = false;

        objects = new();

        biome = Generation.CheckBiome(position);

        for (int i = 0; i < count; i++)
        {
            var pos = new Vector2(rnd.Range(-8f,8f), rnd.Range(-8f,8f));

            bool overlaps = false;

            foreach(var obj in objects)
            {
                if ((obj.position - pos).SqrMagnitude() < 4)
                {
                    overlaps = true;
                    break;
                }
            }
            if (overlaps) continue;

            var ids = Generation.BiomeToIds(biome);

            objects.Add(new GenObject {
                position = pos,
                id = ids[rnd.Range(0, ids.Length)],
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