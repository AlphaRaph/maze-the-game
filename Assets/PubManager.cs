using System.Collections;
using UnityEngine;

public class PubManager : MonoBehaviour
{
    [SerializeField]
    private MapGenerator generator;
    [SerializeField]
    private Animation anim;


    private void Awake()
    {
        generator.UpdateMap(generator.MazeSize, generator.Seed, generator.LevelOfDetail);
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        yield return new WaitForSeconds(10);

        anim.Play();
    }
}
