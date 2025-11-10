using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;

[Serializable]
public class EnumObjectMapping<TEnum> : IEnumerable where TEnum : struct, Enum {
    [SerializeField]
    private List<TEnum> enumList = new List<TEnum>();
    [SerializeField]
    private List<GameObject> gameObjectList = new List<GameObject>();

    public EnumObjectMapping() {
        enumList = new List<TEnum>((TEnum[])Enum.GetValues(typeof(TEnum)));
        gameObjectList = new List<GameObject>(enumList.Count);
        for (int i = 0; i < enumList.Count; i++) {
            gameObjectList.Add(null);
        }
    }

    public GameObject this[TEnum index] {
        get => gameObjectList[(int)(object)index];
        set => gameObjectList[(int)(object)index] = value;
    }

    public IEnumerator GetEnumerator() => enumList.GetEnumerator();
}


public class FoodSpawner : MonoBehaviour {
    public static FoodSpawner Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    [SerializeField]
    public EnumObjectMapping<CustomerOrder.Meal> FoodTypesMapping = new EnumObjectMapping<CustomerOrder.Meal>();
    public Queue<CustomerOrder.Meal> FoodTypesQueue { get; } = new Queue<CustomerOrder.Meal>();

    public float Offset;
    public int NumberSpawnedAtOnce;
    public float SpawnDelay;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start() {
        for (int i = 0; i < NumberSpawnedAtOnce; i++) {
            SpawnNextAtPosition(i);
        }
    }

    public void RequestFood(CustomerOrder.Meal mealType) {
        FoodTypesQueue.Enqueue(mealType);
        Debug.Log("Food Spawner: Added " + mealType.ToString() + " to the queue.");
    }

    private void SpawnNextAtPosition(int index) {
        if (FoodTypesQueue.Count == 0) {
            Debug.Log("Food Spawner: Queue is empty, waiting for new customer orders.");
            return;
        }

        CustomerOrder.Meal nextType = FoodTypesQueue.Dequeue();
        GameObject prefab = FoodTypesMapping[nextType];
        if (prefab == null) {
            Debug.LogWarning($"Prefab for {nextType} is not assigned!");
            return;
        }

        Vector3 spawnPos = transform.position + transform.forward * (index * Offset);
        GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = instance.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        interactable.selectEntered.AddListener(_ => this.OnFoodPickedUp(instance));

        spawnedObjects.Add(instance);
    }

    public void OnFoodPickedUp(GameObject pickedObject) {
        if (!spawnedObjects.Contains(pickedObject)) return;
        spawnedObjects.Remove(pickedObject);
        StartCoroutine(SlideAndSpawn());
    }

    private IEnumerator SlideAndSpawn() {
        yield return new WaitForSeconds(SpawnDelay);

        for (int i = 0; i < spawnedObjects.Count; i++) {
            Vector3 targetPos = transform.position + transform.forward * (i * Offset);
            StartCoroutine(MoveToPosition(spawnedObjects[i].transform, targetPos, 0.25f));
        }

        yield return new WaitForSeconds(SpawnDelay);
        SpawnNextAtPosition(spawnedObjects.Count);
    }

    private IEnumerator MoveToPosition(Transform obj, Vector3 target, float duration) {
        Vector3 start = obj.position;
        float t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            obj.position = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }
        obj.position = target;
    }
}