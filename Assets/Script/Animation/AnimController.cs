using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    private Animator m_animtor;
    private Dictionary<string, float> m_dicClipLength = new Dictionary<string, float>(); //원하는 애니메이션의 재생시간을 가지고온다. 
    public float GetClipLength(string clipName) { return m_dicClipLength[clipName]; }
    public AnimatorStateInfo StateInfo { get { return m_animtor.GetCurrentAnimatorStateInfo(0); } }
    public void Play(string clipName,bool isBlend=true)
    {
        if (isBlend) m_animtor.SetTrigger(clipName); //블랜딩 상태에서는 Trigger 형태로 애니메이션 재생
        else m_animtor.Play(clipName, 0, 0f); // 블랜딩이 아닌 상태에서는 Play 바로 형태로 애니메이션 재생
    }
    private void Awake() { m_animtor = GetComponent<Animator>(); }   
    private void Start()
    {
        var clips = m_animtor.runtimeAnimatorController;
        for (int i = 0; i < clips.animationClips.Length; i++)
            m_dicClipLength.Add(clips.animationClips[i].name, clips.animationClips[i].length); //객체의 모든 애니메이션의 재생을 Dictionary에 추가해준다.
    }
}
