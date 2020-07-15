Shader "Unlit/bubbles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap ("noise-map", 2D) = "white" {}
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
        _Color("color", Vector) = (1.0,1.0,1.0,0.0)
        _Color2("color2", Vector) = (1.0,1.0,1.0,0.0)
        xRad("xRad", float) = 0.0
        zRad("zRad", float) = 0.0//to do --use to implement oval pots
        center("center", Vector) = (0.0,0.0,0.0,0.0)//center of pot water
        time("time", float) = 0.0 //increasing timer to help with animations
        waterSize("waterSize", float) = 0.0//the size of the water we are rendering on for height purposes
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

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
            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;
            uniform float4 _LightPos;
            uniform float4 _Color;
            uniform float4 _Color2;
            uniform float xRad;
            uniform float zRad;
            uniform float time;
            uniform float waterSize;
            uniform float4 center;
            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.x = v.vertex.x + sin(v.vertex.x*10 + time*3)/20;
                v.vertex.z = v.vertex.z + sin(v.vertex.z*10 + time*3)/20;
                float yPos = v.vertex.y;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
               // o.vertex = UnityObjectToClipPos(v.vertex);
                
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = v.normal;
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float getAlpha (v2f i)
            {
                //get the distance between this fragment and this center of the pot
                float worldCenterDistance = length(i.wpos.xz - center.xz);
                //t is a constant we need to calculate to get the equation
                //fot the vector that goes from the camera to the fragment
                float t = (center.y - _WorldSpaceCameraPos.y) / (i.wpos.y - _WorldSpaceCameraPos.y);
                //use this constant to calculate the intercept between a vector going from the center to the
                //pot to the vector going from the camera to the fragment
                float x = _WorldSpaceCameraPos.x + t*(i.wpos.x - _WorldSpaceCameraPos.x);
                float z = _WorldSpaceCameraPos.z + t*(i.wpos.z - _WorldSpaceCameraPos.z);
                float3 intercept = float3(x, center.y,z);
                float3 centerToIntercept = center.xyz - intercept;
                //get the distance for this vector
                //distance the fragmant is from the center of the circle
                float distFromCenter = length(centerToIntercept);
                //declare our alpha value so by default the texture is opaque
                float alpha = 1.0;
                //if the distance from the center to the camera vector is greater than the radius
                //of the circle, make the fragmant invisible.
                //TODO condider oval shapes (use zRadius)
                if (distFromCenter > xRad){
                    alpha = 0.0;
                }
                return alpha;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color2;
                fixed4 col2 = _Color;
                float alpha = getAlpha(i);
                float3 lightDir = normalize(_LightPos - i.wpos);
                float NdotL = dot(i.worldNormal, lightDir);
                float intensity = saturate(NdotL);
                float3 camDir = normalize(_WorldSpaceCameraPos - i.wpos);

                float3 H = normalize(lightDir + camDir);
                float NdotH = dot(i.worldNormal, H);
                float specIntensity = saturate(NdotH);

                float overall = intensity + specIntensity;
                // apply fog
                
                if(overall < 0.9){
                    col = col*0.1;
                }else if(overall < 1.0){
                    col = col*0.8;
                }else if(overall < 2.5){
                    col = col* 0.9;
                }else{
                    col = col;
                }
                //col = col2 * overall;
                col.a = alpha;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
