using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ActionBtnDel();

public enum ActionButtonType
{
    None=-1,
    LeftAttack,RightAttack,Skill,
    Max
}
public class ActionManager : SingtonMonoBehaviour<ActionManager>
{
    [SerializeField] private GameObject m_actionBtnObejct;
    private ActionButton[] m_actionButton;
    void Start()
    {
        m_actionButton = m_actionBtnObejct.GetComponentsInChildren<ActionButton>();
    }
    public void SetButton(ActionButtonType type, ActionBtnDel actionBtnDel)
    {
        m_actionButton[(int)type].SetActionButton(actionBtnDel);
    }
}
