using System;
using UnityEngine;

namespace RapidGUI
{
    public static partial class RGUI
    {
        delegate object SliderFunc(object v, object min, object max);

        public static class SliderSetting
        {
            public static float minWidth = 150f;
            public static float fieldWidth = 50f;
        }

        public static object Slider(object obj, object min, object max, Type type, string label, params GUILayoutOption[] options)
        {
            using (new GUILayout.VerticalScope(options))
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("<b>" + label + "</b>");
            }

            return obj;
        }

        public static float SliderFloat(float v, float min, float max, float defaultValue, string label = null)
        {
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>" + label + "</b>", GUILayout.MinWidth(SliderSetting.minWidth));
            var ret = GUILayout.HorizontalSlider(v, min, max, GUILayout.MinWidth(SliderSetting.minWidth));
            ret = (float)StandardField(ret, v.GetType(), GUILayout.Width(SliderSetting.fieldWidth));

            GUI.backgroundColor = Color.black;
            if (GUILayout.Button("<b>Reset</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                ret = defaultValue;
            }
            GUILayout.EndHorizontal();
            return ret;
        }
    }
}