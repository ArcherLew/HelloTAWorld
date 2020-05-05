Shader "MyWorld/Mushroom Crown"
{
    Properties
    {
        _UpperColor ("Upper Color", Color) = (0.137, 0.235, 0.541, 1)
        _LowerColor ("Lower Color", Color) = (0.522, 0.153, 0.753, 1)
        _GlimOffset ("Glim Offset", Float) = 0.0
        _DarkPeriod ("Dark Period", Float) = 0.95
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
            
            fixed4 _UpperColor;
            fixed4 _LowerColor;
            float _GlimOffset;
            float _DarkPeriod;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 vertex : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float glimmer : TEXCOORD2;
            };
            
            v2f vert(a2v v) {
                v2f o;
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);                
                o.vertex = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                // 蘑菇腹部按各自频率自发光
                float t = sin((_Time.y / 10) + _GlimOffset);
                o.glimmer = step(_DarkPeriod, t) * (t - _DarkPeriod) * (1 / (1 - _DarkPeriod));
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // 蘑菇背面颜色和光照有关			
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);                
                float lambert = saturate(dot(worldNormal, worldLightDir));

                float f = step(i.vertex.y, 0);                
                
                fixed4 col = f * _LowerColor * i.glimmer * (0.5 * lambert + 0.5) + (1 - f) * _UpperColor * lambert;
                return col;
            } 
            
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
