// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyWorld/Ground"
{
    Properties
    {
        _Color ("Color", Color) = (0.095, 0.153, 0.170, 1)
    }

    SubShader
    {
        Tags {"RenderType"="Opaque"}

        Pass {
            Tags { "LightMode"="ForwardBase" }
            
            Cull Off
            
            CGPROGRAM  

            #pragma multi_compile_fwdbase	

            #pragma vertex vert 
            #pragma fragment frag
            
            #include "UnityCG.cginc" 
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
            
            fixed4 _Color;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                // float3 normal : TEXCOORD0;
                float4 col : COLOR;
				SHADOW_COORDS(0)
            };
            
            v2f vert(a2v v) {
                v2f o;
                float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);                
                fixed3 diffuse = _Color.rgb * fixed3(1.0, 1.0, 1.0) * saturate(dot(worldNormal, worldLightDir)); // todo: light
                o.pos = UnityObjectToClipPos(v.vertex);
                o.col = fixed4(diffuse, 1.0);
                
			 	TRANSFER_SHADOW(o);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {	

				fixed shadow = SHADOW_ATTENUATION(i);		

                return fixed4(i.col.rgb * shadow, 1.0);
            } 
            
            ENDCG
        }
    }
    FallBack "VertexLit"
}
