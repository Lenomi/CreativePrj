using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUIForObject3D : MonoBehaviour
{
    public string Path = "Prefabs/UI/UIFollowTarget";
    // Start is called before the first frame update
    void Start()
    {
        var go = Instantiate(Resources.Load<GameObject>(Path));
        go.transform.SetParent(UIManager.Instance.Container);
        var ui = go.GetComponent<UIFollowTarget>();
        ui.Init(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
