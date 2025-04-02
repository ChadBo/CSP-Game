using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsManager : MonoBehaviour
{
    public Animator blackAnimator;
    public AudioSource songPlayer;

    private void Update()
    {
        if(blackAnimator.GetBool("FadeIn"))
        {
            songPlayer.volume -= Time.deltaTime*0.5f;
        }
    }

    private IEnumerator loadSceneNum()
    {
        blackAnimator.SetBool("FadeIn", true);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(1);
    }

    public void loadScene()
    {
        StartCoroutine(loadSceneNum());
    }

    public void quitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
