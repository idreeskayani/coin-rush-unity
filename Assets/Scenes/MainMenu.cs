using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Text bestText;
    public string gameScene = "SampleScene";

    void Start()
    {
        Time.timeScale = 1f;
        bestText.text = "BEST " + PlayerPrefs.GetInt("best", 0);
    }

    public void StartGame() { SceneManager.LoadScene(gameScene); }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}