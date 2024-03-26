using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class UIFollowTarget : MonoBehaviour
{
    [SerializeField]
    protected GameObject Container;

    protected GameObject _target;
    [SerializeField]
    protected Vector3 _offset;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void LateUpdate()
    {
        UpdatePosition();
    }

    protected void UpdatePosition()
    {
        if (_target != null && Camera.main != null)
        {
            var targetPosition = _target.transform.position;
            if (_offset.sqrMagnitude > 0)
            {
                targetPosition = targetPosition + _target.transform.rotation * _offset * _target.transform.lossyScale.y;
            }
            transform.position = Camera.main.WorldToScreenPoint(targetPosition);
        }
    }

    public virtual void Init(GameObject target)
    {
        _target = target;
    }
}
