using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public AnimationCurve curve;
    public bool start = false;

    private IEnumerator Shaking(float duration, float intensity)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            startPosition = transform.position;
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + (Random.insideUnitSphere * strength * intensity);
            yield return null;
        }

        transform.position = startPosition;
    }

    public void ShakeCamera(float duration, float intensity)
    {
        StartCoroutine(Shaking(duration, intensity));
    }
}
