using UnityEngine;

public class PetDemoCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform m_target = default;

    [SerializeField] private float m_distance = 1.5f;
    [SerializeField] private float m_height = 0.5f;
    [SerializeField] private float m_lookAtAroundAngle = 180.0f;

    private float m_targetHeightOffset = 0.5f;

    private void LateUpdate()
    {
        if (m_target == null) { return; }

        float targetHeight = m_target.position.y + m_height;

        Quaternion currentRotation = Quaternion.Euler(0, m_lookAtAroundAngle, 0);

        Vector3 position = m_target.position;
        position -= currentRotation * Vector3.forward * m_distance;
        position.y = targetHeight;

        transform.position = position;
        transform.LookAt(m_target.position + new Vector3(0, m_targetHeightOffset, 0));
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width));

        GUILayout.Space(32);

        GUILayout.BeginHorizontal();

        Color oldColor = GUI.color;
        GUI.color = Color.black;

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
        GUILayout.Label("B: Pick up / drop item");
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUI.color = oldColor;

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
