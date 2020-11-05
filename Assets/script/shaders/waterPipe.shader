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
        _DirectionPrev("Direction Previous", Vector) = (0.0,0.0,0.0,0.0)
        _Count("timer", float) = 0.0
        _PipeSize("pipe size", float) = 1.0
        _PipeLength("length", float) = 0.0
        _PipeRadius("radius", float) = 0.0
        _PipeSegmentsRound("number of segments round", float) = 0.0
        _PipeSegmentsLong("number of segments long", float) = 0.0
        _Exponent("exonential - spout curvature", float) = 0.0

        baseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)

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

            #include "cellShading.cginc"

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;
            float4 _PipeStart;
            float4 _PipeEnd;
            float4 _PreviousEnd;
            float4 _Direction;
            float4 _DirectionPrev;

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
            float _PipeSize;
            float _Exponent;

            float4 getVertexDistortion(float4 vertex, float2 uv){
                float mag = clamp(vertex.z,0.5,5.0);
                vertex.xy *= mag * _PipeSize;

                float4 start = _PipeStart;
                float4 pipeEnd = _PipeEnd;
                float4 prevEnd = _PreviousEnd;
                float pipeLength = _PipeLength;

                float4 direction = normalize(start - _PipeEnd);
                float4 directionPrev = normalize(start - _PreviousEnd);

                float sway = (vertex.z / pipeLength);

                float4 end = (direction * (1.0-sway) + directionPrev * (sway));

                float adjusted = (length(end.xz) / pipeLength);
                vertex.z *= adjusted*1.4;

                float z = length(vertex.z);
                float endZ = length(end.xz);
                float a = (-end.y) / pow(endZ, _Exponent);

                float y = pow(z, _Exponent) * a;
                vertex.y += y;

                float ax = (end.xz) / (endZ*endZ);
                float x = z * z * ax;
                //vertex.x += x;

                #if !defined(SHADER_API_OPENGL)
                    float4 col = tex2Dlod(_NoiseMap, float4(float2(uv.x + _Count/90,uv.y - _Count/60),0,0));
                    float s = (col.r)*(sway);
                    vertex.x += ((s)*0.3*_PipeSize);
                    vertex.z += (s*0.3*_PipeSize);
                #endif
                return vertex;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.uv.z = v.vertex.z;

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

                //next = normalize(getVertexDistortion(next, float2(v.uv.x + (1.0 / _PipeRadius),v.uv.y)));
                //next2 = normalize(getVertexDistortion(next2, float2(v.uv.x - (1.0 / _PipeRadius),v.uv.y)));
//
                //next3 = normalize(getVertexDistortion(next3, float2(v.uv.x + (1.0 / _PipeRadius),v.uv.y + zstep / (_PipeLength * v.vertex.z))));
                //next4 = normalize(getVertexDistortion(next4, float2(v.uv.x - (1.0 / _PipeRadius),v.uv.y + zstep / (_PipeLength * v.vertex.z))));

                float normal = cross(next-next3,next-next4);

                o.normal = normal;
                o.wpos = worldPos;
                //v.vertex.x += sin(_Count/2 * v.vertex.y)/100;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, float2(i.screenPos.x , i.screenPos.y)/i.screenPos.w);

                float4 shading = GetShading(i.wpos, i.vertex, _WorldSpaceLightPos0.xyzw, i.normal, i.viewDir, col, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                return  baseColor - i.uv.z/10;
            }
            ENDCG
        }
    }
}
