using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void TitleSceneChange()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void GameMainSceneChange()
    {
        SceneManager.LoadScene("GameMainScene");
    }

    public void GameClearSceneChange()
    {
        SceneManager.LoadScene("GameClearScene");
    }
    public void GameOverSceneChange()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}
