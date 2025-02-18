using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeController : MonoBehaviour
{
    public static ScreenShakeController instance;

    private float shakeTimeRemaining, shakePower, shakeFadeTime, shakeRotation;
    public float rotationMultiplier;

    private void Start()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        if(shakeTimeRemaining > 0)
        {
            Debug.Log("SHAKIN");
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * (shakePower/100);
            float yAmount = Random.Range(-1f, 1f) * (shakePower/100);

            transform.position += new Vector3(xAmount, yAmount, 0);
            shakePower = Mathf.MoveTowards(shakePower, 0, shakeFadeTime*Time.deltaTime);
            shakeRotation = Mathf.MoveTowards(shakeRotation, 0, shakeFadeTime * rotationMultiplier * Time.deltaTime);
        }

        transform.rotation = Quaternion.Euler(0f, 0f, shakeRotation * Random.Range(-1f, 1f));
    }

    public void StartShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;
        shakeFadeTime = power / length;
        shakeRotation = power * rotationMultiplier;
    }
}
