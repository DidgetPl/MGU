using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static List<string> planeNames = new List<string>() { "A50", "MiG-29", "SU-25k", "Su-27", "Su-34", "su57", "tu95", "Tu160" };
    public static List<string> targets = new List<string>();

    void Awake()
    {
        Opponent op = FindObjectOfType<Opponent>().GetComponent<Opponent>();
        Player player = FindObjectOfType<Player>().GetComponent<Player>();
        while(targets.Count() < 4)
        {
            string newTarget = planeNames[Random.Range(0, planeNames.Count())];
            Debug.Log("HEJAHO " + newTarget);
            if (!targets.Contains(newTarget))
                targets.Add(newTarget);
        }
        foreach (string target in targets)
        {
            op.targetSpotted.Add(target, false);
            player.targetSpotted.Add(target, false);
        }
    }
}
