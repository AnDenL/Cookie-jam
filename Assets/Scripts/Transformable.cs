using System.Collections;
using Creatures;
using UnityEngine;

public class Transformable : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    
    public void SetSoul(AIController controller)
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(Transform(controller));
    }

    private IEnumerator Transform(AIController controller)
    {
        float t = 1;

        while (t > 0)
        {
            yield return null;
        }

        prefab.SetActive(false);
        var obj = Instantiate(prefab, transform.position, Quaternion.identity);
        obj.GetComponent<Creature>().SetController(controller);
        prefab.SetActive(true);

        while (t < 1)
        {
            
            yield return null;
        }
    }
}
