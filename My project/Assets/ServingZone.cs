using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class ServingZone : MonoBehaviour
{
    private CustomerAI ownerCustomer;
    private CustomerOrder.Meal requiredFood;

    public AudioResource correctOrderSoundEffect;
    public AudioResource incorrectOrderSoundEffect;
    public AudioSource audioSource;

    public int pointsPerDelivery = 1;

    public void InitializeZone(CustomerAI owner, CustomerOrder.Meal desiredFood)
    {
        ownerCustomer = owner;
        requiredFood = desiredFood;
        Debug.Log("Zone initialized, waiting for: " + desiredFood.ToString());
    }

    private IEnumerator PlaySoundAndDestroy(FoodItem item, CustomerAI customer)
    {
        audioSource.resource = correctOrderSoundEffect;
        audioSource.Play();

        Debug.Log("Correct food delivered!");

        if (Game_Manager.Instance != null) {
            Game_Manager.Instance.AddPoints(pointsPerDelivery);
        }

        customer.ReceiveFood(item.gameObject);

        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(this.gameObject);
    }

    // This is called by the FoodItem when it's dropped in
    public void TryAcceptFood(FoodItem item)
    {
        if (ownerCustomer == null)
        {
            Debug.LogError("This zone has no owner!");
            return;
        }

        if (item.GetFoodType() == requiredFood)
        {
            StartCoroutine(PlaySoundAndDestroy(item, ownerCustomer));
        }
        else
        {
            Debug.Log("Wrong food! Wanted " + requiredFood.ToString() + ", got " + item.GetFoodType().ToString());

            audioSource.resource = incorrectOrderSoundEffect;
            audioSource.Play();
        }
    }
}