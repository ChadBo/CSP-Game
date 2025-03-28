using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpenStickerSheet : MonoBehaviour
{
    public Button menuButton;

    public Image Paper;

    // Sticker images to be assigned in the Inspector
    public Image Health;
    public Image Plugging;
    public Image GreatDoor;
    public Image Sprint;
    public Image Sword;
    public Image Rolling;
    public Image StarGuide;
    public Image EnemyGuide;
    public Image Banner;
    public Image Map;

    private Dictionary<string, Image> stickerImages = new Dictionary<string, Image>();
    private Dictionary<string, bool> stickerBools = new Dictionary<string, bool>();

    private float OpenMenuInput;
    public bool menuIsOpen;
    private bool hasBeenCalled = false;

    private TextMeshProUGUI TabTooltip;

    void Start()
    {
        // Initialize the dictionary with sticker names and corresponding images
        stickerImages.Add("Health", Health);
        stickerImages.Add("Plugging", Plugging);
        stickerImages.Add("GreatDoor", GreatDoor);
        stickerImages.Add("Sprint", Sprint);
        stickerImages.Add("Sword", Sword);
        stickerImages.Add("Rolling", Rolling);
        stickerImages.Add("StarGuide", StarGuide);
        stickerImages.Add("EnemyGuide", EnemyGuide);
        stickerImages.Add("Banner", Banner);
        stickerImages.Add("Map", Map);

        // Initialize all stickers as locked (false)
        foreach (var key in stickerImages.Keys)
        {
            stickerBools[key] = false;
        }

        TabTooltip = GameObject.Find("TabTooltip").GetComponent<TextMeshProUGUI>();

        menuButton.enabled = false;
        menuButton.gameObject.GetComponent<Image>().enabled = false;
        menuButton.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
    }

    void Update()
    {
        OpenMenuInput = InputManager.Menu;

        if (OpenMenuInput == 1 && !menuIsOpen && !hasBeenCalled)
        {
            OpenMenu();
        }
        else if (OpenMenuInput == 1 && menuIsOpen && !hasBeenCalled)
        {
            CloseMenu();
        }
    }

    private void OpenMenu()
    {
        Cursor.visible = true;

        Paper.enabled = true;
        hasBeenCalled = true;
        Invoke(nameof(SetMenuIsOpen), 0.3f);

        // Enable images for unlocked stickers
        foreach (var sticker in stickerBools)
        {
            if (sticker.Value && stickerImages.ContainsKey(sticker.Key))
            {
                stickerImages[sticker.Key].enabled = true;
            }
        }
        menuButton.enabled = true;
        menuButton.gameObject.GetComponent<Image>().enabled = true;
        menuButton.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;
    }

    private void CloseMenu()
    {
        Cursor.visible = false;

        Paper.enabled = false;
        hasBeenCalled = true;
        Invoke(nameof(SetMenuIsOpen), 0.3f);

        // Disable all sticker images
        foreach (var sticker in stickerImages.Values)
        {
            sticker.enabled = false;
        }
        menuButton.enabled = true;
        menuButton.gameObject.GetComponent<Image>().enabled = true;
        menuButton.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;
    }

    private void SetMenuIsOpen()
    {
        menuIsOpen = !menuIsOpen;
        hasBeenCalled = false;
    }

    // Function to unlock a sticker dynamically
    public void SetBool(string key, bool value)
    {
        if (stickerBools.ContainsKey(key))
        {
            stickerBools[key] = value;
        }
        else
        {
            Debug.LogWarning($"Sticker '{key}' does not exist in dictionary.");
        }
    }

    public void EnableTabTooltip()
    {
        TabTooltip.enabled = true;
        Invoke("disableTooltip", 2f);
    }

    private void disableTooltip()
    {
        TabTooltip.enabled = false;
    }
}