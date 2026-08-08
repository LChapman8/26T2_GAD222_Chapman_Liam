using UnityEngine;

public class MemoryPathTracker : MonoBehaviour
{
    public enum MemoryPath
    {
        None,
        Career,
        Relationship,
        Family
    }

    public static MemoryPathTracker Instance { get; private set; }

    public MemoryPath SelectedPath { get; private set; } = MemoryPath.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetPath(MemoryPath path)
    {
        SelectedPath = path;

        Debug.Log("Memory path selected: " + path);
    }
}