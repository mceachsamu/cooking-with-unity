Shader "Unlit/candle-light"
{
    Properties
    {
        _NoiseMap ("noise texture", 2D) = "white" {}
        [HDR]
        _Color ("Color", Color) = (0.0,0.0,0.0,0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;

            uniform float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                float4 noise = tex2Dlod (_NoiseMap, float4(float2(v.uv.x + _Time.y/4.0,v.uv.y + _Time.y/2.0),0,0));
                v.vertex.xy *= (1.0 + noise.r/5.0);
                v.vertex.z *= (1.0 + noise.r/3.0);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseMap);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;

                return col;
            }
            ENDCG
        }
    }
}
