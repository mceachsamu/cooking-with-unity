#if !defined(MY_SHADOWS_INCLUDED)
#define MY_SHADOWS_INCLUDED

#include "UnityCG.cginc"

struct VertexData {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float3 uv : TEXCOORD0;
};

sampler2D _Tex;
float4 _Tex_ST;

float _MaxHeight;

#if defined(SHADOWS_CUBE)
	struct Interpolators {
		float4 position : SV_POSITION;
		float3 lightVec : TEXCOORD0;
	};

	Interpolators MyShadowVertexProgram (VertexData v) {
		Interpolators i;
		i.position = UnityObjectToClipPos(v.position);
		//i.uv = TRANSFORM_TEX(v.uv, _Tex);
		#if !defined(SHADER_API_OPENGL)
			float4 height = tex2Dlod (_Tex, float4(float2(v.uv.x,v.uv.y),0,0));
			i.position.y += height.r - _MaxHeight;
		#endif
		i.lightVec = mul(unity_ObjectToWorld, i.position).xyz - _LightPositionRange.xyz;
		return i;
	}

	float4 MyShadowFragmentProgram (Interpolators i) : SV_TARGET {
		float depth = length(i.lightVec) + unity_LightShadowBias.x;
		depth *= _LightPositionRange.w;
		return UnityEncodeCubeShadowDepth(depth);
	}
#else
	float4 MyShadowVertexProgram (VertexData v) : SV_POSITION {
		float4 position =
			UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
		return UnityApplyLinearShadowBias(position);
	}

	half4 MyShadowFragmentProgram () : SV_TARGET {
		return 0;
	}
#endif

#endif