// Machinika Museum
// © Littlefield Studio
// Writted by Franck-Olivier FILIN - 2017
//
// InteractiveObjectEditor.cs

#if UNITY_EDITOR

using UnityEditor;
using NodalInteractiveCreator.Objects;

namespace NodalInteractiveCreator.Editor.Objects
{
    [CustomEditor(typeof(InteractiveObject))]
    [CanEditMultipleObjects]
    public class InteractiveObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif