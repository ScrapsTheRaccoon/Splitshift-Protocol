using System.Collections;
using UnityEngine;

public class TutorialObjects : MonoBehaviour
{
    [SerializeField] private GameObject qKeyInputPrompt;
    [SerializeField] private GameObject eKeyInputPrompt;
    [SerializeField] private GameObject tabInputPrompt;
    [SerializeField] private GameObject spaceInputPrompt;

    [SerializeField] private GameObject SecondTrigger;

    private new Collider2D collider;

    void Start()
    {
        qKeyInputPrompt.SetActive(false);
        eKeyInputPrompt.SetActive(false);
        tabInputPrompt.SetActive(false);
        spaceInputPrompt.SetActive(false);

        collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collider.enabled = false;
        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        // Space
        spaceInputPrompt.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        spaceInputPrompt.SetActive(false);

        // wait until 2nd trigger reached
        yield return new WaitUntil(() => TutorialSecondTrigger.reached);

        // wait until player tries to jump again
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        // Q
        qKeyInputPrompt.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Q));
        qKeyInputPrompt.SetActive(false);

        // Tab
        tabInputPrompt.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Tab));
        tabInputPrompt.SetActive(false);

        // E
        eKeyInputPrompt.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        eKeyInputPrompt.SetActive(false);
    }
}
