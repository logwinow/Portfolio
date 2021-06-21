using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LanguageNode
{
    public string russianText;
    public string englishText;

    public string GetText(Language.LaguageType language)
    {
        if (language == Language.LaguageType.English)
        {
            return englishText;
        }
        else
            return russianText;
    }

    public string GetText()
    {
        return GetText(Language.laguage);
    }
}
