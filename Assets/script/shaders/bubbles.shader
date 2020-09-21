Shader "Unlit/bubbles"
{
    Properties
    {
        _HeightMap("heightmap", 2D) = "white" {}
        _DecayMap ("decay-map", 2D) = "white" {}
        _DecayAmount ("decay-amount", float) = 0.0
        _MaxHeight("max height", float) = 0.0
        _WaterSize("water size", float) = 0.0
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
        _Color("color", Vector) = (1.0,1.0,1.0,0.0)
        _Color2("color2", Vector) = (1.0,1.0,1.0,0.0)
        xRad("xRad", float) = 0.0
        zRad("zRad", float) = 0.0//to do --use to implement oval pots
        center("center", Vector) = (0.0,0.0,0.0,0.0)//center of pot water
        time("time", float) = 0.0 //increasing timer to help with animations
        waterSize("waterSize", float) = 0.0//the size of the water we are rendering on for height purposes
        _WaterLevel("water level", float) = 0.0
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


            sampler2D _HeightMap;
            float4 _HeightMap_ST;

            sampler2D _DecayMap;
            float4 _DecayMap_ST;

            uniform float4 _PotCenter;
            uniform float _WaterSize;
            uniform float _MaxHeight;
            uniform float _DecayAmount;
            uniform float4 _LightPos;
            uniform float4 _Color;
            uniform float4 _Color2;
            uniform float xRad;
            uniform float zRad;
            uniform float time;
            uniform float waterSize;
            uniform float4 center;
            uniform float _WaterLevel;

            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.x = v.vertex.x + sin(v.vertex.x*10 + time*3)/20;
                v.vertex.z = v.vertex.z + sin(v.vertex.z*10 + time*3)/20;
                float yPos = v.vertex.y;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _DecayMap);
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
                float alpha = 0.7;
                //if the distance from the center to the camera vector is greater than the radius
                //of the circle, make the fragmant invisible.
                //TODO condider oval shapes (use zRadius)
                if (distFromCenter > xRad){
                    alpha = 0.0;
                }
                return alpha;
            }

            float2 getWaterUV(v2f i){
                return ((i.wpos.xz - _PotCenter.xz + _WaterSize/2.0)/_WaterSize);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                fixed4 col2 = _Color2;
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
                    col = col*0.8;
                }else if(overall < 1.0){
                    col = col2*0.95;
                }else if(overall < 2.5){
                    col = col2* 1.0;
                }else{
                    col = col2;
                }
                col.a = alpha;
                fixed4 decay = tex2D(_DecayMap, float2(i.uv.x-0.75,i.uv.y)) + _DecayAmount;
                if (decay.r < 1.0) {
                    col.a = 0.0;
                }

                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);

                float waterLevel = 1.0 * waterHeight + _WaterLevel - _MaxHeight;

                if (i.wpos.y < waterLevel){
                    col.a = 0.0;
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
