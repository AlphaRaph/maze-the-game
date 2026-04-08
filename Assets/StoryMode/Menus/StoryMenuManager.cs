using UnityEngine;

public class StoryMenuManager : MonoBehaviour
{
    private void Start()
    {
        MySound.instance.PlayStory();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoHome();
        }
    }

    public void GoHome()
    {
        MySound.ButtonSound();
        MySceneManager.LoadScene("Home");
    }
}
