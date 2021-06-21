using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(WaterRippleRenderer), PostProcessEvent.AfterStack, "Custom/Water Ripple", false)]
public class WaterRipple : PostProcessEffectSettings
{
    public TextureParameter noise = new TextureParameter { value = null };
    public Vector2Parameter speed = new Vector2Parameter { value = new Vector2(0, 1f) };
    [Range(0, 1f)]
    public FloatParameter intensity = new FloatParameter { value = 0.035f };
}

public class WaterRippleRenderer : PostProcessEffectRenderer<WaterRipple>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/WaterRipple"));
        sheet.properties.SetTexture("_Noise", settings.noise);
        sheet.properties.SetVector("_Speed", settings.speed);
        sheet.properties.SetFloat("_Intensity", settings.intensity);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
