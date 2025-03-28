using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterGreatDoor : MonoBehaviour
{
    public int sceneNum;
    public GameObject door; // Reference to the door GameObject

    private void Start()
    {
        Debug.Log(DoorStateManager.IsDoorOpen);
        // Set the door state when the scene starts
        if (DoorStateManager.IsDoorOpen && door != null)
        {
            // Open the door (this depends on your door mechanics, here assuming setting an open animation or disabling the door collider)
            OpenDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(enterUnderground());
        }
    }

    private IEnumerator enterUnderground()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerHealthManager>().blackScreenAnimator.SetBool("FadeIn", true);
        yield return new WaitForSeconds(3f);

        // Set the door state to open when entering the underground scene
        DoorStateManager.IsDoorOpen = true;

        // Load the underground scene
        SceneManager.LoadScene(sceneNum);

        if (sceneNum == 2)
        {
            GameObject.FindWithTag("Player").transform.position = new Vector3(0, 20, 0);
        }
    }

    // Open the door logic
    private void OpenDoor()
    {
        if (door != null)
        {
            door.transform.position = new Vector2(-5, 26.5f);
        }
    }
}
