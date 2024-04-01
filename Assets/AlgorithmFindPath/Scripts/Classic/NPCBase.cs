using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCBase : MoveableObject
{
    [HideInInspector]
    public Obstacle Destination;
    public NodeClassicInGame LastestNode;
    public float DurationIdleFrom = 0.5f;
    public float DurationIdleTo = 2f;
    public float DurationDoingFrom = 2f;
    public float DurationDoingTo = 5f;
    protected State _currentState = State.Idle;
    public State CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    protected float _cooldownTimeToNextAction = 2;
    private NodeClassicInGame _destinationNode;
    protected float _cooldownTimeToStart = 0;

    public Action FindDestination;

    protected string _currentAnimation;
    protected Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
    }


    public override void Init(Map map)
    {
        base.Init(map);
        //_cooldownTimeToStart = UnityEngine.Random.RandomRange(DelayStartFrom, DelayStartTo);
    }

    public override void UpdateLogic(float deltaTime)
    {
        if (!IsRunning)
            return;
        if(_cooldownTimeToStart > 0)
        {
            _cooldownTimeToStart -= deltaTime;
            return;
        }
        
        if(_currentState == State.Idle)
        {
            if (LastestNode != null)
            {
                if (_cooldownTimeToNextAction > 0)
                {
                    _cooldownTimeToNextAction -= Time.deltaTime;
                }
                if (_cooldownTimeToNextAction <= 0)
                {
                    _cooldownTimeToNextAction = 0;
                    FindDestination?.Invoke();
                }
            }  
        }
        else if(_currentState == State.Move)
        {
            UpdateMove(deltaTime);
        }
        else if(_currentState == State.Doing)
        {
            if (_cooldownTimeToNextAction > 0)
            {
                _cooldownTimeToNextAction -= Time.deltaTime;
            }
            if (_cooldownTimeToNextAction <= 0)
            {
                _cooldownTimeToNextAction = 0;
                DoingDone();
            }
        }
        SmoothTransform(deltaTime);
    }

    protected override void RemoveNodeAfterPass(NodeClassicInGame nodeClassicInGame)
    {
        LastestNode = nodeClassicInGame;
    }

    public override void Move(List<NodeClassicInGame> path)
    {
        base.Move(path);
        _destinationNode = path[path.Count - 1];
        _currentState = State.Move;
        PlayAnimation();
    }

    protected override void MoveDone()
    {
        _currentState = State.Doing;
        _cooldownTimeToNextAction = (float)_map.GetRandomFloat(DurationDoingFrom, DurationDoingTo);
        PlayAnimation();
    }

    protected void DoingDone()
    {
        _currentState = State.Idle;
        _cooldownTimeToNextAction = (float)_map.GetRandomFloat(DurationIdleFrom, DurationIdleTo);
        PlayAnimation();
    }

    public void RemoveUsingNode()
    {
        if(Destination != null)
        {
            Destination.RemoveUsingNode(_destinationNode);
        }
    }

    protected virtual void PlayAnimation()
    {
        
    }

    public enum State
    {
        Idle,
        Move,
        Doing,
    }
}
