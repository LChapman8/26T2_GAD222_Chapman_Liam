using System.Collections;
using UnityEngine;

public class RandomAmbientSound : MonoBehaviour
{
    [Header("Sounds")]
    [Tooltip("Sounds that can randomly play.")]
    [SerializeField] private AudioClip[] audioClips;

    [Header("Timing")]
    [Tooltip("Minimum time between sounds.")]
    [SerializeField] private float minDelay = 3f;

    [Tooltip("Maximum time between sounds.")]
    [SerializeField] private float maxDelay = 8f;

    [Header("Random Spawn Area")]
    [Tooltip("Size of the area sounds can spawn within.")]
    [SerializeField] private Vector3 spawnArea = new Vector3(10f, 2f, 10f);

    [Header("3D Audio")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.7f;

    [Tooltip("Distance where the sound is at full volume.")]
    [SerializeField] private float minDistance = 1f;

    [Tooltip("Maximum distance the sound can be heard.")]
    [SerializeField] private float maxDistance = 15f;

    [Header("Settings")]
    [SerializeField] private bool playImmediately = false;

    private Coroutine soundRoutine;
    private int previousClipIndex = -1;

    private void OnEnable()
    {
        soundRoutine = StartCoroutine(RandomSoundRoutine());
    }

    private void OnDisable()
    {
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
            soundRoutine = null;
        }
    }

    private IEnumerator RandomSoundRoutine()
    {
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning(
                $"No ambient audio clips assigned to {gameObject.name}.");

            yield break;
        }

        if (!playImmediately)
        {
            yield return new WaitForSeconds(
                Random.Range(minDelay, maxDelay));
        }

        while (true)
        {
            PlayRandomSound();

            yield return new WaitForSeconds(
                Random.Range(minDelay, maxDelay));
        }
    }

    private void PlayRandomSound()
    {
        int clipIndex = GetRandomClipIndex();

        AudioClip selectedClip = audioClips[clipIndex];

        if (selectedClip == null)
            return;

        previousClipIndex = clipIndex;

        Vector3 randomPosition = GetRandomPosition();

        GameObject soundObject =
            new GameObject("Random Ambient Sound");

        soundObject.transform.position = randomPosition;

        AudioSource source =
            soundObject.AddComponent<AudioSource>();

        source.clip = selectedClip;
        source.volume = volume;

        // Make the sound fully 3D.
        source.spatialBlend = 1f;

        source.rolloffMode =
            AudioRolloffMode.Logarithmic;

        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.playOnAwake = false;
        source.loop = false;

        source.Play();

        // Remove temporary object after audio finishes.
        Destroy(
            soundObject,
            selectedClip.length + 0.5f);
    }

    private int GetRandomClipIndex()
    {
        if (audioClips.Length <= 1)
            return 0;

        int newIndex;

        do
        {
            newIndex =
                Random.Range(0, audioClips.Length);
        }
        while (newIndex == previousClipIndex);

        return newIndex;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 offset = new Vector3(
            Random.Range(
                -spawnArea.x / 2f,
                spawnArea.x / 2f),

            Random.Range(
                -spawnArea.y / 2f,
                spawnArea.y / 2f),

            Random.Range(
                -spawnArea.z / 2f,
                spawnArea.z / 2f)
        );

        return transform.position + offset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(
            transform.position,
            spawnArea);
    }
}