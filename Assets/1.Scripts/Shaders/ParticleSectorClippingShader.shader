Shader "Custom/ParticleSectorClippingShader"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)

        _CenterX ("Center X", float) = 0.0
        _CenterY ("Center Y", float) = 0.0
        _Degree ("Rotation Degree", float) = 0.0
        _Angle ("Clipping Angle", Range(0, 360)) = 180 // 클리핑 허용 각도
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend One One   // Additive Blending 유지
        ZWrite Off      // 깊이 버퍼 비활성화 (반투명 효과)
        Cull Off        // 양면 렌더링

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Color;

             float _CenterX;
            float _CenterY;
            float _Degree;   // 회전 각도
            float _Angle;    // 허용 시야각

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // 월드 좌표 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = i.worldPos;
                float3 centerPos = float3(_CenterX, 0, _CenterY);

                // 중심점에서 현재 파티클 위치까지의 방향 벡터
                float3 viewVec = normalize(worldPos - centerPos);

                 viewVec.y = 0; // Y축 제거하여 수평 방향만 비교
                viewVec = normalize(viewVec); // 다시 정규화

                // _Degree 값을 각도로 변환 (라디안)
                float rad = radians(_Degree);
    
                // 회전 변환된 방향 벡터 계산
                float3 clipDir = float3(cos(rad), 0, sin(rad));

                float angle = acos(dot(viewVec, clipDir)) * (360.0 / 3.14159265); // 각도 변환
                if (angle > _Angle) // 시야각 벗어나면 제거
                {
                    discard;
                }

                // 텍스처 샘플링 및 컬러 적용
                fixed4 texColor = tex2D(_MainTex, i.uv);
                texColor.rgb *= texColor.a; // 알파값을 RGB에 곱해서 투명도 적용

                return texColor * i.color;
            }
            ENDCG
        }
    }
}