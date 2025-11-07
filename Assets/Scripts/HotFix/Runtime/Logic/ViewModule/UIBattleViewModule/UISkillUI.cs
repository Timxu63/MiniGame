
using System;
using Framework.Runtime;
using HotFixBattle;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public RectTransform button;
    public RectTransform dragIndicator;
    public RectTransform rangePreview;
    public float maxDragDistance = 150f;

    private Vector2 startPos;      // UI按钮初始位置
    private Vector2 direction;     // 当前方向
    private int fingerId = -1;     // 控制此技能的手指
    private bool isDragging = false;
    private int _index;

    public void Init(int index)
    {
        if (button != null)
            startPos = button.anchoredPosition;

        if (dragIndicator != null)
            dragIndicator.gameObject.SetActive(false);

        if (rangePreview != null)
            rangePreview.gameObject.SetActive(false);

        fingerId = -1;
        isDragging = false;
        direction = Vector2.zero;
        _index = index;
    }

    public bool IsFingerActive(int testFingerId)
    {
        return fingerId == testFingerId;
    }

    public bool IsFree()
    {
        return fingerId == -1;
    }

    public bool IsDragging()
    {
        return isDragging;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    /// <summary>
    /// 手指开始按下技能按钮
    /// </summary>
    public void OnBeginFinger(int newFingerId, Vector2 screenPos, Camera uiCam)
    {
        fingerId = newFingerId;
        isDragging = false;
        direction = Vector2.zero;
    }

    /// <summary>
    /// 手指拖动过程
    /// </summary>
    public void OnMoveFinger(Vector2 screenPos, Camera uiCam)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            button.parent as RectTransform,
            screenPos,
            uiCam,
            out localPos
        );

        Vector2 dragVector = localPos - startPos;
        float distance = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);

        if (distance > 10f) // 拖动阈值
        {
            isDragging = true;
            direction = dragVector.normalized;

            if (dragIndicator != null)
            {
                dragIndicator.gameObject.SetActive(true);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                dragIndicator.rotation = Quaternion.Euler(0, 0, angle);
            }
            if (rangePreview != null)
            {
                rangePreview.gameObject.SetActive(true);
                rangePreview.anchoredPosition = startPos + direction * distance;
            }
        }
    }

    /// <summary>
    /// 手指结束（释放）
    /// </summary>
    public void OnEndFinger(Vector2 screenPos)
    {
        PlayerSkillArgs args = new PlayerSkillArgs();
        args.skillId = _index;
        if (isDragging)
        {
            args.direction = direction;
            args.haveDir = true;
            Debug.Log($"[SkillUI] 技能释放: {button.name}, 方向: {direction}");
        }
        else
        {
            args.haveDir = true;
            Debug.Log($"[SkillUI] 技能点击: {button.name}");
        }
        
        // TODO: 调用技能释放逻辑
        GameApp.Event.DispatchNow((int)LocalMessageName.CC_PlayerMove, args);
        ResetState();
    }

    private void ResetState()
    {
        fingerId = -1;
        isDragging = false;
        direction = Vector2.zero;

        if (dragIndicator != null)
            dragIndicator.gameObject.SetActive(false);

        if (rangePreview != null)
        {
            rangePreview.gameObject.SetActive(false);
            rangePreview.anchoredPosition = startPos;
        }
    }
}