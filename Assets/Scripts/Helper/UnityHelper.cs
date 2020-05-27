using UnityEngine;


public class UnityHelper
{
    public static Sprite LoadSprite(string spriteName)
    {
        object obj = Resources.Load("UITextures/" + spriteName, typeof(Sprite));
        Sprite sp = obj as Sprite;
        return sp;
    }
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
