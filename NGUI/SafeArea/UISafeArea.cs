using ES_TK.Misc;
using UnityEngine;

namespace ES_TK.UI.Common
{
    public class UISafeArea : UIWidget
    {
        private static readonly int rootWidth = 1920;
        
        private static readonly int rootHeight = 1080;
        
        protected override void OnStart()
        {
            base.OnStart();
            UpdateWidgetSize();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            UpdateWidgetSize();
        }

        public void UpdateWidgetSize()
        {
            GetSizePosSafeArea(out Vector3 localPos, out Vector3 size);
            transform.localPosition = localPos;
            width = (int) size.x;
            height = (int) size.y;
        }

        private void GetSizePosSafeArea(out Vector3 localPos, out Vector3 size)
        {
            var safeArea = GetSafeAreaRect(root, out Vector2 panelSize);
            localPos = safeArea.position;
            size = safeArea.size;
        }

        private static Rect GetSafeAreaRect(UIRoot root, out Vector2 panelSize)
        {
            var rootSize = root == null
                ? new Vector2(rootWidth, rootHeight)
                : new Vector2(root.manualWidth, root.manualHeight);
            var rootRatio = rootSize.x / rootSize.y;
            var screenRatio = NGUITools.screenSize.x / NGUITools.screenSize.y;
            var ratio = rootRatio < screenRatio
                ? (rootSize / NGUITools.screenSize).y
                : (rootSize / NGUITools.screenSize).x;
            var safeAreaSize = Screen.safeArea.size * ratio;
            panelSize = NGUITools.screenSize * ratio;
            var safeAreaPos = Screen.safeArea.position * ratio;
            var posX = safeAreaPos.x - (panelSize.x - safeAreaSize.x) * 0.5f;
            var posY = safeAreaPos.y - (panelSize.y - safeAreaSize.y) * 0.5f;
            
            var posXY = new Vector2(posX, posY);
            
            return new Rect(posXY, safeAreaSize);
        }
    }
}
