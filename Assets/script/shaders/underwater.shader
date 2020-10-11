Shader "Unlit/underwater"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _HeightMap("heightmap", 2D) = "white" {}
        _MaxHeight("max height", float) = 0.0
        _WaterSize("water size", float) = 0.0
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _WaterOpaqueness("water opaqueness", float) = 0.0
        _WaterLevel("water level", float) = 0.0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGBA

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
                float4 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float3 worldNormal : NORMAL;
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
            uniform float4 _LightPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                float2 uv = ((worldPos.xz - _PotCenter.xz - _WaterSize/2.0)/_WaterSize);
                #if !defined(SHADER_API_OPENGL)
                    float4 height = tex2Dlod (_HeightMap, float4(float2(uv.x,uv.y),0,0));
                    //v.vertex.z = v.vertex.z + height.r/100.0;
                #endif

                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex.x = o.vertex.x + sin(o.vertex.x*10 + time)/10;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = v.normal;
                worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;
                UNITY_TRANSFER_FOG(o,o.vertex);
                //o.vertex = o.vertex * noise.y;
                return o;
            }

            float2 getWaterUV(v2f i){
                return ((i.wpos.xz - _PotCenter.xz + _WaterSize/2.0)/_WaterSize);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 lightDir = normalize(_LightPos - i.wpos);
                float NdotL = dot(i.worldNormal, lightDir);
                float intensity = saturate(NdotL);
                float3 camDir = normalize(_WorldSpaceCameraPos - i.wpos);

                float3 H = normalize(lightDir + camDir);
                float NdotH = dot(i.worldNormal, H);
                float specIntensity = saturate(NdotH);

                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);

                float overall = intensity + specIntensity;
                // apply fog
                if(overall < 0.2){
                    col = col*0.4;
                }
                if(overall < 0.6){
                    col = col*0.5;
                }
                if(overall < 1.0){
                    col = col* 1.0;
                }

                float waterLevel = 1.0 * waterHeight + _WaterLevel - _MaxHeight ;

                if (i.wpos.y < waterLevel + 0.05){
                    col.a =  2.0 - pow(abs(i.wpos.y - _WaterLevel),0.5) * _WaterOpaqueness;
                }else{
                    col.a = 0.0;
                }
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
