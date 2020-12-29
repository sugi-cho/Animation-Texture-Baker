using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.UI;

public class GridSpawner : MonoBehaviour
{
    //[SerializeField, Tooltip("number of entities to spawn (default 10)")] private int m_spawnCount = 10;
    [SerializeField, Tooltip("spacing between entities (default 1.5)")] private float m_spacing = 1.5f;
    [SerializeField, Tooltip("a text object that will get updated with the current spawn count (optional)")] private Text m_statusText = null;
    [SerializeField, Tooltip("an input field object that lets the player enter the spawn count")] private InputField m_inputField = null;

    //bool m_run = false;
    int m_currentCount = 0;
    //int m_prefabIndex = 0;
    EntityManager m_em;
    float3 m_pos;
    float m_width = 0f;
    int m_spawnCount = 0;
    NativeArray<Entity> m_prefabs;

    //public void StartSpawning()
    //{
    //    m_run = true;
    //}

    //public void StopSpawning()
    //{
    //    m_run = false;
    //}

    // Start is called before the first frame update
    void Start()
    {
        //m_pos = transform.position;
        m_em = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = m_em.CreateEntityQuery(ComponentType.ReadOnly<SpawnTag>());
        m_prefabs = query.ToEntityArray(Allocator.Persistent);

        UpdateCountText();
    }

    public void ClearSpawns()
    {
        EntityQuery query = m_em.CreateEntityQuery(ComponentType.ReadOnly<SpawnedTag>());
        m_em.DestroyEntity(query);
        UpdateCountText();
    }

    public void BatchSpawn()
    {
        ClearSpawns();

        if (m_prefabs.Length <= 0) {
            m_statusText.text = "No entities found with SpawnTag.";
            return;
        }

        m_pos = transform.position;
        m_spawnCount = int.Parse(m_inputField.text);
        m_width = math.sqrt(m_spawnCount) * m_spacing;
        int wholePart = m_spawnCount / m_prefabs.Length;
        int remainder = m_spawnCount - (wholePart * m_prefabs.Length);
        // for every prefab, spawn wholePart instances
        for (int i = 0; i < m_prefabs.Length; i++) {
            // if we're on the last index, include the remainder
            if (i == m_prefabs.Length - 1) { wholePart += remainder; }
            // do a batch instantiation
            NativeArray<Entity> entities = m_em.Instantiate(m_prefabs[i], wholePart, Allocator.Temp);
            // set the position for each of the new entities
            for (int j = 0; j < entities.Length; j++) {
                m_em.AddComponentData<Translation>(entities[j], new Translation { Value = m_pos });
                m_em.AddComponent<SpawnedTag>(entities[j]);
                IncrementPosition();
            }
            m_currentCount += entities.Length;
            entities.Dispose();
        }
        UpdateCountText();
    }

    void UpdateCountText()
    {
        if (m_statusText != null) { m_statusText.text = m_currentCount + " of " + m_spawnCount + " entities"; }
    }

    void IncrementPosition()
    {
        // increment position
        m_pos.x += m_spacing;
        if (m_pos.x > (m_width + transform.position.x)) {
            m_pos.x = transform.position.x;
            m_pos.z += m_spacing;
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (!m_run) { return; }
    //    if (m_currentCount >= m_spawnCount) { return; }

    //    Entity entity = m_em.Instantiate(m_prefabs[m_prefabIndex]);
    //    if (entity == Entity.Null) { return; }

    //    // increment prefab index
    //    m_prefabIndex++;
    //    if (m_prefabIndex >= m_prefabs.Length) { m_prefabIndex = 0; }

    //    m_em.AddComponentData<Translation>(entity, new Translation { Value = new float3(m_pos) });

    //    IncrementPosition();

    //    m_currentCount++;

    //    UpdateCountText();
    //}


}
