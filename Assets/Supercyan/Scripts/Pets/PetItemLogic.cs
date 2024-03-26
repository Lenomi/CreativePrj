using UnityEngine;

public class PetItemLogic : MonoBehaviour
{
    [SerializeField] private Transform m_itemHoldDummy = default;
    public Transform ItemHoldDummy { get => m_itemHoldDummy; }

    [SerializeField] private MeshCollider m_collider = default;
    [SerializeField] private Collider m_trigger = default;
    [SerializeField] private Rigidbody m_rigidBody = default;

    public void OnPickup()
    {
        m_collider.enabled = false;
        m_trigger.enabled = false;
        if (m_rigidBody != null) { m_rigidBody.isKinematic = true; }
    }

    public void OnDrop()
    {
        m_collider.enabled = true;
        m_trigger.enabled = true;
        if (m_rigidBody != null) { m_rigidBody.isKinematic = false; }
    }
}
