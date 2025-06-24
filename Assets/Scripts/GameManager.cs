using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static List<string> planeNames = new List<string>() { "A50", "MiG-29", "Su-25", "Su-27", "Su-34", "Su-57", "Tu-95", "Tu-160" };
    public static List<string> targets = new List<string>();
    public GameObject checklist;
    bool alreadyInEndgame = false;

    void Awake()
    {
        Opponent op = FindObjectOfType<Opponent>().GetComponent<Opponent>();
        Player player = FindObjectOfType<Player>().GetComponent<Player>();
        while(targets.Count() < 4)
        {
            string newTarget = planeNames[Random.Range(0, planeNames.Count())];
            if (!targets.Contains(newTarget))
                targets.Add(newTarget);
        }
        foreach (string target in targets)
        {
            op.targetSpotted.Add(target, false);
            player.targetSpotted.Add(target, false);
        }

        for (int i = 0; i < 4; i++)
            checklist.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = targets[i];
    }

    public void Endgame(bool playerWon)
    {
        if (!alreadyInEndgame)
        {
            alreadyInEndgame = true;
            FindObjectOfType<NotificationManager>().ShowNotification(playerWon ? "Gratulacje, Wygra³eœ!" : "Niestety przegra³eœ.");
            StartCoroutine(QuitWithDelayCoroutine());
        }

    }

    private IEnumerator QuitWithDelayCoroutine()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
