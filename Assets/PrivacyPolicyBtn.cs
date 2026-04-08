using UnityEngine;

public class PrivacyPolicyBtn : MonoBehaviour
{
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://maze-the-game.web.app/privacy-policy.html");
    }
}
