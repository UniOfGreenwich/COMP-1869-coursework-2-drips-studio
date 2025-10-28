using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Generic object pool that reuses inactive objects to avoid expensive
/// instantiation during gameplay.
/// </summary>
/// <typeparam name="T">Component type stored in the pool.</typeparam>
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Stack<T> pool = new();
    private readonly int maxSize;

    /// <summary>
    /// Creates a new object pool.
    /// </summary>
    /// <param name="prefab">Prefab used when the pool requires new instances.</param>
    /// <param name="initialSize">Number of objects preallocated for the pool.</param>
    /// <param name="parent">Optional parent transform for pooled objects.</param>
    /// <param name="maxSize">Maximum number of objects retained in the pool.</param>
    public ObjectPool(T prefab, int initialSize, Transform parent = null, int maxSize = -1)
    {   
        this.prefab = prefab;
        this.parent = parent;
        this.maxSize = maxSize < 0 ? int.MaxValue : maxSize;
        for (int i = 0; i < initialSize; i++)
        {
            if (prefab.GetType() == typeof(AudioSource)) // Adds group to output field
            {
                AudioMixer audioMixer = AudioManager.Instance.mixer;
                AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("SFX");
                
                if (groups.Length > 0)
                {
                    prefab.GetComponent<AudioSource>().outputAudioMixerGroup = groups[0];
                }
            }
            
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }

    /// <summary>
    /// Retrieves an object from the pool.
    /// </summary>
    /// <returns>An active instance of <typeparamref name="T"/>.</returns>
    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Pop();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return Object.Instantiate(prefab, parent);
    }

    /// <summary>
    /// Returns an object to the pool for reuse.
    /// </summary>
    /// <param name="obj">Instance previously obtained from <see cref="Get"/>.</param>
    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        if (pool.Count < maxSize)
        {
            pool.Push(obj);
        }
        else
        {
            Object.Destroy(obj.gameObject);
        }
    }
}
