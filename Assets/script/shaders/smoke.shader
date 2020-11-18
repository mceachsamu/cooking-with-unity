Shader "Unlit/smoke"
{
    Properties
    {

        _NoiseMap ("noise map", 2D) = "white" {}
        _Color ("color", Color) = (0.0,0.0,0.0,0.0)
        _Count ("count", int) = 0
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            uniform sampler2D _NoiseMap;
            uniform float4 _NoiseMap_ST;

            uniform float4 _Color;
            uniform int _Count;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.x -= sin(v.vertex.z*1600 - _Count/20.0)/5000;// -  v.vertex.z;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseMap);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 NoiseMap = tex2D(_NoiseMap, float2(i.uv.x, i.uv.y));
                
                fixed4 col = _Color;
                col.a = clamp(i.vertex.y/2000 - NoiseMap.r , 0, 1)*0.0;
                return col;
            }
            ENDCG
        }
    }
}
