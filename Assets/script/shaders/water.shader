// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}//the main texture -- not used
        _NoiseMap ("Texture2", 2D) = "white" {}
        _NoiseMap2 ("Texture3", 2D) = "white" {}
        baseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)
        secondaryColor("secondary-color", Vector) = (1.0,0.44,0.0,0.0)
        xRad("xRad", float) = 0.0
        zRad("zRad", float) = 0.0//to do --use to implement oval pots
        center("center", Vector) = (0.0,0.0,0.0,0.0)//center of pot water
        spoon_end("spoon-end", Vector) = (0.0,0.0,0.0,0.0)//center of spoon end
        time("time", float) = 0.0 //increasing timer to help with animations
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;
            sampler2D _NoiseMap2;
            float4 _NoiseMap2_ST;

            uniform float xRad;
            uniform float zRad;
            uniform float time;

            uniform float4 center;
            uniform float4 spoon_end;

            uniform float4 baseColor;

            uniform float4 secondaryColor;
            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //get world position from object position
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;
                // // sample the texture
                 #if !defined(SHADER_API_OPENGL)
                    float4 tex = tex2Dlod (_MainTex, float4(v.uv.xy,0,0));
                    v.vertex.y = tex.r;
                #endif
                // float4 col = tex2dlod(_MainTex, v.uv);

 //               v.vertex.y = v.vertex.y + col.x;
                //recalculate vertex position after distortion
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                worldPos = mul (unity_ObjectToWorld, v.vertex);
                worldPos.y = worldPos.y;
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
                fixed4 col = tex2D(_MainTex, i.uv);
                //get the value of the noise maps at this fragmant
                fixed4 noiseMap = tex2D(_NoiseMap, i.uv);
                fixed4 noiseMap2 = tex2D(_NoiseMap2, i.uv);
                //check to see if we should render this fragment (if its inside the pot
                float alpha = getAlpha(i);
                //get the value of the two noise maps at this fragment
                float worldCenterDistance = length(i.wpos.xz - center.xz);
                //playing around with noise maps to create cool wave shapes
                float amplitude = sin(worldCenterDistance*40 - time*1.3 + noiseMap.x*10) + pow(noiseMap2.x + 0.5,2)*2;
                amplitude = clamp(amplitude, 0.0,1.0);

                //get the distance between the end of the spoon and this fragment
                float distToSpoonEnd = length(spoon_end.xyz - i.wpos.xyz);
                float Distortion = distToSpoonEnd- sin(time + noiseMap2.x*20)*0.02;
                if (amplitude < 0.7 || Distortion < 0.3){
                    col = float4(baseColor.x,baseColor.y,baseColor.z,alpha);
                }else{
                    col = float4(secondaryColor.x,secondaryColor.y,secondaryColor.z,alpha);
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
