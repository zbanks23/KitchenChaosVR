using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public void Transition()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
