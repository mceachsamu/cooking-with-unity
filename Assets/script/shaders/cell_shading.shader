Shader "Unlit/cell_shading"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _HeightMap("heightmap", 2D) = "white" {}
        _WaterSize("water size", float) = 0.0
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
        _WaterOpaqueness("water opaqueness", float) = 0.0
        _WaterLevel("water level", float) = 0.0
        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.0,0.0,0.0,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags {
            "LightMode" = "ForwardBase"
	        "PassFlags" = "OnlyDirectional"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #include "Lighting.cginc"

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
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _HeightMap;
            float4 _HeightMap_ST;
            uniform float _WaterSize;
            uniform float4 _PotCenter;
            uniform float4 _LightPos;
            uniform float _WaterOpaqueness;
            uniform float _WaterLevel;

            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float4 _AmbientColor;

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
                o.viewDir = WorldSpaceViewDir(v.vertex);
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
                float intensity = smoothstep(0, 0.01, NdotL);
                float3 viewDir = i.viewDir;

                float3 H = normalize(_LightPos + viewDir);
                float NdotH = dot(i.worldNormal, H);
                float specIntensity = pow(NdotH * intensity, _Glossiness * _Glossiness);

                float specularIntensitySmooth = smoothstep(0.005, 0.01, specIntensity);
                float4 specular = specularIntensitySmooth * _SpecularColor;

                float4 rimDot = 1 - dot(viewDir, i.worldNormal);

                float rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimDot);
                float4 rim = rimIntensity * _RimColor;

                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);

                float overall = 1.0;

                if (intensity > 0){
                    overall = 1.0;
                }else{
                    overall = 0.0;
                }


                float waterLevel = waterHeight + _WaterLevel;


                UNITY_APPLY_FOG(i.fogCoord, col);
                return col * (_AmbientColor + overall + specular + rim) ;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
