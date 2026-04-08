using UnityEngine;

public class NewHomeManager : MonoBehaviour
{
    [SerializeField]
    private GameManagerInfinity gameManager;
    [SerializeField]
    private HomeMenu homeMenu;
    [SerializeField]
    private Camera tempCamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Play()
    {
        homeMenu.Disable();
        gameManager.StartGame();
    }
}
