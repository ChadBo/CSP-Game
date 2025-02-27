using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceGameObject : MonoBehaviour
{
    public GameObject target;
    public float RangeToTrack;
    public float addedDegrees = 0;
    public bool flipOnOverTurn;

    public SpriteRenderer srIfOverturn;

    void Update()
    {
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < RangeToTrack)
        {
            Vector2 dirToTarget = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + addedDegrees);

            // Flip sprite if it turns beyond ±90 degrees
            if (flipOnOverTurn && srIfOverturn != null)
            {
                srIfOverturn.flipY = Mathf.Abs(angle) > 90f;
                //Debug.Log(angle);
            }
        }
    }
}