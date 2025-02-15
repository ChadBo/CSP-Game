using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordKeyHandler : MonoBehaviour
{
    public SpriteRenderer topLayerPillarSR;
    public Y_Sorting ySortConnectedToTopObject;
    public PlayerAttacking PA;

    public void ShowTopPillarOnTopOfSword(bool set)
    {   
        if (set)
        {
            ySortConnectedToTopObject.canSort = false;
            topLayerPillarSR.sortingOrder = PA.weaponSR.sortingOrder + 2;
        }
        else
        {
            ySortConnectedToTopObject.canSort = true;
        }
    }
}
