using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPrefab;
    public Transform notificationPanel;

    public void ShowNotification(string message)
    {
        GameObject obj = Instantiate(notificationPrefab, notificationPanel);
        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;
        obj.GetComponent<NotificationText>().Initialize();
    }
}
