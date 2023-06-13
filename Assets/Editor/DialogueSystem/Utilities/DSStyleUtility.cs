using UnityEditor;
using UnityEngine.UIElements;

namespace DS.Utilities
{

    public static class DSStyleUtility
    {
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach(string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        // "this" allows it to be called like visualElement.AddStyleSheets()
        // "params string[]" allows us to give the strings listed and separated by comma, like:
        // visualElement.AddStyleSheets("sheet1", "sheet2", "sheet3");
        // (only works with array and as the last parameter)
        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach(string styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName);
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }

}