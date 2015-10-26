using UnityEngine;
using System.Collections;

public class CompBuffRange : MonoBehaviour
{
    private CompController _cc;
    private ConnectionLine _line;

    private bool _once;

    private bool _heal;

    void Start()
    {
        _cc = GetComponentInParent<CompController>();
        _line = _cc.transform.FindChild("Line").gameObject.GetComponent<ConnectionLine>();

        
    }

    void AllowHeal()
    {
        _heal = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.AddComputerInPlayerBuffArea(GameManager.GetPlayer(), _cc);

            if (GameManager.IsComputerHacked(_cc))
            {
                _line.SetDestination(GameManager.GetPlayer().transform);

                if(_cc.IsDehackInProgress) return;

                //TODO: dynamic line animation times
                _line.AnimateLine(Enums.AnimType.FromOriginToDestination, .25f);
            }

            Invoke("AllowHeal", 1);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.IsComputerHacked(_cc))
        {
            if (_heal)
            {
                HealWave();
            }
        }
    }

    void HealWave()
    {
        GameManager.GetPlayer().AddHealth(10);
        GameManager.GetPlayer().AddEnergy(10);
        _heal = false;
        Invoke("AllowHeal", 1);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.RemoveComputerInPlayerBuffArea(GameManager.GetPlayer(), _cc);

            if (GameManager.IsComputerHacked(_cc) && !_cc.IsDehackInProgress)
            {
                AnimateDisconnect();
            }

            CancelInvoke();
        }
    }

    public void AnimateDisconnect()
    {
        var trs = new GameObject().transform;
        trs.position = new Vector3(GameManager.GetPlayer().transform.position.x, GameManager.GetPlayer().transform.position.y, GameManager.GetPlayer().transform.position.z);

        _line.SetDestination(trs);

        //TODO: dynamic line animation times
        _line.AnimateLine(Enums.AnimType.FromDestinationToOrigin, .25f);
    }
}
