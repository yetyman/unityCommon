using Assets.Scripts.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GenericClasses
{
    public static class RectTransformExtensions
    {
        public static float TotalWidth(this RectTransform rt)
        {
            return rt.rect.width -rt.offsetMax.x + rt.offsetMin.x;
        }
        public static float TotalHeight(this RectTransform rt)
        {
            return rt.rect.yMax - rt.rect.yMin;// rt.offsetMax.y + rt.offsetMin.y;
        }

        public static void SetRect(this RectTransform rs, float left, float top, float right, float bottom)
        {
            rs.offsetMin = new Vector2(left, bottom);
            rs.offsetMax = new Vector2(-right, -top);
        }
        public static Rect ToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            float x = transform.position.x + transform.anchoredPosition.x;
            float y = Screen.height - transform.position.y - transform.anchoredPosition.y;

            return new Rect(x, y, size.x, size.y);
        }
    }
}
