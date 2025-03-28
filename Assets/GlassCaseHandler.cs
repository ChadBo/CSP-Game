using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlassCaseHandler : MonoBehaviour
{
    public Sprite[] GlassSprites;
    public int currentSprite;
    private SpriteRenderer sr;
    public SpriteRenderer shadowSR;
    public BoxCollider2D glassColl;
    public SphereCollider postProcessingVol;

    public Light2D starLight;
    public SpriteRenderer starSR;
    public ParticleSystem idlePS;
    public ParticleSystem explodePS;
    public Transform StarDoor;

    public bool hasStarted = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        currentSprite = GlassSprites.Length - 1;
    }

    void Update()
    {
        if(currentSprite == 0 && !hasStarted)
        {
            hasStarted = true;
            glassColl.enabled = false;
            shadowSR.enabled = false;
            idlePS.loop = false;
            ScreenShakeController.instance.StartShake(5, 3);
            StarDoor.position = new Vector2(-5, 26.5f);
        }
        if(currentSprite == 0)
        {
            explodeStar();
        }
    }

    public void hitGlass()
    {
        currentSprite--;
        sr.sprite = GlassSprites[currentSprite];
    }


    private void explodeStar()
    {
        if(starLight.intensity < 25 && starSR.enabled)
        {
            starLight.intensity += Time.deltaTime * 5;
        }
        else if (!starSR.enabled)
        {
            starLight.intensity -= Time.deltaTime * 15;
        }
        if (starLight.intensity >= 25)
        {
            starSR.enabled = false;
            explodePS.Play();
        }
        if(starLight.intensity <= 0)
        {
            starLight.enabled = false;
            postProcessingVol.radius = 0f;
            DoorStateManager.IsDoorOpen = true;
        }
    }
}
