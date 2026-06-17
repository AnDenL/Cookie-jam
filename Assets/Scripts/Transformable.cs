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
            t -= Time.deltaTime * 4;

            transform.localScale = new Vector3(t, 1, 1);

            yield return null;
        }

        prefab.SetActive(false);
        var obj = Instantiate(prefab, transform.position, Quaternion.identity);
        obj.GetComponent<Creature>().SetController(controller);
        prefab.SetActive(true);
        obj.SetActive(true);

        while (t < 1)
        {
            t += Time.deltaTime * 4;

            obj.transform.localScale = new Vector3(t, 1, 1);

            yield return null;
        }

        Destroy(gameObject);
    }
}
