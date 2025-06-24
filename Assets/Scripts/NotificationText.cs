using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationText : MonoBehaviour
{
    public float displayTime = 3f;
    public float fadeDuration = 1f;

    public void Initialize()
    {
        //StartCoroutine(FadeAndDestroyCoroutine(displayTime, fadeDuration));
        Destroy(gameObject, 3f);
    }

    private IEnumerator FadeAndDestroyCoroutine(float waitTime, float fadeTime)
    {
        yield return new WaitForSeconds(waitTime);

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        Debug.Log("CZAS NA NISZCZONKO!!!");
        Destroy(gameObject);
    }
}
