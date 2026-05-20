using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScene : MonoBehaviour
{
    [SerializeField] private float creditsLength;

    private void Start()
    {
        StartCoroutine(CreditsRoutine());
    }
    private IEnumerator CreditsRoutine()
    {
        yield return new WaitForSeconds(creditsLength);
        LevelManager.Instance.LoadScene("MainMenu", "CrossFade");
    }
}
