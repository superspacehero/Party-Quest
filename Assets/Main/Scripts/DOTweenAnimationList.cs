using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

[AddComponentMenu("DOTween/DOTween Animation List")]
public class DOTweenAnimationList : MonoBehaviour
{
    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    public void Reset()
    {
        getChildAnimations = getChildAnimations;
        animationContainer = animationContainer;
    }

    private bool getChildAnimations
    {
        get => _getChildAnimations;
        set => _getChildAnimations = value;
    }
    [SerializeField]
    private bool _getChildAnimations = true;

    public GameObject animationContainer
    {
        get
        {
            if (!_animationContainer)
                _animationContainer = gameObject;

            return _animationContainer;
        }

        set
        {
            _animationContainer = value;

            if (_animationContainer != null)
                animations = (getChildAnimations) ? _animationContainer.GetComponentsInChildren<DOTweenAnimation>() : _animationContainer.GetComponents<DOTweenAnimation>();

            foreach (DOTweenAnimation animation in animations)
                if (!animation.targetIsSelf)
                    animation.tweenTargetIsTargetGO = false;
        }
    }
    [SerializeField]
    private GameObject _animationContainer;

    public DOTweenAnimation[] animations;

    public void Play()
    {
        foreach (DOTweenAnimation animation in animations)
            animation.DOPlay();
    }

    public void PlayAllWithID(string id)
    {
        foreach (DOTweenAnimation animation in animations)
            if (animation.id == id)
            {
                animation.DOPlay();
                Debug.Log("Playing " + animation.animationType + " with ID " + id, animation.gameObject);
            }
    }

    public void PlayForward()
    {
        foreach (DOTweenAnimation animation in animations)
            animation.DOPlayForward();
    }

    public void PlayBackwards()
    {
        foreach (DOTweenAnimation animation in animations)
        {
            animation.DOPlayBackwards();
        }
    }

    public void PlayBackwardsAllWithID(string id)
    {
        foreach (DOTweenAnimation animation in animations)
            if (animation.id == id)
            {
                animation.DOPlayBackwards();
                Debug.Log("Playing " + animation.animationType + " backwards with ID " + id, animation.gameObject);
            }
    }

    public void Rewind()
    {
        foreach (DOTweenAnimation animation in animations)
            animation.DORewind();
    }

    public void Restart()
    {
        foreach (DOTweenAnimation animation in animations)
            animation.DORestart();
    }

    public void RestartAllWithID(string id)
    {
        foreach (DOTweenAnimation animation in animations)
            if (animation.id == id)
            {
                animation.DORestart();
                Debug.Log("Restarting " + animation.animationType + " with ID " + id, animation.gameObject);
            }
    }

    public void Complete()
    {
        foreach (DOTweenAnimation animation in animations)
            animation.DOComplete();
    }

    public void ResetAnimation()
    {
        foreach (DOTweenAnimation animation in animations)
        {
            animation.DORewind();
            animation.DOPlay();
        }
    }
}
