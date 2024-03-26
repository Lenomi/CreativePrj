using System.Collections.Generic;
using UnityEngine;

public class PetDemoCameraLogic : MonoBehaviour
{
    private float m_distance = 1.5f;
    private float m_height = 0.5f;
    private float m_lookAtAroundAngle = 180;

    [SerializeField] private Transform[] m_targetTransforms = default;

    private struct CameraTarget
    {
        public Transform Transform;
        public Animator Animator;
        public List<string> AnimationTriggers;
    }

    private List<CameraTarget> m_targets;
    private CameraTarget m_currentTarget = default;
    private int m_targetIndex = 0;

    private void Start()
    {
        if (m_targetTransforms.Length == 0) { return; }

        InitializeCameraTargets();

        ChangeTarget(0);
    }

    private void InitializeCameraTargets()
    {
        m_targets = new List<CameraTarget>();

        for (int i = 0; i < m_targetTransforms.Length; i++)
        {
            CameraTarget target = new CameraTarget();
            target.Transform = m_targetTransforms[i];
            target.Animator = m_targetTransforms[i].GetComponent<Animator>();
            AnimatorControllerParameter[] parameters = target.Animator.parameters;

            target.AnimationTriggers = new List<string>();

            for (int j = 0; j < parameters.Length; j++)
            {
                if (parameters[j].type == AnimatorControllerParameterType.Trigger)
                {
                    target.AnimationTriggers.Add(parameters[j].name);
                }
            }

            m_targets.Add(target);
        }
    }

    public void NextTarget()
    {
        ChangeTarget(1);
    }

    public void PreviousTarget()
    {
        ChangeTarget(-1);
    }

    private void ChangeTarget(int step)
    {
        if (m_targetTransforms.Length == 0) { return; }
        m_targetIndex += step;
        if (m_targetIndex > m_targets.Count - 1) { m_targetIndex = 0; }
        if (m_targetIndex < 0) { m_targetIndex = m_targets.Count - 1; }
        m_currentTarget = m_targets[m_targetIndex];
    }

    private void Update()
    {
        if (m_targetTransforms.Length == 0) { return; }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PreviousTarget();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            NextTarget();
        }
    }

    private void LateUpdate()
    {
        if (m_targetTransforms.Length == 0) { return; }

        float targetHeight = m_currentTarget.Transform.position.y + m_height;
        float currentRotationAngle = m_lookAtAroundAngle;

        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        Vector3 position = m_currentTarget.Transform.position;
        position -= currentRotation * Vector3.forward * m_distance;
        position.y = targetHeight;

        transform.position = position;
        transform.LookAt(m_currentTarget.Transform.position + new Vector3(0, m_height, 0));
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width));

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Previous character (Q)"))
        {
            PreviousTarget();
        }

        if (GUILayout.Button("Next character (E)"))
        {
            NextTarget();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(16);

        for (int i = 0; i < m_currentTarget.AnimationTriggers.Count; i++)
        {
            if (i == 0) { GUILayout.BeginHorizontal(); }

            if (GUILayout.Button(m_currentTarget.AnimationTriggers[i]))
            {
                m_currentTarget.Animator.SetTrigger(m_currentTarget.AnimationTriggers[i]);
            }

            if (i == m_currentTarget.AnimationTriggers.Count - 1) { GUILayout.EndHorizontal(); }
        }

        GUILayout.Space(16);

        Color oldColor = GUI.color;
        GUI.color = Color.black;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        GUILayout.Label("WASD or arrows: Move");
        GUILayout.Label("Left Shift: Walk");
        GUILayout.Label("Space: Jump");
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        GUILayout.Label("Z: Toggle sit");
        GUILayout.Label("X: Toggle eat");
        GUILayout.Label("C: Toggle sleep");
        GUILayout.Label("V: Toggle lie down");
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUI.color = oldColor;

        GUILayout.EndVertical();
    }
}
