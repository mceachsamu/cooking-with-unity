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

        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.1,0.1,0.1,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags {
            "LightMode" = "ForwardAdd"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd


            #include "UnityCG.cginc"
            #include "cellShading.cginc"

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


            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float4 _AmbientColor;


            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.x = v.vertex.x + sin(v.vertex.x*10 + time*3)/20;
                v.vertex.z = v.vertex.z + sin(v.vertex.z*10 + time*3)/20;
                float yPos = v.vertex.y;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _DecayMap);
                o.worldNormal = v.normal;
                o.wpos = mul (unity_ObjectToWorld, v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
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
                float4 shading = GetShading(i.wpos, i.vertex, _WorldSpaceLightPos0, i.worldNormal, i.viewDir, col, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                float alpha = getAlpha(i);
                shading.a = alpha;
                fixed4 decay = tex2D(_DecayMap, float2(i.uv.x-0.75,i.uv.y+0.1)) + _DecayAmount;
                if (decay.r < 1.0) {
                    shading.a = 0.0;
                }

                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);

                float waterLevel = 1.0 * waterHeight + _WaterLevel - _MaxHeight;

                if (i.wpos.y < waterLevel){
                    shading.a = 0.0;
                }

                return shading * pow(col, 0.5);
            }
            ENDCG
        }
    }
}
