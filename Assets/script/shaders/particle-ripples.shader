Shader "Unlit/particle-ripples"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap ("noise map", 2D) = "white" {}
        _NoiseMap2 ("noise map2", 2D) = "white" {}
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _WaterSize("water size", float) = 0.0
        _HeightMap("heightmap", 2D) = "white" {}
        _MaxHeight("max height", float) = 0.0
        _WaterLevel("water level", float) = 0.0
        _ItemWorldPosition("item position", Vector) = (0.0,0.0,0.0,0.0)
        _Magnitude("magnitude", float) = 0.0

        _Position ("position", Vector) = (0.0,0.0,0.0,0.0)
        _Counter ("counter", int) = 0
    }
    SubShader
    {
        
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGBA
        Cull Off Lighting Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float3 wpos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;
            sampler2D _NoiseMap2;
            float4 _NoiseMap2_ST;

            sampler2D _HeightMap;
            float4 _HeightMap_ST;
            uniform float _MaxHeight;
            uniform float _WaterLevel;
            uniform float4 _PotCenter;
            uniform float _WaterSize;
            uniform float4 _ItemWorldPosition;
            
            float _Magnitude;
            float4 _Position;
            int _Counter;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;

                return o;
            }

            float2 getWaterUV(v2f i){
                return ((i.wpos.xz - _PotCenter.xz + _WaterSize/2.0)/_WaterSize);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 noise = tex2D(_NoiseMap, i.uv);
                fixed4 noise2 = tex2D(_NoiseMap2, i.uv);
                fixed4 col = tex2D(_MainTex, i.uv);


                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);

                float waterLevel = waterHeight + _WaterLevel - _MaxHeight;

                float heightDiff = clamp(waterLevel - _ItemWorldPosition.y,0.0,1.0)*5;


                float dist = sin(length(_Position - i.wpos) * 40 + noise.r*3 - _Counter/10.0) + heightDiff + noise2.r * 6 * (1 - col.a);
                dist = dist / pow(length(_Position - i.wpos),8);
                col = clamp(col - dist ,0.0,1.0);
                if (col.r > 0.5){
                    col.rgba = float4(1.0,1.0,1.0,1.0);
                }else{
                    col.rgba = float4(0.0,0.0,0.0,0.0);
                }
                return col;
            }
            ENDCG
        }
    }
}
