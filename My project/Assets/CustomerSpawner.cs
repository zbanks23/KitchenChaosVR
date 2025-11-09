using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customers;


    Dictionary<Transform, bool> isSeatTaken = new Dictionary<Transform, bool>();
    Dictionary<GameObject, Transform> customerSeats = new Dictionary<GameObject, Transform>();

    // Takes a customer GameObject and sits them in a new seat
    void SeatCustomer(GameObject customer, Transform oldSeat = null)
    {
        Transform seat = transform.GetChild(Random.Range(0, transform.childCount));
        customer.transform.position = seat.position;
        customer.transform.rotation = seat.rotation;

        // Prevent customer from sitting in another customer's seat or in the same seat again
        if (isSeatTaken[seat] || seat == oldSeat)
        {
            SeatCustomer(customer, oldSeat);
            return;
        }

        // Updates the seating arrangements in both dictionaries
        customerSeats[customer] = seat;
        isSeatTaken[seat] = true;

        // Generate a new order when the customer gets seated
        customer.GetComponent<CustomerOrder>().GenerateOrder();

        // Plays correct animation on customer based on the tag attached to the seating position
        if (seat.gameObject.tag == "FaceLeft") customer.GetComponent<Animator>().Play("left facing seated");
        else customer.GetComponent<Animator>().Play("right facing seated");
    }

    // Takes a seated customer and moves them to a different spot
    void MoveCustomer(GameObject customer)
    {
        Transform oldSeat = customerSeats[customer];
        isSeatTaken[oldSeat] = false;
        SeatCustomer(customer, oldSeat);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform seat in transform)
        {
            isSeatTaken[seat] = false;
        }

        foreach (GameObject customer in customers)
        {
            SeatCustomer(customer);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //     {
    //         MoveCustomer(customers[Random.Range(0, customers.Length)]);
    //     }
    // }
}
