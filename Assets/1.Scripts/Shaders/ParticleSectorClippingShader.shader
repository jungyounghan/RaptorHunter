Shader "Custom/ParticleSectorClippingShader"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)

        _CenterX ("Center X", float) = 0.0
        _CenterY ("Center Y", float) = 0.0
        _Degree ("Rotation Degree", float) = 0.0
        _Angle ("Clipping Angle", Range(0, 360)) = 180 // Ŭ���� ��� ����
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend One One   // Additive Blending ����
        ZWrite Off      // ���� ���� ��Ȱ��ȭ (������ ȿ��)
        Cull Off        // ��� ������

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
            float _Degree;   // ȸ�� ����
            float _Angle;    // ��� �þ߰�

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // ���� ��ǥ ��ȯ
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = i.worldPos;
                float3 centerPos = float3(_CenterX, 0, _CenterY);

                // �߽������� ���� ��ƼŬ ��ġ������ ���� ����
                float3 viewVec = normalize(worldPos - centerPos);

                 viewVec.y = 0; // Y�� �����Ͽ� ���� ���⸸ ��
                viewVec = normalize(viewVec); // �ٽ� ����ȭ

                // _Degree ���� ������ ��ȯ (����)
                float rad = radians(_Degree);
    
                // ȸ�� ��ȯ�� ���� ���� ���
                float3 clipDir = float3(cos(rad), 0, sin(rad));

                float angle = acos(dot(viewVec, clipDir)) * (360.0 / 3.14159265); // ���� ��ȯ
                if (angle > _Angle) // �þ߰� ����� ����
                {
                    discard;
                }

                // �ؽ�ó ���ø� �� �÷� ����
                fixed4 texColor = tex2D(_MainTex, i.uv);
                texColor.rgb *= texColor.a; // ���İ��� RGB�� ���ؼ� ���� ����

                return texColor * i.color;
            }
            ENDCG
        }
    }
}