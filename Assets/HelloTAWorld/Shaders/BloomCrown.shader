Shader "MyWorld/Bloom Crown"
{
    Properties
    {
        _CrownColor ("Crown Color", Color) = (0.137, 0.235, 0.541, 1)
        // _TimeOffset ("Bloom Offset", Float) = 0.0
        // _DarkPeriod ("Dark Period", Float) = 0.95
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        

        Pass {
            Tags { "LightMode"="ForwardBase" }
            
            Cull Off
            
            CGPROGRAM  
            #pragma vertex vert 
            #pragma fragment frag
            
            #include "UnityCG.cginc" 
            
            fixed4 _CrownColor;
            // float _TimeOffset;
            // float _DarkPeriod;
            
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

            struct outMrt
            {
                fixed4 dest0 : SV_Target0;
                fixed dest1 : SV_Target1;
            };

            
            v2f vert(a2v v) {
                v2f o;
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);                
                o.vertex = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                // 蘑菇腹部按各自频率自发光
                // float t = sin((_Time.y / 10) + _TimeOffset);
                // o.glimmer = step(_DarkPeriod, t) * (t - _DarkPeriod) * (1 / (1 - _DarkPeriod));
                o.glimmer = 1;
                
                return o;
            }

            outMrt frag(v2f i) : SV_Target {
                // 蘑菇背面颜色和光照有关			
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);                
                float lambert = saturate(dot(worldNormal, worldLightDir));
                fixed4 col = _CrownColor * i.glimmer * (0.5 * lambert + 0.5);
                
                outMrt mrt0;
                mrt0.dest0 = col;
                mrt0.dest1 = col.a; // todo: Bloom Strength
                return mrt0;
            } 
            
            ENDCG
        }
    }
    FallBack "VertexLit"
}
