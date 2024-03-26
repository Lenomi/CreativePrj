Shader "Hidden/PaintIn3D/CwDilate"
{
	Properties
	{
	}
	SubShader
	{
		Cull Off

		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag

				float4 _CwCoord;

				struct a2v
				{
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
					float2 texcoord2 : TEXCOORD2;
					float2 texcoord3 : TEXCOORD3;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};

				void Vert(a2v i, out v2f o)
				{
					float2 texcoord = i.texcoord0 * _CwCoord.x + i.texcoord1 * _CwCoord.y + i.texcoord2 * _CwCoord.z + i.texcoord3 * _CwCoord.w;
					o.vertex = float4(texcoord.xy * 2.0f - 1.0f, 0.5f, 1.0f);
#if UNITY_UV_STARTS_AT_TOP
					o.vertex.y = -o.vertex.y;
#endif
				}

				float4 Frag(v2f i) : SV_TARGET
				{
					return float4(1.0f, 1.0f, 1.0f, 1.0f);
				}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag

				#include "../../../PaintCore/Required/Resources/CwShared.cginc"

				sampler2D _CwTexure;
				int       _CwSteps;
				float2    _CwSize;
				float4    _CwOffsets[9];

				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv     : TEXCOORD0;
				};

				void Vert(a2v i, out v2f o)
				{
					o.vertex = UnityObjectToClipPos(i.vertex);
					o.uv     = i.texcoord0;
				}

				float4 Frag(v2f i) : SV_TARGET
				{
					float2 uv = CW_SnapToPixel(i.uv, _CwSize);

					if (tex2D(_CwTexure, uv).x < 1.0f)
					{
						for (int s = 1; s <= 32; s++)
						{
							for (int j = 1; j <= 8; j++)
							{
								if (tex2Dlod(_CwTexure, float4(uv + _CwOffsets[j].xy * s, 0, 0)).x >= 1.0f)
								{
									return float4(j, s, 0.0f, 0.0f) / 255.0f;
								}
							}
						}
					}

					return float4(0.0f, 0.0f, 0.0f, 0.0f);
				}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag

				#include "../../../PaintCore/Required/Resources/CwShared.cginc"
				
				sampler2D _CwTexure;
				sampler2D _CwLookup;
				float2    _CwSize;
				float4    _CwOffsets[9];

				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv     : TEXCOORD0;
				};

				void Vert(a2v i, out v2f o)
				{
					o.vertex = UnityObjectToClipPos(i.vertex);
					o.uv     = i.texcoord0;
				}

				float4 Frag(v2f i) : SV_TARGET
				{
					float2 uv     = CW_SnapToPixel(i.uv, _CwSize);
					float4 lookup = tex2D(_CwLookup, uv) * 255.0f;

					return tex2D(_CwTexure, uv + _CwOffsets[lookup.x].xy * lookup.y);
				}
			ENDCG
		}
	}
}
