Shader "Dungeon Xpress/Cutout Shader"
{
    Properties
    {
        [NoScaleOffset]_MainTexture("Main Texture", 2D) = "white" {}
        _Tint("Tint", Color) = (1, 1, 1, 0)
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            float2 lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 sh;
            #endif
            float4 fogFactorAndVertexLight;
            float4 shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if defined(LIGHTMAP_ON)
            float2 interp5 : TEXCOORD5;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 interp6 : TEXCOORD6;
            #endif
            float4 interp7 : TEXCOORD7;
            float4 interp8 : TEXCOORD8;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp6.xyz =  input.sh;
            #endif
            output.interp7.xyzw =  input.fogFactorAndVertexLight;
            output.interp8.xyzw =  input.shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.lightmapUV = input.interp5.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp6.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp7.xyzw;
            output.shadowCoord = input.interp8.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTexture);
            float4 _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.tex, _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_R_4 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.r;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_G_5 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.g;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_B_6 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.b;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_A_7 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.a;
            float4 _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0 = _Tint;
            float4 _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2;
            Unity_Multiply_float(_SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0, _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0, _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2);
            float _Property_1b2a10f515414d899bf1d2095677c30e_Out_0 = _Metallic;
            float _Property_ddcecb940d5148b09dd5197a58dd3645_Out_0 = _Smoothness;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.BaseColor = (_Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2.xyz);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = _Property_1b2a10f515414d899bf1d2095677c30e_Out_0;
            surface.Smoothness = _Property_ddcecb940d5148b09dd5197a58dd3645_Out_0;
            surface.Occlusion = 1;
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
        #pragma multi_compile _ _GBUFFER_NORMALS_OCT
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_GBUFFER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            float2 lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 sh;
            #endif
            float4 fogFactorAndVertexLight;
            float4 shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if defined(LIGHTMAP_ON)
            float2 interp5 : TEXCOORD5;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 interp6 : TEXCOORD6;
            #endif
            float4 interp7 : TEXCOORD7;
            float4 interp8 : TEXCOORD8;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp6.xyz =  input.sh;
            #endif
            output.interp7.xyzw =  input.fogFactorAndVertexLight;
            output.interp8.xyzw =  input.shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.lightmapUV = input.interp5.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp6.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp7.xyzw;
            output.shadowCoord = input.interp8.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTexture);
            float4 _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.tex, _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_R_4 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.r;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_G_5 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.g;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_B_6 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.b;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_A_7 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.a;
            float4 _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0 = _Tint;
            float4 _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2;
            Unity_Multiply_float(_SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0, _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0, _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2);
            float _Property_1b2a10f515414d899bf1d2095677c30e_Out_0 = _Metallic;
            float _Property_ddcecb940d5148b09dd5197a58dd3645_Out_0 = _Smoothness;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.BaseColor = (_Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2.xyz);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = _Property_1b2a10f515414d899bf1d2095677c30e_Out_0;
            surface.Smoothness = _Property_ddcecb940d5148b09dd5197a58dd3645_Out_0;
            surface.Occlusion = 1;
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_META
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTexture);
            float4 _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.tex, _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_R_4 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.r;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_G_5 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.g;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_B_6 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.b;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_A_7 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.a;
            float4 _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0 = _Tint;
            float4 _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2;
            Unity_Multiply_float(_SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0, _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0, _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2);
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.BaseColor = (_Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2.xyz);
            surface.Emission = float3(0, 0, 0);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_2D
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTexture);
            float4 _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.tex, _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_R_4 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.r;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_G_5 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.g;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_B_6 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.b;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_A_7 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.a;
            float4 _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0 = _Tint;
            float4 _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2;
            Unity_Multiply_float(_SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0, _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0, _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2);
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.BaseColor = (_Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2.xyz);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

            ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            float2 lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 sh;
            #endif
            float4 fogFactorAndVertexLight;
            float4 shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if defined(LIGHTMAP_ON)
            float2 interp5 : TEXCOORD5;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 interp6 : TEXCOORD6;
            #endif
            float4 interp7 : TEXCOORD7;
            float4 interp8 : TEXCOORD8;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp6.xyz =  input.sh;
            #endif
            output.interp7.xyzw =  input.fogFactorAndVertexLight;
            output.interp8.xyzw =  input.shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.lightmapUV = input.interp5.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp6.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp7.xyzw;
            output.shadowCoord = input.interp8.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTexture);
            float4 _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.tex, _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_R_4 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.r;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_G_5 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.g;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_B_6 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.b;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_A_7 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.a;
            float4 _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0 = _Tint;
            float4 _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2;
            Unity_Multiply_float(_SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0, _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0, _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2);
            float _Property_1b2a10f515414d899bf1d2095677c30e_Out_0 = _Metallic;
            float _Property_ddcecb940d5148b09dd5197a58dd3645_Out_0 = _Smoothness;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.BaseColor = (_Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2.xyz);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = _Property_1b2a10f515414d899bf1d2095677c30e_Out_0;
            surface.Smoothness = _Property_ddcecb940d5148b09dd5197a58dd3645_Out_0;
            surface.Occlusion = 1;
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_META
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTexture);
            float4 _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.tex, _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_R_4 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.r;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_G_5 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.g;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_B_6 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.b;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_A_7 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.a;
            float4 _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0 = _Tint;
            float4 _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2;
            Unity_Multiply_float(_SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0, _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0, _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2);
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.BaseColor = (_Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2.xyz);
            surface.Emission = float3(0, 0, 0);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_2D
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpacePosition;
            float4 ScreenPosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTexture_TexelSize;
        float4 _Tint;
        float _Smoothness;
        float _Metallic;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTexture);
        SAMPLER(sampler_MainTexture);
        float2 _ScreenPosition;
        float _CutoutSize;
        float _CutoutSmoothness;
        float _CutoutOpacity;
        float _CutoutNoiseScale;

            // Graph Functions
            
        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTexture);
            float4 _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.tex, _Property_7928f31b2b6b4bdeac5beeeaf6ad2213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_R_4 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.r;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_G_5 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.g;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_B_6 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.b;
            float _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_A_7 = _SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0.a;
            float4 _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0 = _Tint;
            float4 _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2;
            Unity_Multiply_float(_SampleTexture2D_c8024d1556f74cf3bbb9ba39865d6599_RGBA_0, _Property_224b9ee7e2d74f7989365a85da2ad154_Out_0, _Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2);
            float _Property_06ef17f976644c878a5f63afade2bbf8_Out_0 = _CutoutOpacity;
            float _Property_a8e0909a72f440a6ad0468080a715f94_Out_0 = _CutoutSmoothness;
            float4 _ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float2 _Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0 = _ScreenPosition;
            float2 _Remap_429ea9ebe43f48d293b93b3053661460_Out_3;
            Unity_Remap_float2(_Property_e6f74b3f799f4e779af1a75f66f598f7_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3);
            float2 _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2;
            Unity_Add_float2((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), _Remap_429ea9ebe43f48d293b93b3053661460_Out_3, _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2);
            float2 _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3;
            Unity_TilingAndOffset_float((_ScreenPosition_8c57e5478c3e4bda9b7707d1f58d7e98_Out_0.xy), float2 (1, 1), _Add_7428d99bdcfd490f9c6262b90d4123ba_Out_2, _TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3);
            float2 _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2;
            Unity_Multiply_float(_TilingAndOffset_7c8330afae934278b8aa8208cabbea0d_Out_3, float2(2, 2), _Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2);
            float2 _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2;
            Unity_Subtract_float2(_Multiply_bb124da9c9a54d839e9f37cdc8a2c0bc_Out_2, float2(1, 1), _Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2);
            float _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2);
            float _Property_17ce2b36148d497b91af9676534e22c2_Out_0 = _CutoutSize;
            float _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2;
            Unity_Multiply_float(_Divide_9be94361fa9a47fc90b5a47b8b12818c_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0, _Multiply_d561018314624fc6a1b019d30a256fe1_Out_2);
            float2 _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0 = float2(_Multiply_d561018314624fc6a1b019d30a256fe1_Out_2, _Property_17ce2b36148d497b91af9676534e22c2_Out_0);
            float2 _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2;
            Unity_Divide_float2(_Subtract_e94594876d09430e9ece9d63c1b29edc_Out_2, _Vector2_22f3e6372dcc467ba9912143f0d45586_Out_0, _Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2);
            float _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1;
            Unity_Length_float2(_Divide_ce276912bea8401c94c0186cf2aea0f7_Out_2, _Length_468672a7c77e4c26a84c46e5877cbd93_Out_1);
            float _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1;
            Unity_OneMinus_float(_Length_468672a7c77e4c26a84c46e5877cbd93_Out_1, _OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1);
            float _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1;
            Unity_Saturate_float(_OneMinus_f69d9ae2cb9d403db80d539662216736_Out_1, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1);
            float _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3;
            Unity_Smoothstep_float(0, _Property_a8e0909a72f440a6ad0468080a715f94_Out_0, _Saturate_a3a5bd31c7514d35bfaba769adb37e0e_Out_1, _Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3);
            float _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0 = _CutoutNoiseScale;
            float _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_846c1e594de1455d8a9fcdcdc4807a66_Out_0, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2);
            float _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2;
            Unity_Multiply_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _GradientNoise_e10d257faf5d4e1aa8cac77a700021f4_Out_2, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2);
            float _Add_266e1201e9a446b192a14bd9c5690483_Out_2;
            Unity_Add_float(_Smoothstep_2d1df055c2b0440a9b49b62587a0d0e9_Out_3, _Multiply_5967a03ec1774c26b8618c467dd740fd_Out_2, _Add_266e1201e9a446b192a14bd9c5690483_Out_2);
            float _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2;
            Unity_Multiply_float(_Property_06ef17f976644c878a5f63afade2bbf8_Out_0, _Add_266e1201e9a446b192a14bd9c5690483_Out_2, _Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2);
            float _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3;
            Unity_Clamp_float(_Multiply_9668ed52001c42d582e8aa19e2d69986_Out_2, 0, 1, _Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3);
            float _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            Unity_OneMinus_float(_Clamp_a0c08a4dbc164c77bc88dda70efbab0a_Out_3, _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1);
            surface.BaseColor = (_Multiply_1cfb840c49dd422dad21cf9cd6a3357c_Out_2.xyz);
            surface.Alpha = _OneMinus_0a88194bf04e47dab6cee238b7e6ea0f_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

            ENDHLSL
        }
    }
    CustomEditor "ShaderGraph.PBRMasterGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}