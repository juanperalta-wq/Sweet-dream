Shader "Custom/URP/FlashLit"
{
    Properties
    {
        [Header(Base)]
        _ColorBase ("Color Base", Color) = (1,1,1,1)
        _Specular ("Specular", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [Header(Flash)]
        [HDR] _ColorHDRFlash ("Color HDR Flash", Color) = (1,1,1,1)
        [HDR] _ColorHDRFlash2 ("Color HDR Flash 2", Color) = (1,1,1,1)
        _Flash ("Flash (Multiplicador HDR)", Float) = 1.0
        _FrecuenciaFlash ("Frecuencia de Flash (Hz) - velocidad del ciclo completo", Float) = 2.0
        _TiempoDuracionFlash ("Tiempo que se sostiene cada color antes de mezclar (s)", Float) = 0.1
        _IntensidadFlash ("Intensidad Flash (0 = Base, 1 = Interpolacion 3 colores)", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Soporte básico de luz principal y luces adicionales
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ColorBase;
                half4 _ColorHDRFlash;
                half4 _ColorHDRFlash2;
                float _Specular;
                float _Metallic;
                float _Flash;
                float _FrecuenciaFlash;
                float _TiempoDuracionFlash;
                float _IntensidadFlash;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float fogCoord    : TEXCOORD2;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = vertexInput.positionCS;
                OUT.positionWS = vertexInput.positionWS;
                OUT.normalWS = normalInput.normalWS;
                OUT.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(IN.positionWS));

                // ---- Luz principal ----
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 diffuse = _ColorBase.rgb * mainLight.color * NdotL;

                // ---- Especular tipo Blinn-Phong ----
                float3 halfDir = normalize(mainLight.direction + viewDirWS);
                half NdotH = saturate(dot(normalWS, halfDir));
                half specExponent = lerp(8.0h, 128.0h, _Specular);
                half specTerm = pow(NdotH, specExponent) * _Specular;

                // Metallic actua como tinte del especular (workflow tipo Standard)
                half3 specTint = lerp(half3(0.04h, 0.04h, 0.04h), _ColorBase.rgb, _Metallic);
                half3 specular = specTint * specTerm * mainLight.color;

                // ---- Luces adicionales (point/spot) ----
                half3 additionalLightsColor = half3(0,0,0);
                #ifdef _ADDITIONAL_LIGHTS
                    int additionalLightsCount = GetAdditionalLightsCount();
                    for (int i = 0; i < additionalLightsCount; ++i)
                    {
                        Light light = GetAdditionalLight(i, IN.positionWS);
                        half addNdotL = saturate(dot(normalWS, light.direction));
                        additionalLightsColor += _ColorBase.rgb * light.color * addNdotL * light.distanceAttenuation;
                    }
                #endif

                // ---- Luz ambiental (Spherical Harmonics) ----
                half3 ambient = SampleSH(normalWS) * _ColorBase.rgb;

                // Piso mínimo de ambiente: evita que el material se vea negro
                // cuando la escena no tiene luces/ambiente configurado
                half3 ambientFloor = _ColorBase.rgb * 0.05h;
                ambient = max(ambient, ambientFloor);

                half3 litColor = diffuse + specular + ambient;

                half3 finalColor;

                // Si IntensidadFlash = 0 -> dibuja directamente el color base (con specular y metallic)
                if (_IntensidadFlash <= 0.0001h)
                {
                    finalColor = litColor;
                }
                else
                {
                    // ---- Interpolación cíclica entre 3 colores (ColorBase -> Flash1 -> Flash2 -> ColorBase...) ----
                    float periodoTotal = 1.0 / max(_FrecuenciaFlash, 0.0001);
                    float duracionSegmento = periodoTotal / 3.0;
                    float tiempoEnCiclo = fmod(_Time.y, periodoTotal);

                    int segmento = clamp((int)floor(tiempoEnCiclo / duracionSegmento), 0, 2);
                    float tiempoEnSegmento = tiempoEnCiclo - segmento * duracionSegmento;

                    float tiempoSostenido = clamp(_TiempoDuracionFlash, 0.0, duracionSegmento);
                    float tiempoTransicion = max(duracionSegmento - tiempoSostenido, 0.0001);

                    // 0 mientras se sostiene el color actual, luego se mueve de 0 a 1 durante la transición
                    float blendT = saturate((tiempoEnSegmento - tiempoSostenido) / tiempoTransicion);
                    blendT = smoothstep(0.0, 1.0, blendT);

                    half3 colorNodoA, colorNodoB;
                    if (segmento == 0)
                    {
                        colorNodoA = _ColorBase.rgb;
                        colorNodoB = _ColorHDRFlash.rgb * _Flash;
                    }
                    else if (segmento == 1)
                    {
                        colorNodoA = _ColorHDRFlash.rgb * _Flash;
                        colorNodoB = _ColorHDRFlash2.rgb * _Flash;
                    }
                    else
                    {
                        colorNodoA = _ColorHDRFlash2.rgb * _Flash;
                        colorNodoB = _ColorBase.rgb;
                    }

                    half3 flashColor = lerp(colorNodoA, colorNodoB, blendT);

                    // Si IntensidadFlash = 1 -> interpola entre los 3 colores en bucle
                    finalColor = lerp(litColor, flashColor, saturate(_IntensidadFlash));
                }

                finalColor = MixFog(finalColor, IN.fogCoord);

                return half4(finalColor, _ColorBase.a);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
