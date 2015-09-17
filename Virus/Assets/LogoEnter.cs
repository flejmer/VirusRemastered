using UnityEngine;
using System.Collections;

public class LogoEnter : MonoBehaviour
{
    public Transform StartPosition;
    public Transform AimPosition;
    public float Duration = 4;

    private void Start()
    {
        StartCoroutine(Enter());
    }

    IEnumerator Enter()
    {
        float progress = 0;

        while (progress < 1)
        {
            progress += Time.deltaTime/Duration;
            transform.position = Vector3.Lerp(StartPosition.position, AimPosition.position, progress);
            yield return null;
        }
    }
}
