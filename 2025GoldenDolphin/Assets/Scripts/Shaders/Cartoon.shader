Shader "Unlit/Cartoon_ShadowSeparate"
{
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _ColorTint ("Color Tint", Color) = (1, 1, 1, 1)  // 纹理整体色调
        
        _Outline ("Outline Width", Range(0, 0.1)) = 0.02  // 轮廓宽度
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)  // 轮廓颜色
        
        // 自身光照三档颜色（step + lerp 梯度）
        _SelfColor1 ("Self Color 1 (Darkest)", Color) = (0.3, 0.3, 0.5, 1)
        _SelfColor2 ("Self Color 2 (Mid)", Color) = (0.6, 0.6, 0.8, 1)
        _SelfColor3 ("Self Color 3 (Lightest)", Color) = (1, 1, 1, 1)
        _SelfThreshold1 ("Self Dark→Mid Threshold", Range(0, 1)) = 0.3
        _SelfThreshold2 ("Self Mid→Light Threshold", Range(0, 1)) = 0.7
        
        // 独立阴影颜色（覆盖在自身颜色上）
        _ShadowColor ("Shadow Color", Color) = (0.1, 0.1, 0.2, 1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        
        // 轮廓线Pass（不变）
        Pass {
            NAME "OUTLINE"
            Cull Front    // 只渲染背面
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float _Outline;
            fixed4 _OutlineColor;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            }; 
            
            struct v2f {
                float4 pos : SV_POSITION;
            };
            
            v2f vert (a2v v) {
                v2f o;
                float4 pos = mul(UNITY_MATRIX_MV, v.vertex); 
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);  
                normal.z = -0.5; // 调整轮廓厚度均匀性
                pos += float4(normalize(normal), 0) * _Outline;
                o.pos = mul(UNITY_MATRIX_P, pos);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target { 
                return float4(_OutlineColor.rgb, 1);               
            }
            ENDCG
        }
        
        // 主渲染Pass（核心修改：自身颜色梯度 + 阴影独立覆盖）
        Pass {
            Tags { "LightMode"="ForwardBase" }
            Cull Back
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase  // 支持阴影
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            // 声明属性
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorTint;
            fixed4 _SelfColor1, _SelfColor2, _SelfColor3;
            fixed _SelfThreshold1, _SelfThreshold2;
            fixed4 _ShadowColor;
            
            // 顶点输入/输出结构
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            }; 
            
            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)  // 阴影坐标
            };
            
            // 顶点着色器（不变）
            v2f vert (a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);  // 传递阴影信息
                return o;
            }
            
            // 片段着色器（核心：自身颜色梯度 + 阴影覆盖）
            float4 frag(v2f i) : SV_Target { 
                // 1. 纹理采样与基础色调
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed3 baseTint = texColor.rgb * _ColorTint.rgb;
                
                // 2. 光照方向与法向夹角（NdotL）
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed ndotl = saturate(dot(worldNormal, worldLightDir)); // 映射到 0~1
                
                // 3. 自身颜色三档梯度（step + lerp）
                fixed3 selfColor;
                // 暗档
                if (ndotl <= _SelfThreshold1) {
                    selfColor = _SelfColor1.rgb;
                }
                // 中档
                else if (ndotl <= _SelfThreshold2) {
                    fixed t = (ndotl - _SelfThreshold1) / (_SelfThreshold2 - _SelfThreshold1);
                    selfColor = lerp(_SelfColor1.rgb, _SelfColor2.rgb, t);
                }
                // 亮档
                else {
                    fixed t = (ndotl - _SelfThreshold2) / (1 - _SelfThreshold2);
                    selfColor = lerp(_SelfColor2.rgb, _SelfColor3.rgb, t);
                }
                selfColor *= baseTint;  // 叠加纹理色调
                
                // 4. 阴影混合（独立阴影色覆盖自身颜色）
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos); // atten: 0（全阴影）~1（无阴影）
                fixed3 finalColor = lerp(_ShadowColor.rgb, selfColor, atten);
                
                // 5. 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * selfColor;
                
                // 6. 最终颜色（环境光 + 阴影混合后颜色）
                return fixed4(ambient + finalColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"  // 降级兼容
}