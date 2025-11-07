using Framework.EventSystem;
using Framework.Runtime;
using Framework.ViewModule;
using HotFixBattle;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotFix
{
    public class UIBattleViewModule : BaseViewModule, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI text_Wave;
        [SerializeField] private RectTransform rockerBackground; // 摇杆背景
        [SerializeField] private RectTransform rockerHandle;     // 摇杆手柄

        [Header("扩展点击区域设置")]
        [SerializeField] private float expansionFactor = 1f; // 扩展因子

        [Header("摇杆参数")]
        [SerializeField] private float handleRange = 2;      // 手柄最大偏移

        public Vector2 Direction { get; private set; }
        private Rect expandedRockerArea;
        private bool isDragging;

        public override void RegisterEvents(EventSystemManager manager)
        {
            manager.RegisterEvent((int)LocalMessageName.CC_BattleWaveChange, BattleWaveChange);
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
            manager.UnRegisterEvent((int)LocalMessageName.CC_BattleWaveChange, BattleWaveChange);
        }

        public override void OnCreate(object data)
        {
            
        }

        public override void OnDelete()
        {
            
        }

        private void BattleWaveChange(int type, BaseEventArgs eventargs)
        {
            if (eventargs is BattleWaveChangeEventArgs args)
                text_Wave.text = args.WaveId.ToString();
        }

        private void OnBattleDirectionChange(int type, BaseEventArgs eventargs)
        {
            if (eventargs is DirectionChangedEventArgs args)
            {
                // Handle direction change
            }
        }

        public override void OnOpen(object data)
        {
            // 只在打开时计算固定扩展区域
            Vector2 anchoredPosition = rockerBackground.anchoredPosition;
            Vector2 sizeDelta = rockerBackground.sizeDelta;

            expandedRockerArea = new Rect(
                anchoredPosition.x - (sizeDelta.x * expansionFactor * 0.5f),
                anchoredPosition.y - (sizeDelta.y * expansionFactor * 0.5f),
                sizeDelta.x * expansionFactor,
                sizeDelta.y * expansionFactor
            );
        }

        public override void OnClose()
        {
            
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (Application.isMobilePlatform)
                HandleTouchInput();
            else
                HandleMouseInput();
        }

        private bool IsInExpandedArea(Vector2 screenPoint)
        {
            Vector2 localPos;

            RectTransform parentRect = rockerBackground.parent as RectTransform; // UIBattle
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                screenPoint,
                GameApp.View.UICamera, // Overlay 模式传 null；Camera 模式传 UI Camera
                out localPos
            );
            return expandedRockerArea.Contains(localPos);
        }

        private void DragRocker(Vector2 screenPosition)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rockerBackground,
                screenPosition,
                GameApp.View.UICamera,
                out localPos
            );
            localPos = Vector2.ClampMagnitude(localPos, handleRange);
            rockerHandle.anchoredPosition = localPos;
            Direction = localPos / handleRange;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsInExpandedArea(eventData.position))
            {
                DragRocker(eventData.position);
                isDragging = true;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
                DragRocker(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isDragging && Direction != Vector2.zero)
            {
                var args = new DirectionChangedEventArgs { Direction = Direction };
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_RockerMove, args);
            }
            isDragging = false;
            rockerHandle.anchoredPosition = Vector2.zero;
            Direction = Vector2.zero;
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (IsInExpandedArea(touch.position))
                        DragRocker(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    OnPointerUp(null);
                }
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0) && IsInExpandedArea(Input.mousePosition))
                DragRocker(Input.mousePosition);

            if (Input.GetMouseButton(0) && IsInExpandedArea(Input.mousePosition))
                DragRocker(Input.mousePosition);

            if (Input.GetMouseButtonUp(0))
                OnPointerUp(null);
        }
    }
}
