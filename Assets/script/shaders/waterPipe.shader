Shader "Unlit/waterPipe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PipeStart("pipe start", Vector) = (0.0,0.0,0.0,0.0)
        _PipeEnd("pipe end", Vector) = (0.0,0.0,0.0,0.0)
        _PreviousEnd("pipe end previous", Vector) = (0.0,0.0,0.0,0.0)
        _Count("timer", float) = 0.0
        _PipeLength("length", float) = 0.0
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _PipeStart;
            float4 _PipeEnd;
            float4 _PreviousEnd;

            float _Count;
            float _PipeLength;

            v2f vert (appdata v)
            {
                v2f o;

                float mag = v.vertex.z;
                v.vertex *= mag;
                float4 start = _PipeStart;
                float4 pipeEnd = _PipeEnd;
                float4 prevEnd = _PreviousEnd;

                pipeEnd.y = _PipeEnd.y-1.5;
                prevEnd.y = _PreviousEnd.y-1.5;
                float startZ = v.vertex.z;

                float AdjustedPipeLength = _PipeLength*mag;

                float sway = v.vertex.z / AdjustedPipeLength;
                o.uv.z = sway;
                float4 end = (pipeEnd * (1.0-sway) + prevEnd * (sway));

                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);

                float adjustedX = (length(start.xz-end.xz)) / (AdjustedPipeLength);
                v.vertex.z *= adjustedX;

                float z = length(v.vertex.z);
                float endZ = length(start.z-end.z);
                float a =  (end.y) / (endZ*endZ);
                float y = z * z * a;
                v.vertex.y += y;

               //float endX = (_PipeEnd.z-end.z);
               //v.vertex.x -= endX*sway;
               //float endZ2 = (_PipeEnd.x-end.x);
               //v.vertex.z -= endZ2*sway;

                v.vertex.x += sin(_Count/2 * v.vertex.y)/100;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col * i.uv.z;
            }
            ENDCG
        }
    }
}
