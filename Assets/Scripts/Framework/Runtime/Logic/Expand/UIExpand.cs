using UnityEngine;

namespace Framework.Runtime
{
    public static class UIExpand
    {
        public static void SetForPadding(this RectTransform t)
        {
            t.sizeDelta = Vector3.zero;
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.anchorMin = Vector2.zero;
            t.anchorMax = Vector2.one;
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            var parentRect = t.parent.GetComponent<RectTransform>().rect;
            var top = GetTopHeight()/Screen.height*parentRect.height;
            var bottom = GetBottomHeight()/Screen.height*parentRect.height;
            var weight = 1080f;
            var parentWeight = parentRect.width;
            var delta = (parentWeight - weight) / 2;
            //offsetMin.x : Left
            //offsetMin.y : Bottom
            //-offsetMax.x : Right
            //-offsetMax.y : Top
            t.offsetMin = new Vector2(delta, bottom);
            t.offsetMax = new Vector2(-delta, top);
        }
        
        private static float GetTopHeight()
        {
            int safeAreaOffset = (int)(Screen.height - Screen.safeArea.yMax);
            return -safeAreaOffset;
        }
        
        private static float GetBottomHeight()  
        {
            return Screen.safeArea.yMin;
        }
    }
}