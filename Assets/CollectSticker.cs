using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSticker : MonoBehaviour
{
    public string nameOfImageBoolean;
    public OpenStickerSheet StickerSheetScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Ensures only the player triggers it
        {
            StickerSheetScript.SetBool(nameOfImageBoolean, true);
            Destroy(gameObject); // Removes the sticker after collecting it
        }
    }
}