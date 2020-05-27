using UnityEngine;
using System.Collections;
/// <summary>
/// HTML标记类辅助方法
/// https://docs.unity3d.com/Manual/StyledText.html
/// </summary>
public class HtmlMarkUp 
{
    /// <summary>
    /// Sets the size of the text according to the parameter value, given in pixels.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="pixelSize"></param>
    public static string GetFontSizeString(string str,int pixelSize)
    {
        return string.Format("<size={0}>{1}</size>", pixelSize, str);
    }
	
}
