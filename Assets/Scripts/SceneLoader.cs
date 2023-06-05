using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneAndFade(index));
    }

    private IEnumerator LoadSceneAndFade(int index)
    {
        transition.Play("Crossfade_Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(index);
        transition.Play("Crossfade_End");
    }
}
