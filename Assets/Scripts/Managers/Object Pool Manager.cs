using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Handles reusable object instances.
    /// Used for units, projectiles, effects, and other frequently spawned objects.
    /// </summary>
    public sealed class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }


        [System.Serializable]
        private class Pool
        {
            public string ID;
            public GameObject Prefab;
            public int InitialSize = 10;
        }


        [SerializeField]
        private List<Pool> _pools = new();


        private readonly Dictionary<string, Queue<GameObject>> _poolDictionary = new();


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializePools();
        }


        private void InitializePools()
        {
            foreach (Pool pool in _pools)
            {
                if (pool.Prefab == null)
                    continue;


                Queue<GameObject> objects =
                    new Queue<GameObject>();


                for (int i = 0; i < pool.InitialSize; i++)
                {
                    GameObject obj =
                        CreateObject(pool.Prefab);

                    obj.SetActive(false);

                    objects.Enqueue(obj);
                }


                _poolDictionary.Add(
                    pool.ID,
                    objects);
            }
        }


        private GameObject CreateObject(GameObject prefab)
        {
            return Instantiate(
                prefab,
                transform);
        }


        public GameObject Spawn(
            string id,
            Vector3 position,
            Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(id))
            {
                Debug.LogWarning(
                    $"Pool '{id}' does not exist.");
                return null;
            }


            Queue<GameObject> pool =
                _poolDictionary[id];


            GameObject obj;


            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                Pool data =
                    _pools.Find(x => x.ID == id);

                obj = CreateObject(data.Prefab);
            }


            obj.transform.SetPositionAndRotation(
                position,
                rotation);

            obj.SetActive(true);


            return obj;
        }


        public void Return(
            string id,
            GameObject obj)
        {
            if (obj == null)
                return;


            obj.SetActive(false);


            if (_poolDictionary.ContainsKey(id))
            {
                _poolDictionary[id].Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    }
}