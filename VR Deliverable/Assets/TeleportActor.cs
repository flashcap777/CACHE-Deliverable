using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Teleports an Actor on Trigger Enter
 */
public class TeleportActor : MonoBehaviour
{
    [SerializeField] Vector3 TargetPos;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrackObjectInRange>())
            other.transform.position = TargetPos;
        Debug.Log(other);
    }
}
