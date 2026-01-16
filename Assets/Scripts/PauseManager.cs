using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject SettingsMenu;
    public static bool isPaused = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if ( pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);


            if (hit.collider != null)
            {
                string name = hit.collider.gameObject.name;

                if(name == "PauseButton"){
                    TogglePause();
                }
                else if(name == "ResumeButton"){
                    ResumeGame();
                }
                else if(name == "ExitButton"){
                    ExitGame();
                }
                else if(name == "ExitLevel"){
                    ExitLevel();
                }
                else if(name == "Settings"){
                    SettingsMenu.SetActive(true);
                }
            }
        }
    }

    public void TogglePause()
    {
        if(SettingsMenu.activeSelf)
        {
            return;
        }
        isPaused = !isPaused;
        if (pauseMenu != null){
            pauseMenu.SetActive(isPaused);
        }
        Time.timeScale = isPaused ? 0f : 1f;

        
    }


    public void ResumeGame()
    {
        if(SettingsMenu.activeSelf)
        {
            return;
        }
        isPaused = false;
        if(pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Time.timeScale = 1f;
        
    }


    public void ExitGame()
    {
        if(SettingsMenu.activeSelf)
        {
            return;
        }
        Application.Quit();
        
    }

    public void ExitLevel()
    {
        if(SettingsMenu.activeSelf)
        {
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
