using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GuardActivator : MonoBehaviour
{
    public List<EnemyGuardAI> GuardList = new List<EnemyGuardAI>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var ai in GuardList.Where(ai => !ai.PlayerControlled))
            {
                ai.StartChase();
            }
        }
        else if(other.CompareTag("Lightning"))
        {
            var script = other.gameObject.GetComponent<SimpleMissileController>();

            if (!script.WhoFired.Equals(GameManager.GetPlayer().gameObject)) return;

            foreach (var ai in GuardList.Where(ai => !ai.PlayerControlled))
            {
                ai.StartChase();
            }
        }
    }
}
