using System;
using System.Collections.Generic;
using Framework;
using Framework.Runtime;
using Framework.ViewModule;
using UnityEngine;

public class AndroidBackManager
{
    private List<BaseViewModule> m_list = new List<BaseViewModule>(); 
    
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (m_list.Count > 0)
            {
                var viewName = m_list[m_list.Count - 1].m_viewName;
        
                var view = GameApp.View.GetViewModule<BaseViewModule>(viewName);
                if (view != null)
                {
                    if (view.m_isAddBack)
                    {
                        //返回键可用
                        //这里执不执行RemoveAt都可以，因为下面的CloseView中也会删一次
                        RemoveBack(view);
                        GameApp.View.CloseView(viewName);    
                    }
                    else
                    {
                        //返回键不可用，那么等待用户自己手点关闭按钮来移除这个UI
                    }
                }
                else
                {
                    RemoveBack(view);
                }
            }
        }
    }

    public void Clear()
    {
        m_list.Clear();
    }

    public void AddBack(BaseViewModule view)
    {
        if (m_list.Contains(view) == false)
        {
            m_list.Add(view);
        }
        else
        {
            Logger.LogError($"AndroidBackManager.AddBack({view.m_viewName}) error");
        }
    }

    public void RemoveBack(BaseViewModule view)
    {
        if (m_list.Contains(view))
        {
            m_list.Remove(view);
        }
    }
}
