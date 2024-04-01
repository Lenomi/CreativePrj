using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    private List<NodeClassicInGame> _path = new List<NodeClassicInGame>();
    public float Speed = 5;
    [HideInInspector]
    public float SpeedRotate = 8;
    [HideInInspector]
    public bool IsRunning = true;

    private Rigidbody _rigidbody;
    protected Map _map;
    protected Vector3 _colliderVector;
    protected float _stoppingDistance = 0.1f;

    protected Vector3 _lastPosition;
    protected Vector3 _lastEulerAngles;
    protected float _cooldownTimeToCheckSmooth;
    public int ID = 0;

    protected virtual void Awake()
    {
        ID = gameObject.GetHashCode();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    public virtual void Init(Map map)
    {
        _map = map;
        _rigidbody = GetComponent<Rigidbody>();
        _stoppingDistance = map.StoppingDistance;
        SpeedRotate = 16;
    }

    public virtual void UpdateLogic(float deltaTime)
    {
        if (!IsRunning)
            return;
        UpdateMove(deltaTime);
    }

    // Update is called once per frame
    protected void UpdateMove(float deltaTime)
    {
        if (_path.Count > 0)
        {
            var dir = ((_path[0].transform.position - transform.position).normalized + _colliderVector * 3).normalized;
            if (dir.magnitude <= 0)
            {
                dir = transform.forward;
            }
            var speed = Speed * ((_colliderVector.magnitude > 0) ? 1f : 1f);
            var s = dir * speed * deltaTime;
            var d = Vector3.Distance(transform.position + s, _path[0].transform.position);
            if (d <= _stoppingDistance)
            //if (Vector3.Dot(dir, _preDirection) < 0)
            {
                var point = _path[0];
                RemoveNodeAfterPass(_path[0]);
                _path.RemoveAt(0);
                if (_path.Count > 0)
                {
                    dir = ((_path[0].transform.position - transform.position).normalized + _colliderVector * 2).normalized;
                    if (dir.magnitude <= 0)
                    {
                        dir = transform.forward;
                    }
                    speed = Speed * ((_colliderVector.magnitude > 0) ? 0.5f : 1f);
                    s = dir * speed * deltaTime;
                    transform.position += s;
                    dir.y = 0;
                    dir = dir.normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), SpeedRotate * deltaTime);
                }
                else
                {
                    _lastPosition = point.transform.position;
                    _lastEulerAngles = point.transform.eulerAngles;
                    _cooldownTimeToCheckSmooth = 0.5f;
                    SmoothTransform(deltaTime);
                    if (_rigidbody != null)
                    {
                        _rigidbody.isKinematic = true;
                        _rigidbody.useGravity = false;
                        gameObject.layer = LayerMask.NameToLayer("Ignore Collider");
                    }
                    MoveDone();
                }
            }
            else
            {
                transform.position += s;
                dir.y = 0;
                dir = dir.normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), SpeedRotate * deltaTime);
            }
        }
    }

    protected void SmoothTransform(float deltaTime)
    {
        if (_cooldownTimeToCheckSmooth > 0)
        {
            transform.position = Vector3.Lerp(transform.position, _lastPosition, 16 * deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_lastEulerAngles), 16 * deltaTime);
            _cooldownTimeToCheckSmooth -= deltaTime;
            if(_cooldownTimeToCheckSmooth <= 0)
            {
                _cooldownTimeToCheckSmooth = 0;
            }
        }
    }

    protected virtual void RemoveNodeAfterPass(NodeClassicInGame nodeClassicInGame)
    {
        
    }

    public virtual void Move(List<NodeClassicInGame> path)
    {
        _path.Clear();
        _path.AddRange(path);
        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;
            gameObject.layer = LayerMask.NameToLayer("MoveableObject");
        }
    }

    protected virtual void MoveDone()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Ground")
        //    return;
        if (_rigidbody != null)
        {
            var hash1 = ID;
            var moveableObject2 = collision.gameObject.GetComponent<MoveableObject>();
            if (moveableObject2 == null)
                return;
            var hash2 = collision.gameObject.GetHashCode();
            hash2 = moveableObject2.ID;

            if (Speed > moveableObject2.Speed || (Mathf.Abs(Speed - moveableObject2.Speed) <= 0.01f && hash1 > hash2))
            {

            }
            else
            {
                var direct1 = GetDirection();
                var direct2 = moveableObject2.GetDirection();

                if (Vector3.Dot(direct1, direct2) > 0)
                {
                    direct2 = (collision.transform.position - transform.position).normalized;
                    if (Vector3.Dot(direct1, direct2) > 0)
                    {
                        direct2 = Quaternion.AngleAxis(90, Vector3.up) * direct1;
                        _colliderVector = (direct2).normalized;
                    }
                    else
                    {
                        _colliderVector = (direct2 + direct1).normalized;
                    } 
                }
                else
                {
                    direct2 = (transform.position - collision.transform.position).normalized;
                    _colliderVector = (direct2 + direct1).normalized;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.gameObject.tag == "Plane")
        //    return;

        if (_rigidbody != null)
        {
            _colliderVector = Vector3.zero;
        }
    }

    public Vector3 GetDirection()
    {
        if (_path.Count > 0)
        {
            return (_path[0].transform.position - transform.position).normalized;
        }
        else
        {
            return transform.forward;
        }
    }
}
