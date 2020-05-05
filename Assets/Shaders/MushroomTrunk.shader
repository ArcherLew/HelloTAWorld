Shader "MyWorld/Mushroom Trunk"
{
    Properties
    {
        _Color ("Color", Color) = (0.133, 0.15, 0.2, 1)
    }

    SubShader
    {
        Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}

        Pass {
            Tags { "LightMode"="ForwardBase" }
            
            Cull Off
            
            CGPROGRAM  
            #pragma vertex vert 
            #pragma fragment frag
            
            #include "UnityCG.cginc" 
            
            fixed4 _Color;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                // float3 normal : TEXCOORD0;
                float4 col : COLOR;
            };
            
            v2f vert(a2v v) {
                v2f o;
                float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);                
                fixed3 diffuse = _Color.rgb * saturate(dot(worldNormal, worldLightDir)); // todo: light
                o.pos = UnityObjectToClipPos(v.vertex);
                o.col = fixed4(diffuse, 1.0);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {			
                return i.col;
            } 
            
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
