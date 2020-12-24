// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/water"
{
    Properties
    {
        _Tex ("Texture", 2D) = "white" {}//the main texture-- used as the height map
        _RenderTex ("Render texture", 2D) = "white" {}
        _Texture ("tex", 2D) = "white" {}
        _RenderTexAbove ("Render texture above", 2D) = "white" {}
        _BaseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)
        _xRad("xRad", float) = 0.0
        _Seperation("seperation", float) = 0.0
        _TotalSize("totalSize", float) = 0.0
        _MaxHeight("max height", float) = 0.0
        _Center("center", Vector) = (0.0,0.0,0.0,0.0)//_Center of pot water


        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.1,0.1,0.1,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {

        Blend One One
        Tags {"RenderType"="Opaque"
            "LightMode"="ForwardAdd" }
        Lighting On
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "cellShading.cginc"
            #include "waterDistortion.cginc"
            #include "pot-cull.cginc"
            #pragma multi_compile_fwdadd_fullshadows

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD3;
                float4 pos : TEXCOORD4;
            };

            sampler2D _Tex;
            float4 _Tex_ST;

            sampler2D _Texture;
            float4 _Texture_ST;

            sampler2D _RenderTex;
            float4 _RenderTex_ST;

            sampler2D _RenderTexAbove;
            float4  _RenderTexAbove_ST;

            uniform float _Seperation;
            uniform float _TotalSize;
            uniform float _xRad;
            uniform float zRad;
            uniform float _MaxHeight;

            uniform float4 _Center;
            uniform float4 _BaseColor;

            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float4 _AmbientColor;

            v2f vert (appdata v)
            {
                v2f o;

                o.uv = TRANSFORM_TEX(v.uv, _Tex);
                //get world position from object position
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;

                waterOutput w = GetWaterDistortion(_Tex, v.vertex, o.uv, _Seperation, _TotalSize, _MaxHeight);
                v.vertex.y = w.vertex.y;

                o.vertex = UnityObjectToClipPos(v.vertex);
                worldPos = mul (unity_ObjectToWorld, v.vertex);
                worldPos.y = w.vertex.y;
                o.worldNormal = w.worldNorm;

                o.pos = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _Tex);
				o.screenPos = ComputeScreenPos(o.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _BaseColor;
                //check to see if we should render this fragment (if its inside the pot)
                float alpha = getAlpha(i.wpos, _Center, _xRad);

                float4 shading = GetShading(i.wpos, _WorldSpaceLightPos0, i.worldNormal, i.viewDir, _BaseColor, _RimColor, _SpecularColor, _RimAmount, _Glossiness);
                //render the render texure relative to screen position
                fixed4 tex = tex2D(_RenderTex, float2(i.screenPos.x, i.screenPos.y + i.pos.y/1.5+0.3)/i.screenPos.w);
                fixed4 aboveTex = tex2D(_RenderTexAbove, i.uv);

                col = col*shading - tex * clamp(1.0 - tex.a,0.0,1.0) * 0.4;
                col.a = alpha;

                return col;
            }
            ENDCG
        }

    }
}
