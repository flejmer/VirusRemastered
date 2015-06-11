using UnityEngine;
using System.Collections;

public class CompBuffRange : MonoBehaviour
{
    private CompController _cc;

    void Start()
    {
        _cc = GetComponentInParent<CompController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.AddComputerInPlayerBuffArea(GameManager.GetPlayer(), _cc);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.RemoveComputerInPlayerBuffArea(GameManager.GetPlayer(), _cc);
        }
    }
}
