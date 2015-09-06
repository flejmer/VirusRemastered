using UnityEngine;
using System.Collections;

public class CompBuffRange : MonoBehaviour
{
    private CompController _cc;
    private ConnectionLine _line;

    void Start()
    {
        _cc = GetComponentInParent<CompController>();
        _line = GetComponentInParent<ConnectionLine>();
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.AddComputerInPlayerBuffArea(GameManager.GetPlayer(), _cc);

            if (GameManager.IsComputerHacked(_cc))
            {
                _line.SetDestination(GameManager.GetPlayer().transform);

                //TODO: dynamic line animation times
                _line.AnimateLine(Enums.AnimType.FromOriginToDestination, .25f);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.RemoveComputerInPlayerBuffArea(GameManager.GetPlayer(), _cc);

            if (GameManager.IsComputerHacked(_cc))
            {


                var trs = new GameObject().transform;
                trs.position = new Vector3(GameManager.GetPlayer().transform.position.x, GameManager.GetPlayer().transform.position.y, GameManager.GetPlayer().transform.position.z);

                _line.SetDestination(trs);
                
                //TODO: dynamic line animation times
                _line.AnimateLine(Enums.AnimType.FromDestinationToOrigin, .25f);
            }
        }
    }
}
