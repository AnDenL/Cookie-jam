using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    public static Dictionary<string, int> ParticleIndices = new();

    public ParticleSystem[] ParticleSystems = new ParticleSystem[0];

    private void Awake()
    {
        Instance = this;

        var particles = new List<ParticleSystem>();
        foreach (Transform ch in transform)
        {
            var ps = ch.GetComponent<ParticleSystem>();
            if (ps != null) particles.Add(ps);
        }

        ParticleSystems = particles.ToArray();

        ParticleIndices = ParticleSystems
            .Select((ps, index) => new { ps, index })
            .ToDictionary(x => x.ps.name, x => x.index);
    }

    public static void PlayParticle(string name, Vector2 position, int amount, float rotation = 0)
    {
        if (ParticleIndices.TryGetValue(name, out int index))
            PlayParticle(index, position, amount, rotation);
    }

    private static void PlayParticle(int index, Vector2 position, int amount, float rotation)
    {
        ParticleSystem ps = Instance.ParticleSystems[index];
        if (ps == null) return;
        
        var emitParams = new ParticleSystem.EmitParams
        {
            position = position,
            rotation = rotation,
            applyShapeToPosition = true
        };

        ps.Emit(emitParams, amount);
    }
} 

public class ParticlePool
{
    public ParticleSystem ParticlePrefab;

    private ParticleSystem[] pool;
    private int poolSize;

    public ParticlePool(ParticleSystem prefab, int size, Transform parent = null)
    {
        ParticlePrefab = prefab;
        poolSize = size;
        pool = new ParticleSystem[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Object.Instantiate(ParticlePrefab, parent);
            pool[i].gameObject.AddComponent<PooledParticle>();
            pool[i].gameObject.SetActive(false);
        }
    }

    public void PlayParticle(Vector2 position)
    {
        var ps = GetParticleSystem();

        if (ps != null)
        {
            ps.transform.position = position;
            ps.gameObject.SetActive(true);
            ps.Play();
        }
        else
        {
            Debug.LogWarning("No available particle systems in the pool.");
        }

    }


    public void PlayParticle(Vector2 from, Transform to, int amount)
    {
        var ps = GetParticleSystem();

        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.transform.parent = to;
            ps.transform.localPosition = Vector3.zero;
            var emission = ps.emission;
            var shape = ps.shape;
            shape.position = (Vector3)from - to.position;
            emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)amount));
            ps.gameObject.SetActive(true);
            ps.Play();
        }
        else
        {
            Debug.LogWarning("No available particle systems in the pool.");
        }
    }

    private ParticleSystem GetParticleSystem()
    {
        ParticleSystem ps = null;
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].IsAlive(true))
            {
                ps = pool[i];
                break;
            }
        }

        return ps;
    }
} 