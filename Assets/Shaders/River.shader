// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyWorld/River"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0.66, 0.9, 1)
        _Magnitude ("Distortion Magnitude", Float) = 2
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
                // v.vertex.y is 0 or -10
                offset.y = step(-5, v.vertex.y) * sin(_Frequency * _Time.y + v.vertex.x * _InvWaveLength + v.vertex.y * _InvWaveLength + v.vertex.z * _InvWaveLength) * _Magnitude;
                o.vertex = v.vertex + offset;
                // o.pos = UnityObjectToClipPos(v.vertex + offset);
                
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> tristream) {
                g2f o;
                //用两条边算出法线方向
                float3 edgeA = IN[1].vertex.xyz - IN[0].vertex.xyz;
                float3 edgeB = IN[2].vertex.xyz - IN[0].vertex.xyz;
                float3 worldNormal = normalize(cross(edgeB, edgeA)); // todo: ABBA

                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _Color.rgb * fixed3(1.0, 1.0, 1.0) * saturate(dot(worldNormal, worldLightDir)); // todo: _LightColor0.rgb

                // //三角面中心点
                // float3 centerPos = (IN[0].vertex + IN[1].vertex + IN[2].vertex) / 3;
                // //中心点uv位置
                // float2 centerTex = (IN[0].uv + IN[1].uv + IN[2].uv) / 3;
                // //外拓的顶点距离
                // centerPos += float4(worldNormal, 0)*_Length;

                for (uint i = 0; i < 3; i++)
                {
                    o.pos = UnityObjectToClipPos(IN[i].vertex);
                    o.col = fixed4(diffuse, 1.0);
                    // o.col = fixed4(0., 0., 0., 1.);
                    
                    //添加顶点
                    tristream.Append(o);

                    // uint index = (i + 1) % 3;
                    // o.vertex = UnityObjectToClipPos(IN[index].vertex);
                    // o.uv = IN[index].uv;
                    // o.col = fixed4(0., 0., 0., 1.);

                    // tristream.Append(o);

                    // //外部颜色白
                    // o.vertex = UnityObjectToClipPos(float4(centerPos, 1));
                    // o.uv = centerTex;
                    // o.col = fixed4(1.0, 1.0, 1.0, 1.);

                    // tristream.Append(o);
                    //添加三角面
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
