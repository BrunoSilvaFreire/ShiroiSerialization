using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shiroi.Drawing.Drawers {
    public class NotFoundDrawer : TypeDrawer {
        public override int GetPriority() {
            return -1;
        }

        public override bool Supports(Type type) {
            return true;
        }

        public override void Draw(
            Rect rect,
            GUIContent label,
            object value,
            Type valueType,
            FieldInfo fieldInfo,
            Setter setter) {
            var message = string.Format("Couldn't find drawer for field '{0}' of type '{1}'", label, valueType.Name);
            EditorGUI.LabelField(rect, message);
        }
    }
}