using System.Collections;
using UnityEngine;

public class PetItemHoldLogic : MonoBehaviour
{
    [SerializeField] private Animator m_animator = default;
    [SerializeField] private Transform m_itemHoldDummy = default;
    [SerializeField] private KeyCode m_pickUpKey = KeyCode.B;

    private PetItemLogic m_heldItem = null;

    private PetItemLogic m_itemToPickup = null;

    private readonly string m_pickupTrigger = "PickUp";
    private readonly string m_dropTrigger = "Drop";

    private void Update()
    {
        if (Input.GetKeyDown(m_pickUpKey))
        {
            if (m_itemToPickup != null)
            {
                PickUpItem(m_itemToPickup);
            }
            else if (m_heldItem != null)
            {
                DropItem();
            }
        }
    }

    private void PickUpItem(PetItemLogic item)
    {
        if (m_heldItem != null) { DropItem(); }

        StartCoroutine(PickUpCoroutine(item));
    }

    private WaitForSeconds m_pickUpDelay = new WaitForSeconds(0.3f);
    private IEnumerator PickUpCoroutine(PetItemLogic item)
    {
        if (m_animator != null)
        {
            m_animator.SetTrigger(m_pickupTrigger);
            yield return m_pickUpDelay;
        }
        else
        {
            yield return null;
        }

        item.transform.parent = m_itemHoldDummy;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        if (item.ItemHoldDummy != null)
        {
            item.transform.localRotation = item.ItemHoldDummy.localRotation;

            Vector3 dummyDelta = item.ItemHoldDummy.position - m_itemHoldDummy.position;
            item.transform.position -= dummyDelta;
        }

        m_heldItem = item;
        m_heldItem.OnPickup();

        m_itemToPickup = null;
    }

    private void DropItem()
    {
        if (m_heldItem == null) { return; }

        StartCoroutine(DropCoroutine());
    }

    private WaitForSeconds m_dropDelay = new WaitForSeconds(0.2f);
    private IEnumerator DropCoroutine()
    {
        if (m_animator != null)
        {
            m_animator.SetTrigger(m_dropTrigger);
            yield return m_dropDelay;
        }
        else
        {
            yield return null;
        }

        m_heldItem.transform.parent = null;
        m_heldItem.OnDrop();

        m_heldItem = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        PetItemLogic item = other.GetComponent<PetItemLogic>();
        if (item != null)
        {
            m_itemToPickup = item;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PetItemLogic item = other.GetComponent<PetItemLogic>();
        if (item != null && item == m_itemToPickup)
        {
            m_itemToPickup = null;
        }
    }
}
