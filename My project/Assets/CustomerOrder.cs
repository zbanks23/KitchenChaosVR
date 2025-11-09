using UnityEngine;
using TMPro;

public class CustomerOrder : MonoBehaviour
{
    public enum Meal
    {
        Burger,
        Curry,
        HotDog,
        Pancake,
        Pizza,
        Sushi,
        Udon
    }

    private Meal desiredMeal;

    public TextMeshProUGUI textBox;
    public Sprite[] mealIcons;

    public Meal GetDesiredMeal() {
        return desiredMeal;
    }

    public void GenerateOrder() {
        desiredMeal = (Meal)Random.Range(0, System.Enum.GetValues(typeof(Meal)).Length);

        if (textBox != null) {
            textBox.text = desiredMeal.ToString();
        } else {
            Debug.LogWarning("TextBox is not assigned in CustomerOrder script.");
        }
    }
}
