using System.Collections.Generic;
using UnityEngine;

// Brings a destroyed bomb back to where it was after a short delay.
public class BombRespawner : MonoBehaviour
{
    public GameObject bombPrefab;
    public GameObject effectsManager;
    public float respawnDelay = 4f;

    class Slot
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject bomb;
        public float emptySince = -1f;
    }

    readonly List<Slot> slots = new List<Slot>();
    Transform inactiveHolder;

    void Start()
    {
        foreach (Bomb part in FindObjectsByType<Bomb>())
        {
            GameObject root = part.target;
            if (!slots.Exists(slot => slot.bomb == root))
            {
                slots.Add(new Slot { position = root.transform.position, rotation = root.transform.rotation, bomb = root });
            }
        }

        // New bombs are built under an inactive parent so their fields can be set before Target.Awake runs.
        GameObject holder = new GameObject("BombSpawnHolder");
        holder.SetActive(false);
        holder.transform.SetParent(transform, false);
        inactiveHolder = holder.transform;
    }

    void Update()
    {
        foreach (Slot slot in slots)
        {
            if (slot.bomb != null)
            {
                continue;
            }

            if (slot.emptySince < 0f)
            {
                slot.emptySince = Time.time;
            }
            else if (Time.time - slot.emptySince >= respawnDelay)
            {
                slot.bomb = Spawn(slot);
                slot.emptySince = -1f;
            }
        }
    }

    GameObject Spawn(Slot slot)
    {
        GameObject bomb = Instantiate(bombPrefab, slot.position, slot.rotation, inactiveHolder);
        bomb.name = bombPrefab.name;

        foreach (Target target in bomb.GetComponentsInChildren<Target>(true))
        {
            target.effectsManager = effectsManager;
        }

        bomb.transform.SetParent(null, true);
        bomb.AddComponent<SpawnPop>();
        return bomb;
    }
}
