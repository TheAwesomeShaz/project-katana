using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    [SerializeField] bool isLandingPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent<PlayerController>(out PlayerController player))
        {
            //player.SetJump(isLandingPoint);

        }
    }
}
