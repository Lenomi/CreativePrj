using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleStartAppearNPC : MonoBehaviour
{
    [SerializeField]
    Map map;
    // Start is called before the first frame update
    void Start()
    {
        if(map != null)
        {
            map.StartAppearNPCs();
        }
    }
}
