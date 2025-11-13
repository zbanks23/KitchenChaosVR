using UnityEngine;

public class CustomerArrow : MonoBehaviour
{
    CustomerSpawner customerSpawner;
    CustomerOrder.Meal? heldMeal = null;
    Transform targetCustomerTransform = null;
    Transform playerTransform;

    public GameObject arrow;
    MeshRenderer arrowMesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        customerSpawner = FindFirstObjectByType<CustomerSpawner>();
        arrowMesh = arrow.GetComponent<MeshRenderer>();
        arrowMesh.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCustomerTransform != null)
        {
            OrientArrow();
        }
    }

    public void PickedUpObject(GameObject objectPicked)
    {
        var item = objectPicked.GetComponent<FoodItem>();
        heldMeal = item.foodType;
        foreach (var customer in customerSpawner.customers)
        {
            var order = customer.GetComponent<CustomerOrder>();
            if (order.GetDesiredMeal() == heldMeal)
            {
                targetCustomerTransform = customer.transform;
                break;
            }
        }

        if (targetCustomerTransform != null)
        {
            OrientArrow();
            arrowMesh.enabled = true;
        }
    }

    void OrientArrow()
    {
        Vector3 dir = targetCustomerTransform.position - playerTransform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
        arrow.transform.rotation = lookRot;
    }

    public void DroppedObject(GameObject objectDropped)
    {
        arrowMesh.enabled = false;
        targetCustomerTransform = null;
        heldMeal = null;
    }
}
