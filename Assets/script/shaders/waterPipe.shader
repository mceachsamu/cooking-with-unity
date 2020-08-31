Shader "Unlit/waterPipe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap ("noise map", 2D) = "white" {}
        _PipeStart("pipe start", Vector) = (0.0,0.0,0.0,0.0)
        _PipeEnd("pipe end", Vector) = (0.0,0.0,0.0,0.0)
        _PreviousEnd("pipe end previous", Vector) = (0.0,0.0,0.0,0.0)
        _Direction("direction", Vector) = (0.0,0.0,0.0,0.0)
        _Count("timer", float) = 0.0
        _PipeLength("length", float) = 0.0
        _PipeRadius("radius", float) = 0.0
        _PipeSegmentsRound("number of segments round", float) = 0.0
        _PipeSegmentsLong("number of segments long", float) = 0.0

        baseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)

        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.0,0.0,0.0,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
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
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 normal : NORMAL;
                float4 wpos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;
            float4 _PipeStart;
            float4 _PipeEnd;
            float4 _PreviousEnd;
            float4 _Direction;

            float4 _LightPos;

            float4 baseColor;
            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float4 _AmbientColor;

            float _Count;
            float _PipeLength;
            float _PipeRadius;
            float _PipeSegmentsRound;
            float _PipeSegmentsLong;

            float4 getVertexDistortion(float4 vertex, float2 uv){
                float mag = vertex.z;
                vertex *= mag;
                float4 start = _PipeStart;
                float4 pipeEnd = _PipeEnd;
                float4 prevEnd = _PreviousEnd;

                float adjust = 1.7;
                pipeEnd.y = pipeEnd.y - adjust;
                prevEnd.y = pipeEnd.y - adjust;
                float startZ = vertex.z;

                float AdjustedPipeLength = _PipeLength*mag;

                float sway = vertex.z / AdjustedPipeLength;

                float4 end = (pipeEnd * (1.0-sway) + prevEnd * (sway));

                float adjusted = (length(start.xz-end.xz) / AdjustedPipeLength);
                vertex.z *= adjusted;

                float z = length(vertex.z);
                float endZ = length(start.zx-end.zx);
                float a =  (end.y) / (endZ*endZ);
                float y = z * z * a;
                vertex.y += y;
                //dot product using direction vector?
                float endX = (start.z-end.z);
                vertex.x -= endX*sway;
                float endZ2 = (start.x-end.x);
                vertex.z -= endZ2*sway;

                #if !defined(SHADER_API_OPENGL)
                    float4 col = tex2Dlod (_NoiseMap, float4(float2(uv.x + _Count/500,uv.y - _Count/80),0,0));
                    float s = (col.r)*(sway);
                    vertex.x += (s*1.0) - 0.5*sway;
                #endif
                return vertex;
            }

            v2f vert (appdata v)
            {
                v2f o;

                v.vertex = getVertexDistortion(v.vertex, v.uv);
                float PI = 3.14159265359;
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                float4 start = normalize(_Direction) * v.vertex.z + _PipeStart;
                float4 angle = PI * 2 / _PipeSegmentsRound;

                float4 H = start - worldPos;
                float y = cos(angle) * H;
                float x = sin(angle) * H;
                float4 next = float4(start.x + x, start.y + y, start.z, start.w);
                float4 next2 = float4(start.x - x, start.y - y, start.z, start.w);
                float zstep = 0.1;
                float4 next3 = float4(start.x + x, start.y + y, start.z + zstep, start.w);
                float4 next4 = float4(start.x - x, start.y - y, start.z + zstep, start.w);

                // next = normalize(getVertexDistortion(next, float2(v.uv.x + (1.0 / _PipeRadius),v.uv.y)));
                // next2 = normalize(getVertexDistortion(next2, float2(v.uv.x - (1.0 / _PipeRadius),v.uv.y)));

                // next3 = normalize(getVertexDistortion(next3, float2(v.uv.x + (1.0 / _PipeRadius),v.uv.y + zstep / (_PipeLength * v.vertex.z))));
                // next4 = normalize(getVertexDistortion(next4, float2(v.uv.x - (1.0 / _PipeRadius),v.uv.y + zstep / (_PipeLength * v.vertex.z))));

                float normal = cross(next-next3,next-next4);

                o.normal = normal;
                o.wpos = worldPos;
                //v.vertex.x += sin(_Count/2 * v.vertex.y)/100;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            //based on blinn phong shading
            float4 getShading (v2f i)
            {
                fixed4 col = baseColor;
                float3 lightDir = normalize(_LightPos - i.wpos);
                float NdotL = dot(i.normal, lightDir);
                float intensity =   smoothstep(0, 0.1, NdotL);
                float3 viewDir = i.viewDir;

                float3 H = normalize(_LightPos + viewDir);
                float NdotH = dot(i.normal, H);
                float specIntensity = pow(NdotH * intensity, _Glossiness * _Glossiness);

                float specularIntensitySmooth = smoothstep(0.005, 0.01, specIntensity);
                float4 specular = specularIntensitySmooth * _SpecularColor;

                float4 rimDot = 1 - dot(viewDir, i.normal);

                float rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimDot);
                float4 rim = rimIntensity * _RimColor;
                float overall = intensity * 0.5;

                if (overall < 0.0){
                    overall = 0.5;
                }
                if (overall > 0.0){
                    overall = 1.0;
                }

                return (baseColor + overall + specular + rim);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, float2(i.screenPos.x , i.screenPos.y)/i.screenPos.w);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float4 shading = getShading(i);
                return float4(col.b,col.b,col.b,col.a)/2 + baseColor * shading/2;
            }
            ENDCG
        }
    }
}
