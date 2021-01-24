using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer), PostProcessEvent.BeforeStack, "outline")]
public sealed class PostProcessOutline : PostProcessEffectSettings
{
    public IntParameter scale = new IntParameter { value = 1 };

    public FloatParameter depthThreshold = new FloatParameter { value = 0.2f };

    [Range(0, 1)]
    public FloatParameter normalThreshold = new FloatParameter { value = 0.4f };

    public ColorParameter color = new ColorParameter { value = Color.white };

    public TextureParameter texture = new TextureParameter {};
}

public sealed class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/outline"));
        sheet.properties.SetFloat("_Scale", settings.scale);
        sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
        sheet.properties.SetFloat("_NormalThreshold", settings.normalThreshold);
        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetTexture("_Texture", settings.texture);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}