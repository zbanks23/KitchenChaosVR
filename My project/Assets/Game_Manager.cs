using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game_Manager : MonoBehaviour {
    // --- FIX 1: Add Singleton pattern ---
    public static Game_Manager Instance { get; private set; }

    public int points;
    public int timer = 0;
    public bool first = true;
    public TextMeshProUGUI pointDisplay;
    public int pointReq = 20;

    // --- FIX 1 (continued): Add Awake method for Singleton ---
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    void Start() {
        // Use the function to set points, not add them
        SetPoints(0);
    }

    void Update() {
        if (points > pointReq) {
            LoadScene("BreakScene");
        }
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    // --- FIX 2: Correct the logic ---
    // Renamed for clarity, but you can keep "addPoints"
    public void AddPoints(int pointsToAdd) {
        this.points += pointsToAdd; // Use += to add, not =
        pointDisplay.text = "Points: " + this.points; // Update with the new total
    }

    // A helper function to set points at the start
    public void SetPoints(int total) {
        this.points = total;
        pointDisplay.text = "Points: " + this.points;
    }
}