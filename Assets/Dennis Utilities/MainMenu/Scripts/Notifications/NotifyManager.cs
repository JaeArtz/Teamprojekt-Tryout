using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class NotifyManager : MonoBehaviour
{
    public static NotifyManager ManagerInstance;

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationParent;
    [SerializeField] private TMP_Text notificationTextUI;

    private CanvasGroup notificationUICanvasGroup;
    private Queue<Notify> notificationQueue = new Queue<Notify>();
    private bool isDisplayingNotification = false;

    private void Awake()
    {
        if(ManagerInstance == null)
        {
            ManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        notificationUICanvasGroup = notificationParent.GetComponent<CanvasGroup>();
        notificationUICanvasGroup.alpha = 0;
    }

    public void ShowNotification(Notify notificationData)
    {
        notificationQueue.Enqueue(notificationData);
        if(!isDisplayingNotification)
        {
            StartCoroutine(DisplayNotification());
        }
    }

    private IEnumerator DisplayNotification()
    {
        isDisplayingNotification = true;
        while (notificationQueue.Count > 0)
        {
            Notify data = notificationQueue.Dequeue();

            notificationTextUI.text = data.Message;

            yield return StartCoroutine(FadeCanvasGroup(notificationUICanvasGroup, true, data.FadeDuration));

            yield return new WaitForSeconds(data.DisplayDuration);

            yield return StartCoroutine(FadeCanvasGroup(notificationUICanvasGroup, false, data.FadeDuration));
        }
        isDisplayingNotification = false;
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn, float duration)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        float initialAlpha = canvasGroup.alpha;
        float elapsedTime = 0.0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
