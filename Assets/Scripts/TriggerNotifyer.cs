using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNotifyer : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    public bool isInsideCollider = false;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(LayerMask.LayerToName(collider.gameObject.layer));

        isInsideCollider = collider.IsTouchingLayers(layerMask);
        if (isInsideCollider)
        {
            Debug.Log("isInsideCollider");
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.IsTouchingLayers(layerMask))
        {
            isInsideCollider = false;
        }
    }


}
