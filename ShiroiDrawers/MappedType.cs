using System;
using System.Collections.Generic;
using System.Reflection;
using Shiroi.Drawing.Drawers;
using Shiroi.Drawing.Util;
using Shiroi.Serialization;
using UnityEditor;
using UnityEngine;

namespace Shiroi.Drawing {
    public class MappedType {
        private static readonly Dictionary<Type, MappedType> Cache = new Dictionary<Type, MappedType>();

        public static MappedType For(Type type) {
            MappedType t;
            if (Cache.TryGetValue(type, out t)) {
                return t;
            }
            return Cache[type] = new MappedType(type);
        }


        public readonly string Label;
        private readonly List<TypeDrawer> drawers = new List<TypeDrawer>();

        public MappedType(Type type) {
            //Initialize fields
            Label = ObjectNames.NicifyVariableName(type.Name);
            if (Label.EndsWith("Token")) {
                Label = Label.Substring(0, Label.Length - 5);
            }
            SerializedFields = SerializationUtil.GetSerializedMembers(type, true);
            TotalSerializedElements = (uint) SerializedFields.Length;
            //Initialize with label
            Height = EditorGUIUtility.singleLineHeight;
            foreach (var field in SerializedFields) {
                var drawer = TypeDrawers.GetDrawerFor(field.FieldType);
                drawers.Add(drawer);
                Height += drawer.GetTotalLines() * EditorGUIUtility.singleLineHeight;
            }
        }

        private readonly Type type;

        public FieldInfo[] SerializedFields {
            get;
            private set;
        }

        public float Height {
            get;
            private set;
        }

        public uint TotalSerializedElements {
            private set;
            get;
        }

        public Type Type {
            get {
                return type;
            }
        }


        public void DrawFields(
            object obj,
            Rect rect,
            out bool changed
        ) {
            if (!obj.GetType().IsAssignableFrom(type)) {
                throw new Exception(string.Format("'{0}' is not assignable to {1}!", obj, type.Name));
            }
            changed = false;
            //Start at 1 because label
            var currentLine = 0;
            FieldInfo currentField = null;
            Setter setter = value => currentField.SetValue(obj, value);
            for (var index = 0; index < SerializedFields.Length; index++) {
                currentField = SerializedFields[index];
                var fieldType = currentField.FieldType;
                var drawer = drawers[index];

                var fieldName = currentField.Name;
                var typeName = fieldType.Name;
                var totalLines = drawer.GetTotalLines();
                var r = rect.GetLine((uint) currentLine, totalLines);
                currentLine += (int) totalLines;
                var fieldLabel = new GUIContent(ObjectNames.NicifyVariableName(fieldName));
                EditorGUI.BeginChangeCheck();
                drawer.Draw(r, fieldLabel, currentField.GetValue(obj), fieldType, currentField, setter);
                if (EditorGUI.EndChangeCheck()) {
                    changed = true;
                }
            }
        }
    }
}