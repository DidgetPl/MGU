using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class Opponent : MonoBehaviour
{
    Queue<Transform> targetsQueue = new Queue<Transform>();
    Transform currentTarget = null;
    [HideInInspector] public Dictionary<string, bool> targetSpotted = new Dictionary<string, bool>();

    bool isSwitching = false;
    NotificationManager nm;

    private void Start()
    {
        nm = FindObjectOfType<NotificationManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plane") && other.transform.position.y > 25f)
            if (!targetsQueue.Contains(other.transform) && other.transform != currentTarget)
            {
                targetsQueue.Enqueue(other.transform);

                if (currentTarget == null && !isSwitching)
                    StartCoroutine(SwitchTargetRoutine());
            }
    }

    private void Update()
    {
        if (currentTarget)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }

    private IEnumerator SwitchTargetRoutine()
    {
        isSwitching = true;
        while (targetsQueue.Count > 0)
        {
            currentTarget = targetsQueue.Dequeue();
            while((currentTarget.position-transform.position).magnitude > 200f && targetsQueue.Count > 0)
                currentTarget = targetsQueue.Dequeue();

            yield return new WaitForSeconds(2f);

            foreach(string planeName in GameManager.targets)
                if (currentTarget.name.Contains(planeName) && !targetSpotted[planeName])
                {
                    targetSpotted[planeName] = Random.Range(0f, 1f) > 0.6f;
                    if (targetSpotted[planeName])
                    {
                        int sum = targetSpotted.Values.Sum(v => v ? 1 : 0);
                        nm.ShowNotification($"Przeciwnik znalaz³ {sum} na 4 cele!");
                        if (sum == 4) FindObjectOfType<GameManager>().Endgame(false);
                    }

                }
        }

        currentTarget = null;
        isSwitching = false;
    }
}
