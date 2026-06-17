using System.Collections.Generic;
using Creatures;
using UnityEngine;

class CreaturesSpawner : MonoBehaviour
{
    public static CreaturesSpawner Instance;

    [SerializeField] bool spawnerActive = true;
    [SerializeField] float minimumDistanceToPlayer = 10f;
    [SerializeField] float maximumDistanceToPlayer = 20f;
    [SerializeField] float spawnProbability = 0.1f;
    [SerializeField] float spawnCooldown = 30f;
    [SerializeField] int maximumCreaturesCount = 10;
    //[SerializeField] Creature[] creaturesToSpawn;
    [SerializeField] GameObject[] spawnPrefabs;
    [SerializeField] Queue<Creature> spawnedCreatures = new();

    private float _cooldownTime;

    private void Awake()
    {
        // singleton implementation
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    private void Start()
    {
        _cooldownTime = spawnCooldown;
    }

    private void Update()
    {
        if (!spawnerActive) return;

        // call OnCooldown() every spawnCooldown seconds
        if (_cooldownTime > 0)
        {
            _cooldownTime -= Time.deltaTime;
        }
        else
        {
            OnCooldown();
            _cooldownTime = spawnCooldown;
        }
    }

    private void OnCooldown()
    {
        UpdateAllCreaturesStates();
        
        if (spawnedCreatures.Count < maximumCreaturesCount && Random.Range(0f, 1f) > spawnProbability)
        {
            SpawnRandomCreatureAtRandomPositionInPlayerRange();
        }
    }

    public GameObject GetRandomCreatureInBiome(Biome biome)
    {
        return spawnPrefabs[Random.Range(0, spawnPrefabs.Length)]; // TODO: actually look at biome
    }

    public void SpawnRandomCreatureAtRandomPositionInPlayerRange()
    {
        // get random position
        Vector3 playerPos = PlayerController.Instance.GetTargetPosition();
        float spawnRadius = UnityEngine.Random.Range(playerPos.x + minimumDistanceToPlayer, playerPos.x + maximumDistanceToPlayer);
        Vector3 spawnPosition = UnityEngine.Random.insideUnitCircle * spawnRadius;
        
        // get correct creature
        Biome biomeAtSpawnPoint = Generation.CheckBiome((Vector2Int)Vector3Int.RoundToInt(spawnPosition));
        GameObject randomCreature = GetRandomCreatureInBiome(biomeAtSpawnPoint);
        
        GameObject go = Instantiate(GetRandomCreatureInBiome(biomeAtSpawnPoint), spawnPosition, Quaternion.identity);
        RegisterCreature(randomCreature);
    }

    public static void RegisterCreature(GameObject creature)
    {
        Instance.spawnedCreatures.Enqueue(creature.GetComponent<Creature>());
    }

    private void UpdateAllCreaturesStates()
    {
        if (spawnedCreatures.Count == 0) return;

        int count = Mathf.Min(maximumCreaturesCount, spawnedCreatures.Count);

        Vector2 playerPos = PlayerController.Player.transform.position;
        
        for (int i = 0; i < count; i++)
        {
            Creature creature = spawnedCreatures.Dequeue();
            
            if (creature != null)
            {
                UpdateCreatureState(creature, playerPos);
                spawnedCreatures.Enqueue(creature);
            }
        }
    }

    private void UpdateCreatureState(Creature creature, Vector2 playerPos)
    {
        float distSqr = ((Vector2)creature.transform.position - playerPos).sqrMagnitude;
        // creature.IsActive = distSqr < maximumDistanceToPlayer;
        if (distSqr < maximumDistanceToPlayer)
        {
            Destroy(creature.gameObject);
        }
    }
}