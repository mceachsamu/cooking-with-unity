Shader "Unlit/glass"
{
    Properties
    {
        _GlassTexture ("Texture", 2D) = "white" {}
        _BehindGlass ("Behind", 2D) = "white" {}

        [HDR]
        _GlassColor ("color", Color) = (1.0,1.0,1.0,1.0)
        _GlassDistortionX("glass distortion x", range(0.0,10.0)) = 0.0
        _GlassDistortionY("glass distortion y", range(0.0,10.0)) = 0.0
        _GlassTextureFrequency("texture frequency", range(0.0,10.0)) = 1.0
        _GlassTransparency("transparency", range(0.0,1.0)) = 0.0

        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.0,0.0,0.0,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
        _UseNormalMap("Use normal map", int) = 0.0
        _Saturation("Saturation", range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "cellShading.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : NORMAL;

                float4 wpos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };

            sampler2D _GlassTexture;
            float4 _GlassTexture_ST;

            sampler2D _BehindGlass;
            float4 _BehindGlass_ST;

            uniform float4 _GlassColor;
            uniform float _GlassDistortionX;
            uniform float _GlassDistortionY;
            uniform float _GlassTextureFrequency;
            uniform float _GlassTransparency;

            uniform float _Glossiness;
            uniform float4 _SpecularColor;
            uniform float4 _RimColor;
            uniform float _RimAmount;
            uniform float4 _AmbientColor;
            uniform int _UseNormalMap;
            uniform float _Saturation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _GlassTexture);
                o.worldNormal =  UnityObjectToWorldNormal(v.normal);

                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 tex = normalize(tex2D(_GlassTexture, i.uv * _GlassTextureFrequency));
                fixed4 behind = tex2D(_BehindGlass, float2(i.screenPos.x * (1.0 - (tex.r-0.5)*_GlassDistortionX), i.screenPos.y * (1.0 - (tex.r-0.5)*_GlassDistortionY))/i.screenPos.w);
                fixed4 color = _GlassColor;// * _GlassTransparency;

                float4 shading = GetShading(i.wpos, _WorldSpaceLightPos0.xyzw, i.worldNormal, i.viewDir, color, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                return behind * color + tex * shading * _GlassTransparency;
            }
            ENDCG
        }
    }
}
