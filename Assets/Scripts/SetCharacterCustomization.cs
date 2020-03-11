using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCharacterCustomization: MonoBehaviour
{
    public enum BodyTypes { A, B, C, D}

    public BodyTypes bodyTypes = BodyTypes.A;

    public Color SkinColor = Color.gray;
    public Color ShirtColor = Color.white;
    public Color PantsColor = Color.blue;

    public SkinnedMeshRenderer SkinObject;
    public SkinnedMeshRenderer ShirtObject;
    public SkinnedMeshRenderer PantsObject;

    private void Awake()
    {
        bodyTypes = (BodyTypes)SaveManager.BodyType;
        ColorUtility.TryParseHtmlString(SaveManager.SkinColor, out SkinColor);
        ColorUtility.TryParseHtmlString(SaveManager.ShirtColor, out ShirtColor);
        ColorUtility.TryParseHtmlString(SaveManager.PantsColor, out PantsColor);
    }
    private void Start()
    {
        SkinObject.material.color = SkinColor;
        if (bodyTypes != BodyTypes.A) SkinObject.SetBlendShapeWeight((int)bodyTypes - 1, 100);
        ShirtObject.material.color = ShirtColor;
        if (bodyTypes != BodyTypes.A) ShirtObject.SetBlendShapeWeight((int)bodyTypes - 1, 100);
        PantsObject.material.color = PantsColor;
        if (bodyTypes != BodyTypes.A) PantsObject.SetBlendShapeWeight((int)bodyTypes - 1, 100);
    }
}
