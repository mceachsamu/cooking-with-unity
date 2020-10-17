Shader "Unlit/underwater"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _HeightMap("heightmap", 2D) = "white" {}
        _MaxHeight("max height", float) = 0.0
        _WaterSize("water size", float) = 0.0
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _WaterOpaqueness("water opaqueness", float) = 0.0
        _WaterLevel("water level", float) = 0.0

        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.0,0.0,0.0,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
        
        adjust("Adjust", Range(-1, 1)) = 0.0
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
            #pragma multi_compile_fwdadd

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD2;
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

            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float4 _AmbientColor;

            float adjust;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = v.normal;
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            float2 getWaterUV(v2f i){
                return ((i.wpos.xz - _PotCenter.xz + _WaterSize/2.0)/_WaterSize);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                //get shading
                float4 shading = GetShading(i.wpos, i.vertex, _WorldSpaceLightPos0.xyzw, i.worldNormal, i.viewDir, col, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                col = shading;

                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);

                float waterLevel = 1.0 * waterHeight + _WaterLevel - _MaxHeight;

                col = col * pow(shading,0.4);

                if (i.wpos.y < waterLevel + 0.05){
                    col.a = (2.0 - pow(abs(i.wpos.y - _WaterLevel),0.5) * _WaterOpaqueness);
                }else{
                    col.a = 0.0;
                }

                return col;
            }
            ENDCG
        }
    }
}
