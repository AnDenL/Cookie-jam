using UnityEngine;

public class TUTORIAL : MonoBehaviour
{
    private static bool ShowTutor = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowTutor = !ShowTutor;
            transform.GetChild(0).gameObject.SetActive(ShowTutor);
        }
    }
}