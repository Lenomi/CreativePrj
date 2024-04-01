using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC : NPCBase
{
    [HideInInspector]
    public bool InLineUp;

    public NodeClassicInLine LastNodeInLineUp;
    public float DurationTakeTicket = 3f;
    public float DurationToLeaveTicketCounter = 0.5f;
    protected NodeClassicInLine _nextNode;

    protected readonly string ANIMATION_IDLE = "Idle";
    protected readonly string ANIMATION_MOVE = "Walk";
    protected readonly string ANIMATION_TAKE_TICKET = "Idle1";
    [HideInInspector]
    public StateWaitingTakeTicket CurrentWaitingTakeTicket = StateWaitingTakeTicket.None;
    public float DurationToWaitForTakeTicket = 2f;
    public Transform AppearPoint;

    private Vector3 _initPoint;
    private Vector3 _initEulerAngles;

    protected override void Awake()
    {
        base.Awake();
        _initPoint = transform.position;
        _initEulerAngles = transform.eulerAngles;
    }

    public override void Init(Map map)
    {
        base.Init(map);
        if (InLineUp)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Collider");
            if (LastestNode != null)
            {
                NodeClassicInLine node = LastestNode as NodeClassicInLine;
                node.IsUsing = true;
            }
            if(LastNodeInLineUp == null)
            {
                CurrentWaitingTakeTicket = StateWaitingTakeTicket.Hide;
                IsRunning = false;
                gameObject.SetActive(false);
            }
        }
    }

    public override void UpdateLogic(float deltaTime)
    {
        if (!IsRunning)
            return;
        if (CurrentWaitingTakeTicket == StateWaitingTakeTicket.Appear)
        {
            var dir = (_initPoint - transform.position).normalized;
            var s = dir * Speed * deltaTime;
            var prePosition = transform.position;
            transform.position += s;
            dir.y = 0;
            dir = dir.normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), SpeedRotate * deltaTime);
            if (Vector3.Distance(_initPoint, transform.position) <= 0.1f)
            {
                _lastPosition = _initPoint;
                _lastEulerAngles = _initEulerAngles;
                CurrentWaitingTakeTicket = StateWaitingTakeTicket.Wait;
                _cooldownTimeToNextAction = DurationToWaitForTakeTicket;
                _cooldownTimeToCheckSmooth = 0.5f;
                PlayAnimationIdle();
            }
            return;
        }
        else if (CurrentWaitingTakeTicket == StateWaitingTakeTicket.Wait)
        {
            _cooldownTimeToNextAction -= deltaTime;
            if (_cooldownTimeToNextAction <= 0)
            {
                _cooldownTimeToNextAction = 0;
                CurrentWaitingTakeTicket = StateWaitingTakeTicket.None;
            }
            SmoothTransform(deltaTime);
            return;
        }
        if (InLineUp)
        {
            if(_currentState == State.Idle)
            {
                if(_nextNode != null)
                {
                    _cooldownTimeToNextAction -= deltaTime;
                    if(_cooldownTimeToNextAction <= 0)
                    {
                        (LastestNode as NodeClassicInLine).IsUsing = false;
                        _nextNode.IsUsing = true;
                        Move(new List<NodeClassicInGame>() { _nextNode });
                    }
                    return;
                }
                if(LastestNode == null)
                {
                    NodeClassicInLine node = null;
                    var index = -1;
                    for (int i = 0; i < _map.StartNodeInLineUp.Length; i++)
                    {
                        var nodeTemp = _map.StartNodeInLineUp[i];
                        if (!nodeTemp.IsUsing)
                        {
                            index = i;
                            node = nodeTemp;
                            break;
                        }
                    }
                    if (node != null)
                    {
                        node.IsUsing = true;
                        LastestNode = node;
                        NodeClassicInLine nextNode = LastestNode.Dependencies[0] as NodeClassicInLine;
                        while (nextNode != null)
                        {
                            LastNodeInLineUp = nextNode;
                            if (LastNodeInLineUp == _map.LastNodeInLineUp[index])
                            {
                                break;
                            }
                            nextNode = nextNode.Dependencies[0] as NodeClassicInLine;
                        }
                    }
                }
                else
                {

                    NodeClassicInLine node = null;
                    NodeClassicInLine nextNode = LastestNode.Dependencies[0] as NodeClassicInLine;
                    while (nextNode != null)
                    {
                        if (!nextNode.IsUsing)
                        {
                            node = nextNode;
                            if (node == LastNodeInLineUp)
                            {
                                break;
                            }
                            nextNode = nextNode.Dependencies[0] as NodeClassicInLine;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (node != null)
                    {
                        _cooldownTimeToNextAction = DurationToLeaveTicketCounter;
                        _nextNode = node;
                    }
                }
            }
            else if (_currentState == State.Move)
            {
                UpdateMove(deltaTime);
            }
            else if(_currentState == State.Doing)
            {
                _cooldownTimeToNextAction -= deltaTime;
                if(_cooldownTimeToNextAction <= 0)
                {
                    _cooldownTimeToNextAction = 0;
                    FindDestination.Invoke();
                }
            }
            SmoothTransform(deltaTime);
        }
        else
        {
            base.UpdateLogic(deltaTime);
        }
    }

    public override void Move(List<NodeClassicInGame> path)
    {
        if (InLineUp)
        {
            _nextNode = null;
            if(_currentState == State.Doing)
            {
                InLineUp = false;
                (LastestNode as NodeClassicInLine).IsUsing = false;
            }
        }
        base.Move(path);
        if (InLineUp)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Collider");
        }
    }

    protected override void MoveDone()
    {
        if (InLineUp)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Collider");
            if (LastNodeInLineUp == LastestNode)
            {
                _currentState = State.Doing;
                _cooldownTimeToNextAction = DurationTakeTicket;
            }
            else
            {
                _currentState = State.Idle;
            }
            PlayAnimation();
        }
        else
        {
            base.MoveDone();
        }
    }

    protected override void PlayAnimation()
    {
        if (_animator == null) return;
        if (!string.IsNullOrEmpty(_currentAnimation))
        {
            _animator.SetBool(_currentAnimation, false);
        }
        
        if (_currentState == State.Idle)
        {
            _currentAnimation = ANIMATION_IDLE;
        }
        else if(_currentState == State.Move)
        {
            _currentAnimation = ANIMATION_MOVE;
        }
        else if(_currentState == State.Doing)
        {
            if (InLineUp)
            {
                _currentAnimation = ANIMATION_TAKE_TICKET;
            }
            else
            {
                if(Destination != null)
                {
                    _currentAnimation = Destination.Animations[UnityEngine.Random.RandomRange(0, Destination.Animations.Length)];
                }
            }
        }
        if (string.IsNullOrEmpty(_currentAnimation))
        {
            _currentAnimation = ANIMATION_IDLE;
        }
        _animator.SetBool(_currentAnimation, true);
    }

    public void Appear()
    {
        if(CurrentWaitingTakeTicket == StateWaitingTakeTicket.Hide)
        {
            IsRunning = true;
            CurrentWaitingTakeTicket = StateWaitingTakeTicket.Appear;
            transform.position = AppearPoint.transform.position;
            gameObject.SetActive(true);

            PlayAnimationWalk();
        }
    }

    private void PlayAnimationIdle()
    {
        if (_animator == null) return;
        if (!string.IsNullOrEmpty(_currentAnimation))
        {
            _animator.SetBool(_currentAnimation, false);
        }
        _currentAnimation = ANIMATION_IDLE;
        _animator.SetBool(_currentAnimation, true);
    }

    private void PlayAnimationWalk()
    {
        if (_animator == null) return;
        if (!string.IsNullOrEmpty(_currentAnimation))
        {
            _animator.SetBool(_currentAnimation, false);
        }
        _currentAnimation = ANIMATION_MOVE;
        _animator.SetBool(_currentAnimation, true);
    }

    public enum StateWaitingTakeTicket
    {
        None,
        Hide,
        Appear,
        Wait,
    }
}
