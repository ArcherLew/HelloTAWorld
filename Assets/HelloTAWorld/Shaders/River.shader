Shader "MyWorld/River"
{
    Properties
    {
        _Color ("Color", Color) = (0.35, 0.557, 0.851, 1)
        _Magnitude ("Distortion Magnitude", Float) = 1
        _Frequency ("Distortion Frequency", Float) = 2
        _InvWaveLength ("Distortion Inverse Wave Length", Float) = 2
    }

    SubShader
    {
        Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" "DisableBatching"="True"}

        Pass {
            Tags { "LightMode"="ForwardBase" }
            
            Cull Off
            
            CGPROGRAM  
            #pragma vertex vert 
            #pragma fragment frag
            #pragma geometry geom
            
            #include "UnityCG.cginc" 
            
            fixed4 _Color;
            float _Magnitude;
            float _Frequency;
            float _InvWaveLength;
            
            struct a2v {
                float4 vertex : POSITION;
            };

            struct v2g {
                float4 vertex : POSITION;
            };
            
            struct g2f {
                float4 pos : SV_POSITION;
                // float3 normal : TEXCOORD0;
                float4 col : COLOR;
            };
            
            v2g vert(a2v v) {
                v2g o;
                
                float4 offset;
                offset.xzw = float3(0.0, 0.0, 0.0);
                // lower v.vertex.y is 0 or -10
                offset.y = step(-9, v.vertex.y) * sin(_Frequency * _Time.y + v.vertex.x * _InvWaveLength + v.vertex.y * _InvWaveLength + v.vertex.z * _InvWaveLength) * _Magnitude;
                o.vertex = v.vertex + offset;
                
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> tristream) {
                g2f o;
                float3 edgeA = IN[1].vertex.xyz - IN[0].vertex.xyz;
                float3 edgeB = IN[2].vertex.xyz - IN[0].vertex.xyz;
                float3 worldNormal = normalize(cross(edgeA, edgeB));

                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _Color.rgb * fixed3(1.0, 1.0, 1.0) * saturate(dot(worldNormal, worldLightDir)); // todo: _LightColor0.rgb

                for (uint i = 0; i < 3; i++)
                {
                    o.pos = UnityObjectToClipPos(IN[i].vertex);
                    o.col = fixed4(diffuse, 1.0);
                    tristream.Append(o);
                }
                tristream.RestartStrip();
            }

            fixed4 frag(g2f i) : SV_Target {			
                return i.col;
            } 
            
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
