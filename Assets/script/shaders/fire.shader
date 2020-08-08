Shader "Particles/fire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap("noise map", 2D) = "white" {}
        _Count("timer", float) = 0.0
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
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
                float4 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 wpos : TEXCOORD1;
                float3 worldNormal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;

            uniform float _Count;
            uniform float4 _LightPos;

            v2f vert (appdata v)
            {
                v2f o;
                #if !defined(SHADER_API_OPENGL)
                    float4 noise = tex2Dlod (_NoiseMap, float4(float2(v.uv.x,v.uv.y),0,0));
                    v.vertex.x = v.vertex.x*(noise.r +1 )+ sin(v.vertex.y*10)/100;
                    v.vertex.z = v.vertex.z*(noise.r +1 )+ sin(v.vertex.y*10)/010;
                #endif
                v.vertex.x = v.vertex.x + sin(v.vertex.y*10)/100;

                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex.x = v.vertex.x + v.vertex.x;
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.uv.z=v.uv.z;

                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;
                o.worldNormal = v.normal;
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                return o;
            }


            float getShading (v2f i)
            {
                float3 lightDir = normalize(_LightPos - i.wpos);
                float NdotL = dot(i.worldNormal, lightDir);
                float intensity = saturate(NdotL);
                float3 camDir = normalize(_WorldSpaceCameraPos - i.wpos);

                float3 H = normalize(lightDir + camDir);
                float NdotH = dot(i.worldNormal, H);
                float specIntensity = saturate(NdotH);

                float overall = intensity + specIntensity;
                return overall;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float alpha = tex2D(_MainTex, i.uv).a;

                float alpha2 = tex2D(_NoiseMap, float2(i.uv.x,i.uv.y + _Count/100)).r;
                fixed4 color = i.color;
                color.a = alpha;
                float shading = getShading(i);
                color = color * shading;
                // color.a = clamp(alpha - alpha2*1.5,0.0,1.0);
                // if (color.a < 0.3){
                //     color.a = 0.0;
                // }else{
                //     color.a = 1.0;
                // }
                //color.r = color.r + i.vertex.y/10;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                //col.a = 0.5;
                return color;
            }
            ENDCG
        }
    }
}
