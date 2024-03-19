using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DefinedEnums;

public class PlayerAnimationController : AnimationController
{

    StringBuilder m_sb = new StringBuilder();
    PlayerMotion m_state;

    public PlayerMotion GetAnimState()
    {
        return m_state;
    }// 모션 값을 얻음

    public void Play(PlayerMotion motion, bool isBlend = true)
    {
        m_sb.Append(motion);
        m_state = motion;
        Play(m_sb.ToString(), isBlend);
        m_sb.Clear();
    } //2. Motion을 불러오기위함

}
