using UnityEngine;
using UnityEngine.SceneManagement;

public static class MySceneManager
{
    public static void LoadScene (string name)
    {
        SaveSystem.ChangeScene();
        SceneManager.LoadScene(name);
    }
}
