using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    void OnMouseDown()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
