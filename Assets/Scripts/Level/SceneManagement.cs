using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void RestartScene()
    {
        TimeManager.ResetTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
