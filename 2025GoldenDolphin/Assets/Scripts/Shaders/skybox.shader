Shader "Unlit/skybox"
{
    Properties
    {
        // 顶部区域颜色
        _TopColor ("Top Color", Color) = (0.1, 0.2, 0.5, 1) // 默认深蓝色（模拟高空）
        // 中间区域颜色
        _MidColor ("Middle Color", Color) = (0.5, 0.7, 0.9, 1) // 默认浅蓝色（模拟天空中层）
        // 底部区域颜色
        _BotColor ("Bottom Color", Color) = (0.8, 0.9, 1, 1) // 默认淡蓝色（模拟低空）
        
        // 顶部→中间 过渡位置（0-1，值越小过渡越靠上）
        _TopToMid ("Top -> Middle Transition", Range(0,1)) = 0.3 
        // 中间→底部 过渡位置（0-1，值越大过渡越靠下）
        _MidToBot ("Middle -> Bottom Transition", Range(0,1)) = 0.7 
        
        // 过渡柔和度（值越大过渡越平滑，0为硬切分）
        _Smoothness ("Transition Smoothness", Range(0.01, 0.2)) = 0.05 
    }

    SubShader
    {
        Tags { "RenderType"="Skybox" "Queue"="Background" }
        LOD 100
        ZWrite Off // 关闭深度写入（Skybox无需深度）
        Cull Off // 关闭背面剔除（天空盒需360度可见）

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 从Properties接收参数
            float4 _TopColor;
            float4 _MidColor;
            float4 _BotColor;
            float _TopToMid;
            float _MidToBot;
            float _Smoothness;

            // 顶点输入结构体（仅需位置信息）
            struct appdata
            {
                float4 vertex : POSITION;
            };

            // 顶点输出结构体（传递天空盒采样所需的视向量）
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD0; // 视向量（用于计算天空盒上下位置）
            };

            // 顶点着色器：将顶点坐标转换为裁剪空间，并传递视向量
            v2f vert (appdata v)
            {
                v2f o;
                // 天空盒特殊处理：用objectToWorld矩阵将模型空间顶点转为世界空间，再作为视向量
                o.viewDir = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
                // 顶点坐标转裁剪空间
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // 片段着色器：计算当前像素的渐变颜色
            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 归一化视向量，提取Y轴值（Y=1为正上方，Y=-1为正下方，映射到0-1范围）
                float3 normalizedViewDir = normalize(i.viewDir);
                float y = (normalizedViewDir.y + 1) * 0.5; // 映射后y∈[0,1]，0=最下方，1=最上方

                // 2. 计算三个区域的过渡范围（用平滑步进实现柔和过渡）
                // 顶部→中间过渡：y > _TopToMid时偏向顶部色，y < _TopToMid时偏向中间色
                float topToMidLerp = smoothstep(_TopToMid - _Smoothness, _TopToMid + _Smoothness, y);
                // 中间→底部过渡：y > _MidToBot时偏向中间色，y < _MidToBot时偏向底部色
                float midToBotLerp = smoothstep(_MidToBot - _Smoothness, _MidToBot + _Smoothness, y);

                // 3. 分区域计算最终颜色
                fixed4 finalColor;
                if (y > _MidToBot)
                {
                    // 顶部区域：混合顶部色与中间色
                    finalColor = lerp(_MidColor, _TopColor, topToMidLerp);
                }
                else if (y < _TopToMid)
                {
                    // 底部区域：混合中间色与底部色
                    finalColor = lerp(_BotColor, _MidColor, midToBotLerp);
                }
                else
                {
                    // 中间区域：直接使用中间色（过渡范围外的纯中间色区）
                    finalColor = _MidColor;
                }

                return finalColor;
            }
            ENDCG
        }
    }
    // 降级Shader（若设备不支持，使用Unity默认天空盒）
    FallBack "Skybox/6 Sided"
}