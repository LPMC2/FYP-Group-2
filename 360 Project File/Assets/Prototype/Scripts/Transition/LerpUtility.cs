using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public static class LerpUtility
{
    public static void SmoothLerp(MonoBehaviour monoBehaviour, float initialValue, float destinationValue, float duration)
    {
        if (monoBehaviour is Image image)
        {
            image.fillAmount = initialValue;
            image.gameObject.SetActive(true);

            monoBehaviour.StartCoroutine(LerpFillAmount(image, destinationValue, duration));
        }
        else if (monoBehaviour is Slider slider)
        {
            slider.value = initialValue;
            slider.gameObject.SetActive(true);

            monoBehaviour.StartCoroutine(LerpValue(slider, destinationValue, duration));
        }
    }

    private static System.Collections.IEnumerator LerpFillAmount(Image image, float destinationValue, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        float startValue = image.fillAmount;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            image.fillAmount = Mathf.Lerp(startValue, destinationValue, t);
            yield return null;
        }

        image.fillAmount = destinationValue;
    }
    public static System.Collections.IEnumerator LerpTargetValue(float value, float initialValue = 0, float destinationValue = 1, float duration = 1)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        float startValue = initialValue;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            value = Mathf.Lerp(startValue, destinationValue, t);
            yield return null;
        }

        value = destinationValue;
    }
    private static System.Collections.IEnumerator LerpValue(Slider slider, float destinationValue, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        float startValue = slider.value;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            slider.value = Mathf.Lerp(startValue, destinationValue, t);
            yield return null;
        }

        slider.value = destinationValue;
    }
}