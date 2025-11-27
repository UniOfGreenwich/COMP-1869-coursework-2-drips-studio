using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSquishAnimation : MonoBehaviour
{
    [Header("Squash and Stretch Core")]
    [SerializeField] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField] private bool canBeOverwritten;
    [SerializeField] private bool playOnStart;
    [SerializeField] private bool playsEveryTime = true;
    [SerializeField, Range(0, 100f)] private float chanceToPlay = 100f;

    public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    [Header("Animation Settings")]
    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maximumScale = 1.3f;
    [SerializeField] private bool resetToInitialScaleAfterAnimation = true;
    [SerializeField] private bool reverseAnimationCurveAfterPlaying;
    private bool _isReversed;

    [SerializeField] private AnimationCurve squashAndStretchCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
    );

    [Header("Looping Settings")]
    [SerializeField] private bool looping = false;
    [SerializeField] private float loopingDelay = 0.5f;

    [Header("Control")]
    public bool squish;

    private Coroutine _squashAndStretchCoroutine;
    private WaitForSeconds _loopingDelayWaitForSeconds;
    private Vector3 _initialScaleVector;

    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake()
    {
        if (transformToAffect == null)
            transformToAffect = transform;

        _initialScaleVector = transformToAffect.localScale;
        _loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
    }

    private void Start()
    {
        if (playOnStart || squish)
            CheckForAndStartCoroutine();
    }

    private void Update()
    {
        if (squish && _squashAndStretchCoroutine == null)
        {
            CheckForAndStartCoroutine();
            if (!looping) StartCoroutine(ResetSquish());
        }
        else if (!squish && _squashAndStretchCoroutine != null)
        {
            StopCoroutine(_squashAndStretchCoroutine);
            _squashAndStretchCoroutine = null;
            if (resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = _initialScaleVector;
        }
    }

    public void PlaySquashAndStretch()
    {
        if (looping && !canBeOverwritten)
            return;

        CheckForAndStartCoroutine();
    }

    private void CheckForAndStartCoroutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("Axis to affect is set to None.", gameObject);
            return;
        }

        if (_squashAndStretchCoroutine != null)
        {
            StopCoroutine(_squashAndStretchCoroutine);
            if (playsEveryTime && resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = _initialScaleVector;
        }

        _squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            if (!playsEveryTime)
            {
                float random = Random.Range(0, 100f);
                if (random > chanceToPlay)
                {
                    yield return null;
                    continue;
                }
            }

            if (reverseAnimationCurveAfterPlaying)
                _isReversed = !_isReversed;

            float elapsedTime = 0;
            Vector3 originalScale = _initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float curvePosition = _isReversed
                    ? 1 - (elapsedTime / animationDuration)
                    : elapsedTime / animationDuration;

                float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));
                float minimumThreshold = 0.0001f;

                if (Mathf.Abs(remappedValue) < minimumThreshold)
                    remappedValue = minimumThreshold;

                modifiedScale.x = affectX ? originalScale.x * remappedValue : originalScale.x / remappedValue;
                modifiedScale.y = affectY ? originalScale.y * remappedValue : originalScale.y / remappedValue;
                modifiedScale.z = affectZ ? originalScale.z * remappedValue : originalScale.z / remappedValue;

                transformToAffect.localScale = modifiedScale;

                yield return null;
            }

            if (resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = originalScale;

            if (looping)
                yield return _loopingDelayWaitForSeconds;

        } while (looping && squish);
    }

    public void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }

    public void Setup(SquashStretchAxis axis, float time, float zeroMap, float oneMap, AnimationCurve curve,
        bool loop, float delay, bool playImmediately = false)
    {
        axisToAffect = axis;
        animationDuration = time;
        initialScale = zeroMap;
        maximumScale = oneMap;
        squashAndStretchCurve = curve;
        looping = loop;
        loopingDelay = delay;

        _loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);

        if (playImmediately)
            CheckForAndStartCoroutine();
    }

    public IEnumerator ResetSquish()
    {
        yield return new WaitForSeconds(animationDuration);
        squish = false;
    }
}
