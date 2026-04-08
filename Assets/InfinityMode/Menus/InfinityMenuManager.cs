using UnityEngine;

public class InfinityMenuManager : MonoBehaviour
{
    // References
    [SerializeField]
    private InfinityMenu m_infinityMenu;

    private void Start()
    {
        MySound.instance.PlayInfinity();
        m_infinityMenu.Enable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MySceneManager.LoadScene("Home");
        }
    }
}
