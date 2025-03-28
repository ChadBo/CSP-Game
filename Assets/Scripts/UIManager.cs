using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Animator BlackScreen;

    public void loadMenu()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        BlackScreen.SetBool("FadeIn", true);
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(0);
    }
}
