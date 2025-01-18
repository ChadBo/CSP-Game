using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject playerObject;
    private Vector2 playerPos;
    public float lerpSpeed = 5f;

    void Start()
    {
        playerObject = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        playerPos = (Vector2)playerObject.transform.position;

        transform.position = Vector2.Lerp(transform.position, playerPos, lerpSpeed * Time.deltaTime);
    }
}
