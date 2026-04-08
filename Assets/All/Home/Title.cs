using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Title : MonoBehaviour
{
    [SerializeField]
    private Animation m_animation;

    public Image image { get => GetComponent<Image>(); }

    public void PlayAnimation()
    {
        m_animation.Play();
    }
}
