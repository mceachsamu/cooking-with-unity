Shader "Unlit/cell_shading"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
        _WaterLevel("water level", float) = 0.0
        _WaterOpaqueness("water opaqueness", float) = 0.0
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
            // make fog work
            #pragma multi_compile_fog

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

            uniform float4 _LightPos;
            uniform float _WaterLevel;
            uniform float _WaterOpaqueness;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex.x = o.vertex.x + sin(o.vertex.x*10 + time)/10;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = v.normal;
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;
                UNITY_TRANSFER_FOG(o,o.vertex);
                //o.vertex = o.vertex * noise.y;
                return o;
            }

            float getAlpha(v2f i) {
                float dist = _WaterLevel - i.wpos.y;
                if (dist > 0.0){
                    return 0.0;
                }
                float alpha = dist * _WaterOpaqueness;
                return alpha;
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

                float overall = intensity + specIntensity;
                // apply fog
                if(overall < 0.2){
                    col = col*0.4 ;
                }
                if(overall < 0.6){
                    col = col*0.5;
                }
                if(overall < 1.0){
                    col = col* 1.0;
                }

                float alpha = getAlpha(i);
                col =  col + alpha;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
