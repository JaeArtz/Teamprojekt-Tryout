using System.Collections;
using System.IO;
using UnityEngine;

// idea: randomness in everything:
// ->in pause inbetween gusts
// ->in speed/duration of "fade in", "hold" and
//   "fade out" of played audio clip (=> range of float values)

public class WindGustRandom : MonoBehaviour
{
    public WindController windController;

    [Header("Interval (seconds)")]
    public float minInterval = 30f;
    public float maxInterval = 70f;

    [Header("Gust Duration (seconds)")]
    public float minDuration = 4f;
    public float maxDuration = 8f;

    [Header("Gust Influence")]
    public float minInfluence = 0.6f;
    public float maxInfluence = 1.5f;

    [Header("Gust Speed")]
    public float minSpeed = 6f;
    public float maxSpeed = 10f;

    Coroutine loop;

    void OnEnable() => loop = StartCoroutine(RandomLoop());

    void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
    }

    IEnumerator RandomLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            if (!windController) continue;

            float duration = Random.Range(minDuration, maxDuration);
            float influence = Random.Range(minInfluence, maxInfluence);
            float speed = Random.Range(minSpeed, maxSpeed);

            windController.TriggerGust(duration, influence, speed, true);
        }
    }
}
