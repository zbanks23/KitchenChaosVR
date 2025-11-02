using UnityEngine;
using UnityEditor;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;

namespace Unity.VRTemplate
{
    public enum FoodTypes
    {
        Burger,
        Curry,
        HotDog,
        Pancake,
        Pizza,
        Sushi,
        Udon
    }

    [Serializable]
    public class EnumObjectMapping<TEnum> : IEnumerable where TEnum : struct, Enum
    {
        [SerializeField]
        private List<TEnum> enumList = new List<TEnum>();
        [SerializeField]
        private List<GameObject> gameObjectList = new List<GameObject>();

        public EnumObjectMapping()
        {
            enumList = new List<TEnum>((TEnum[]) Enum.GetValues(typeof(TEnum)));
            gameObjectList = new List<GameObject>(enumList.Count);

            // Initialize gameObjectList with null values
            for (int i = 0; i < enumList.Count; i++)
            {
                gameObjectList.Add(null);
            }
        }

        public GameObject this[TEnum index]
        {
            get => gameObjectList[(int)(object)index];
            set => gameObjectList[(int)(object)index] = value;
        }

        public IEnumerator GetEnumerator() => enumList.GetEnumerator();
    }


    public class FoodSpawner : MonoBehaviour
    {
        // public List<GameObject> FoodPrefabs { get; private set; }

        [SerializeField]
        public EnumObjectMapping<FoodTypes> FoodTypesMapping = new EnumObjectMapping<FoodTypes>();

        public Queue<FoodTypes> FoodTypesQueue { get; } = new Queue<FoodTypes>();

        public float Offset;

        public int NumberSpawnedAtOnce;

        public float SpawnDelay;

        private List<GameObject> spawnedObjects = new List<GameObject>();


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            FoodTypesQueue.Enqueue(FoodTypes.Burger);
            FoodTypesQueue.Enqueue(FoodTypes.Burger);
            FoodTypesQueue.Enqueue(FoodTypes.Sushi);
            FoodTypesQueue.Enqueue(FoodTypes.Udon);
            FoodTypesQueue.Enqueue(FoodTypes.Pancake);
            FoodTypesQueue.Enqueue(FoodTypes.Pizza);

            for (int i = 0; i < NumberSpawnedAtOnce; i++)
            {
                SpawnNextAtPosition(i);
            }
        }

        private void SpawnNextAtPosition(int index)
        {
            if (FoodTypesQueue.Count == 0) return;

            FoodTypes nextType = FoodTypesQueue.Dequeue();
            GameObject prefab = FoodTypesMapping[nextType];
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab for {nextType} is not assigned!");
                return;
            }

            Vector3 spawnPos = transform.position + transform.forward * (index * Offset);
            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

            // Unity added this stupidly long class name when I used var and it recompiled so I'm leaving it be
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = instance.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            interactable.selectEntered.AddListener(_ => this.OnFoodPickedUp(instance));

            spawnedObjects.Add(instance);
        }

        public void OnFoodPickedUp(GameObject pickedObject)
        {
            if (!spawnedObjects.Contains(pickedObject)) return;

            spawnedObjects.Remove(pickedObject);

            StartCoroutine(SlideAndSpawn());
        }

        private IEnumerator SlideAndSpawn()
        {
            // Wait spawn delay before sliding objects and then do it again later
            yield return new WaitForSeconds(SpawnDelay);

            // Slide remaining objects backward to new targets as coroutines
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                Vector3 targetPos = transform.position + transform.forward * (i * Offset);
                StartCoroutine(MoveToPosition(spawnedObjects[i].transform, targetPos, 0.25f));
            }

            yield return new WaitForSeconds(SpawnDelay);

            // Spawn new one at the end
            SpawnNextAtPosition(spawnedObjects.Count);
        }

        private IEnumerator MoveToPosition(Transform obj, Vector3 target, float duration)
        {
            Vector3 start = obj.position;
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                obj.position = Vector3.Lerp(start, target, t / duration);
                // This is just worse Update() logic but binds loops to the framerate to stop it from essentially teleporting objects
                yield return null;
            }
            obj.position = target;
        }
    }
    
[CustomEditor(typeof(FoodSpawner))]
public class FoodSpawnerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var spawner = (FoodSpawner)target;

            foreach (FoodTypes foodType in spawner.FoodTypesMapping)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(foodType.ToString(), GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                spawner.FoodTypesMapping[foodType] = (GameObject)EditorGUILayout.ObjectField(spawner.FoodTypesMapping[foodType], typeof(GameObject), false);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(spawner);
                }
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.LabelField("Item Tiling Offset", GUILayout.Width(100));
            spawner.Offset = EditorGUILayout.FloatField(spawner.Offset);

            EditorGUILayout.LabelField("Spawn Delay", GUILayout.Width(100));
            spawner.SpawnDelay = EditorGUILayout.FloatField(spawner.SpawnDelay);

            EditorGUILayout.LabelField("Number Spawned at Once", GUILayout.Width(500));
            spawner.NumberSpawnedAtOnce = EditorGUILayout.IntField(spawner.NumberSpawnedAtOnce);
    }
}
}
