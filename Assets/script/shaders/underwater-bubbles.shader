Shader "Particles/underwater-bubbles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightMap("height map", 2D) = "white" {}
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _WaterSize("water size", float) = 0.0
        _WaterLevel("water level", float) = 0.0
        _MaxHeight("max height", float) = 0.0
        _WaterOpaqueness("water opaqueness", float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float3 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 wpos : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _HeightMap;
            float4 _HeightMap_ST;

            uniform float _WaterSize;
            uniform float _MaxHeight;
            uniform float _WaterOpaqueness;
            uniform float _WaterLevel;

            uniform float4 _PotCenter;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float2 getWaterUV(v2f i){
                return ((i.wpos.xz - _PotCenter.xz + _WaterSize/2.0)/_WaterSize);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // use the colour we get from the particle system
                fixed4 col = i.color;
                // just sample the original textures alpha value
                float alpha = tex2D(_MainTex, i.uv).a;
                col.a = alpha;

                // get the water height at this position
                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);
                float waterLevel = 1.0 * waterHeight + _WaterLevel - _MaxHeight;

                // if the fragment is above the water surface, dont render
                if (i.wpos.y < waterLevel){
                    col.r = abs(i.wpos.y - _WaterLevel) * _WaterOpaqueness;
                    col.g = abs(i.wpos.y - _WaterLevel) * _WaterOpaqueness;
                    col.b = abs(i.wpos.y - _WaterLevel) * _WaterOpaqueness;
                }else{
                    col.a = 0.0 ;
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
