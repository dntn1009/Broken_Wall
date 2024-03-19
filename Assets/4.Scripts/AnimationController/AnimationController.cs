using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator m_animator;
    string m_prevMotion;

    public void Play(string animName, bool isBlend = true)
    {
        if (!string.IsNullOrEmpty(m_prevMotion))
        {
            m_animator.ResetTrigger(m_prevMotion);
            m_prevMotion = null;
        }
        if (isBlend)
        {
            m_animator.SetTrigger(animName);
        }
        else
        {
            m_animator.Play(animName, 0, 0f);
        }
        m_prevMotion = animName;
    }

    // Update is called once per frame
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
}
