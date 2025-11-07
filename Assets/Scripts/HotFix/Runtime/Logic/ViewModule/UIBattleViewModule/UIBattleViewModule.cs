using Framework.EventSystem;
using Framework.Runtime;
using Framework.ViewModule;
using HotFix;
using HotFixBattle;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using LocalMessageName = HotFixBattle.LocalMessageName;

public class UIBattleViewModule : BaseViewModule
{
    [SerializeField] private TextMeshProUGUI text_Wave;
    [Header("摇杆UI")]
    public RectTransform rockerBackground;
    public RectTransform rockerHandle;
    public float rockerHandleRange = 64f;
    public float rockerExpansionFactor = 1.5f;

    [Header("技能UI集合")]
    public SkillUI[] skills;

    private Camera uiCamera;
    public Vector2 MoveDirection { get; private set; }

    private Rect rockerExpandedArea;
    private int rockerFingerId = -1;
    private bool rockerDragging = false;


    public override void OnOpen(object data)
    {
        uiCamera = GameApp.View.UICamera;
        // 初始化摇杆区域
        Vector2 anchoredPosition = rockerBackground.anchoredPosition;
        Vector2 sizeDelta = rockerBackground.sizeDelta;
        rockerExpandedArea = new Rect(
            anchoredPosition.x - (sizeDelta.x * rockerExpansionFactor * 0.5f),
            anchoredPosition.y - (sizeDelta.y * rockerExpansionFactor * 0.5f),
            sizeDelta.x * rockerExpansionFactor,
            sizeDelta.y * rockerExpansionFactor
        );

        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].Init(i);
        }
    }


    public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
    {
#if UNITY_EDITOR
        HandleMouseSimulation();
#else
        HandleTouchInput();
#endif
    }

    public override void RegisterEvents(EventSystemManager manager)
    {
        manager.RegisterEvent((int)LocalMessageName.CC_BattleWaveChange, BattleWaveChange);
    }

    public override void UnRegisterEvents(EventSystemManager manager)
    {
        manager.UnRegisterEvent((int)LocalMessageName.CC_BattleWaveChange, BattleWaveChange);
    }
    private void BattleWaveChange(int type, BaseEventArgs eventargs)
    {
        if (eventargs is BattleWaveChangeEventArgs args)
            text_Wave.text = args.WaveId.ToString();
    }
    public override void OnCreate(object data)
    {
        
    }
    // 判断触点是否在摇杆扩展区域内
    private bool IsInRockerExpandedArea(Vector2 screenPoint)
    {
        Vector2 localPos;
        RectTransform parentRect = rockerBackground.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPoint,
            uiCamera,
            out localPos
        );
        return rockerExpandedArea.Contains(localPos);
    }

    // ============ 触摸处理 ============
    private void HandleTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // --- 摇杆逻辑 ---
            if (touch.phase == TouchPhase.Began && rockerFingerId == -1 && IsInRockerExpandedArea(touch.position))
            {
                rockerFingerId = touch.fingerId;
                rockerDragging = true;
                DragRocker(touch.position);
                continue;
            }
            if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && touch.fingerId == rockerFingerId && rockerDragging)
            {
                DragRocker(touch.position);
                continue;
            }
            if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && touch.fingerId == rockerFingerId)
            {
                ResetRocker();
                continue;
            }

            // --- 多技能逻辑 ---
            foreach (var skill in skills)
            {
                if (touch.phase == TouchPhase.Began && skill.IsFree() && RectTransformUtility.RectangleContainsScreenPoint(skill.button, touch.position, uiCamera))
                {
                    skill.OnBeginFinger(touch.fingerId, touch.position, uiCamera);
                }
                else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && skill.IsFingerActive(touch.fingerId))
                {
                    skill.OnMoveFinger(touch.position, uiCamera);
                }
                else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && skill.IsFingerActive(touch.fingerId))
                {
                    skill.OnEndFinger(touch.position);
                }
            }
        }
    }

    // ============ 鼠标模拟（PC） ============
    private void HandleMouseSimulation()
    {
        // 左键模拟摇杆
        if (Input.GetMouseButtonDown(0) && IsInRockerExpandedArea(Input.mousePosition))
        {
            rockerDragging = true;
            DragRocker(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && rockerDragging)
        {
            DragRocker(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && rockerDragging)
        {
            ResetRocker();
        }

        // =============================
        // 技能按钮检测（左键）
        // =============================
        if (Input.GetMouseButtonDown(0))
        {
            // 循环检测所有技能按钮
            for (int i = 0; i < skills.Length; i++)
            {
                var skill = skills[i];

                if (skill.IsFree() &&
                    RectTransformUtility.RectangleContainsScreenPoint(skill.button, Input.mousePosition, uiCamera))
                {
                    // 用负数伪 fingerId 避免和触屏冲突
                    skill.OnBeginFinger(-1000 - i, Input.mousePosition, uiCamera);
                    break; // 点中一个技能就停止检测
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < skills.Length; i++)
            {
                var skill = skills[i];
                if (skill.IsFingerActive(-1000 - i))
                {
                    skill.OnMoveFinger(Input.mousePosition, uiCamera);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            for (int i = 0; i < skills.Length; i++)
            {
                var skill = skills[i];
                if (skill.IsFingerActive(-1000 - i))
                {
                    skill.OnEndFinger(Input.mousePosition);
                    break;
                }
            }
        }
    }

    // 拖动摇杆
    private void DragRocker(Vector2 screenPosition)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rockerBackground,
            screenPosition,
            uiCamera,
            out localPos
        );
        localPos = Vector2.ClampMagnitude(localPos, rockerHandleRange);
        rockerHandle.anchoredPosition = localPos;
        MoveDirection = localPos / rockerHandleRange;
        
        var args = new DirectionChangedEventArgs { Direction = MoveDirection };
        GameApp.Event.DispatchNow((int)LocalMessageName.CC_PlayerMove, args);
    }

    // 重置摇杆
    private void ResetRocker()
    {
        rockerDragging = false;
        rockerFingerId = -1;
        rockerHandle.anchoredPosition = Vector2.zero;
        MoveDirection = Vector2.zero;
    }


    public override void OnClose()
    {
        
    }
    public override void OnDelete()
    {
        
    }
}