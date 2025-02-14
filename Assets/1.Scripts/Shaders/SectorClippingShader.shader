Shader "Custom/SectorClippingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Strength", float) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Brightness ("Brightness", Range(0,1)) = 1.0

        _CenterX ("Center X", float) = 0.0
        _CenterY ("Center Y", float) = 0.0
        _Degree ("Degree", float) = 0.0
        _Angle ("Angle", Range(0, 360)) = 360
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        float _BumpScale;
        float _Metallic;
        float _Smoothness;
        float _Brightness;

        float _CenterX;
        float _CenterY;
        float _Degree;
        float _Angle;

       void surf (Input IN, inout SurfaceOutputStandard o)
       {
            // 시야각 기반 클리핑 적용 (위/아래는 보이고, 좌우만 제한)
            float3 viewVec = normalize(IN.worldPos - float3(_CenterX, 0, _CenterY));
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

            // 텍스처 색상 가져오기
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            c.rgb *= _Brightness;

            // 노말맵 적용 (강도 조절)
            float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            normal.xy *= _BumpScale;
            normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
            o.Normal = normal;

            // 최종 색상 적용
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }

    FallBack "Diffuse"
}