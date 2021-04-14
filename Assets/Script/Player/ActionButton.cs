using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    private ActionBtnDel m_actionBtnDel;

    public void SetActionButton(ActionBtnDel actionBtnDel)
    {
        m_actionBtnDel = actionBtnDel;
    }
    public void OnPressBtn() { if (m_actionBtnDel != null) m_actionBtnDel(); }
}
