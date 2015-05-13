using UnityEngine;
using System.Collections;

public class SomeScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            DelegateTests.OnPlayerInRangeEvent(other.gameObject, gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            DelegateTests.OnPlayerOutOfRangeEvent(other.gameObject, gameObject);
    }
}
