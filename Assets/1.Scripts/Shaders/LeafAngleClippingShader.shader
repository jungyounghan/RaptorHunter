Shader "Custom/LeafAngleClippingShader" {
 
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    _ShakeDisplacement ("Displacement", Range (0, 1.0)) = 1.0
    _ShakeTime ("Shake Time", Range (0, 1.0)) = 1.0
    _ShakeWindspeed ("Shake Windspeed", Range (0, 1.0)) = 1.0
    _ShakeBending ("Shake Bending", Range (0, 1.0)) = 1.0

    _CenterX ("Center X", float) = 0.0
    _CenterY ("Center Y", float) = 0.0
    _Direction ("Direction", float) = 0.0
    _ViewAngle ("View Angle", Range(0, 180)) = 180
}
 
SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200
    Cull Off
   
CGPROGRAM
#pragma target 3.0
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert addshadow
 
sampler2D _MainTex;
fixed4 _Color;
float _ShakeDisplacement;
float _ShakeTime;
float _ShakeWindspeed;
float _ShakeBending;
 
     struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };
 
void FastSinCos (float4 val, out float4 s, out float4 c) {
    val = val * 6.408849 - 3.1415927;
    float4 r5 = val * val;
    float4 r6 = r5 * r5;
    float4 r7 = r6 * r5;
    float4 r8 = r6 * r5;
    float4 r1 = r5 * val;
    float4 r2 = r1 * r5;
    float4 r3 = r2 * r5;
    float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841} ;
    float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587} ;
    s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
    c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
}
 
 
void vert (inout appdata_full v) {
   
    float factor = (1 - _ShakeDisplacement -  v.color.r) * 0.5;
       
    const float _WindSpeed  = (_ShakeWindspeed  +  v.color.g );    
    const float _WaveScale = _ShakeDisplacement;
   
    const float4 _waveXSize = float4(0.048, 0.06, 0.24, 0.096);
    const float4 _waveZSize = float4 (0.024, .08, 0.08, 0.2);
    const float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);
 
    float4 _waveXmove = float4(0.024, 0.04, -0.12, 0.096);
    float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);
   
    float4 waves;
    waves = v.vertex.x * _waveXSize;
    waves += v.vertex.z * _waveZSize;
 
    waves += _Time.x * (1 - _ShakeTime * 2 - v.color.b ) * waveSpeed *_WindSpeed;
 
    float4 s, c;
    waves = frac (waves);
    FastSinCos (waves, s,c);
 
    float waveAmount = v.texcoord.y * (v.color.a + _ShakeBending);
    s *= waveAmount;
 
    s *= normalize (waveSpeed);
 
    s = s * s;
    float fade = dot (s, 1.3);
    s = s * s;
    float3 waveMove = float3 (0,0,0);
    waveMove.x = dot (s, _waveXmove);
    waveMove.z = dot (s, _waveZmove);
    v.vertex.xz -= mul ((float3x3)unity_WorldToObject, waveMove).xz;
   
}
 
       
        float _CenterX;
        float _CenterY;
        float _Direction;
        float _ViewAngle;

        void surf (Input IN, inout SurfaceOutput o) 
        {

             // 시야각 기반 클리핑 적용 (위/아래는 보이고, 좌우만 제한)
            float3 viewVec = normalize(IN.worldPos - float3(_CenterX, 0, _CenterY));
            viewVec.y = 0; // Y축 제거하여 수평 방향만 비교
            viewVec = normalize(viewVec); // 다시 정규화

            // _Direction 값을 각도로 변환 (라디안)
            float rad = radians(_Direction);
    
            // 회전 변환된 방향 벡터 계산
            float3 clipDir = float3(cos(rad), 0, sin(rad));

            float angle = acos(dot(viewVec, clipDir)) * (180.0 / 3.14159265); // 각도 변환
            if (angle > _ViewAngle) // 시야각 벗어나면 제거
            {
                discard;
            }


        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = c.rgb;
        o.Alpha = c.a;
    }
    ENDCG
    }
 
Fallback "Transparent/Cutout/VertexLit"
}