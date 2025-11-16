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
    private GameObject gameController;

    void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    public void RequestFood(CustomerOrder.Meal mealType) {
        FoodTypesQueue.Enqueue(mealType);
        Debug.Log("Food Spawner: Added " + mealType.ToString() + " to the queue.");

        if (spawnedObjects.Count < NumberSpawnedAtOnce) {
            SpawnNextAtPosition(spawnedObjects.Count);
        }
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

        // This check prevents errors if a prefab is missing the component
        if (interactable != null) {
            interactable.selectEntered.AddListener(_ => {
                CustomerArrow arrowComponent = gameController.GetComponent<CustomerArrow>();
                // Add null checks for safety
                if (arrowComponent != null) {
                    arrowComponent.PickedUpObject(instance);
                }
                this.OnFoodPickedUp(instance);
            });

            interactable.selectExited.AddListener(_ => {
                CustomerArrow arrowComponent = gameController.GetComponent<CustomerArrow>();
                if (arrowComponent != null) {
                    arrowComponent.DroppedObject(instance);
                }
            });
        } else {
            Debug.LogWarning($"Spawned object {prefab.name} is missing an XRGrabInteractable component!");
        }

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
            // Check if object was destroyed while waiting (e.g., delivered)
            if (spawnedObjects[i] != null) {
                StartCoroutine(MoveToPosition(spawnedObjects[i].transform, targetPos, 0.25f));
            }
        }

        yield return new WaitForSeconds(SpawnDelay);
        // Check if there's anything in the queue before spawning
        if (FoodTypesQueue.Count > 0) {
            SpawnNextAtPosition(spawnedObjects.Count);
        }
    }

    private IEnumerator MoveToPosition(Transform obj, Vector3 target, float duration) {
        // Check if object is still valid
        if (obj == null) yield break;

        Vector3 start = obj.position;
        float t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            // Check again in loop
            if (obj == null) yield break;
            obj.position = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }
        // Final check
        if (obj != null) {
            obj.position = target;
        }
    }
}