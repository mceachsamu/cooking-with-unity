Shader "Particles/fire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap("noise map", 2D) = "white" {}
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;

            v2f vert (appdata v)
            {
                v2f o;
                #if !defined(SHADER_API_OPENGL)
                    float4 noise = tex2Dlod (_NoiseMap, float4(float2(v.uv.x,v.uv.y),0,0));
                    v.vertex.x = v.vertex.x*(noise.r +1 )+ sin(v.vertex.y*10)/10;
                    v.vertex.z = v.vertex.z*(noise.r +1 )+ sin(v.vertex.y*10)/10;
                #endif
                v.vertex.x = v.vertex.x + sin(v.vertex.y*10)/10;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex.x = v.vertex.x + v.vertex.x;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float alpha = tex2D(_MainTex, i.uv).a;
                fixed4 color = i.color;
                color.a = alpha;
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
