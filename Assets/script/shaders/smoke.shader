Shader "Unlit/smoke"
{
    Properties
    {

        _NoiseMap ("noise map", 2D) = "white" {}
        _Color ("color", Color) = (0.0,0.0,0.0,0.0)
        _Count ("count", int) = 0

        [HDR]
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
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" "LightMode" = "ForwardAdd"}
        LOD 200
        ColorMask RGBA
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "cellShading.cginc"
            #include "UnityLightingCommon.cginc"

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
                float4 screenPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 wpos : TEXCOORD4;
            };

            uniform sampler2D _NoiseMap;
            uniform float4 _NoiseMap_ST;

            uniform float4 _Color;
            uniform int _Count;

            uniform float _Glossiness;
            uniform float4 _SpecularColor;
            uniform float4 _RimColor;
            uniform float _RimAmount;
            uniform float4 _AmbientColor;

            v2f vert (appdata v)
            {
                v2f o;
                #if !defined(SHADER_API_OPENGL)
                    float4 noise = tex2Dlod (_NoiseMap, float4(v.uv.x, v.uv.y + _Count/1000.0,0,0));
                    //v.vertex.z += (noise.r-0.5)/100.0;
                #endif

                //v.vertex.xz -= sin(v.vertex.zy*1600 - _Count/20.0)/5000;// -  v.vertex.z;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
                o.worldNormal =  UnityObjectToWorldNormal(v.normal);
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseMap);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 NoiseMap = tex2D(_NoiseMap, float2(i.uv.x - _Count/1000.0, i.uv.y - _Count/1000.0));

                fixed4 col = _Color;

                float4 shading = GetShading(i.wpos, _WorldSpaceLightPos0.xyzw, i.worldNormal, i.viewDir, col, _LightColor0, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                //col.a = clamp(i.vertex.y/2000 - NoiseMap.r , 0, 1)*0.5;
                col.a = 0.2;
                shading.a = 1.0;
                return NoiseMap;//shading * col;
            }
            ENDCG
        }
    }
}
