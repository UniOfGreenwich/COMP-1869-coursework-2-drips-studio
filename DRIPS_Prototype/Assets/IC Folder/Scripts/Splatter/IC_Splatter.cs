using System.Collections;
using UnityEngine;

public class IC_Splatter : MonoBehaviour
{
    public SoundEffectTrigger trigger;
    [SerializeField] private float shrinkDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            trigger?.Play();
            StartCoroutine(ShrinkAndDie());
        }
    }

    private IEnumerator ShrinkAndDie()
    {
        Vector3 originalScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // make sure it's exactly 0 at the end
        transform.localScale = Vector3.zero;

        Destroy(gameObject);
    }
}
