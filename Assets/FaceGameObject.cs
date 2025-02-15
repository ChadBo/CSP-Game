using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceGameObject : MonoBehaviour
{
    public GameObject target;
    public float RangeToTrack;

    void Update()
    {
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < RangeToTrack)
        {
            Vector2 dirToTarget = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}