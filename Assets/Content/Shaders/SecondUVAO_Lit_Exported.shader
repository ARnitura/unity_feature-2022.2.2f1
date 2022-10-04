Shader "SecondUVAO_Lit_Exported"
{
    Properties
    {
        [NoScaleOffset] _MainTex("MainTex", 2D) = "white" {}
        [NoScaleOffset]_BumpMap("BumpMap", 2D) = "white" {}
        [NoScaleOffset]_SpecGlossMap("SpecGlossMap", 2D) = "white" {}
        [NoScaleOffset]_OcclusionMap("OcclusionMap", 2D) = "white" {}
        _OcclusionIntencity("OcclusionIntencity", Float) = 1
        [HideInInspector]_BUILTIN_QueueOffset("Float", Float) = 0
        [HideInInspector]_BUILTIN_QueueControl("Float", Float) = -1
    }
        SubShader
        {
            Tags
            {
                // RenderPipeline: <None>
                "RenderType" = "Opaque"
                "BuiltInMaterialType" = "Lit"
                "Queue" = "Geometry"
                "ShaderGraphShader" = "true"
                "ShaderGraphTargetId" = "BuiltInLitSubTarget"
            }
            Pass
            {
                Name "BuiltIn Forward"
                Tags
                {
                    "LightMode" = "ForwardBase"
                }

            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 3.0
            //#pragma multi_compile_instancing
            //#pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
           // #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
           // #pragma multi_compile _ LIGHTMAP_ON
           // #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
            //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            //#pragma multi_compile _ _SHADOWS_SOFT
            //#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            //#pragma multi_compile _ SHADOWS_SHADOWMASK
            // GraphKeywords: <None>

            // Defines
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
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            #define BUILTIN_TARGET_API 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
            #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
            #endif
            #ifdef _BUILTIN_ALPHATEST_ON
            #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
            #endif
            #ifdef _BUILTIN_AlphaClip
            #define _AlphaClip _BUILTIN_AlphaClip
            #endif
            #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
            #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
            #endif


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

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
                 float4 texCoord1;
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
                 float4 uv0;
                 float4 uv1;
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
                 float3 interp0 : INTERP0;
                 float3 interp1 : INTERP1;
                 float4 interp2 : INTERP2;
                 float4 interp3 : INTERP3;
                 float4 interp4 : INTERP4;
                 float3 interp5 : INTERP5;
                 float2 interp6 : INTERP6;
                 float3 interp7 : INTERP7;
                 float4 interp8 : INTERP8;
                 float4 interp9 : INTERP9;
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

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.interp0.xyz = input.positionWS;
                output.interp1.xyz = input.normalWS;
                output.interp2.xyzw = input.tangentWS;
                output.interp3.xyzw = input.texCoord0;
                output.interp4.xyzw = input.texCoord1;
                output.interp5.xyz = input.viewDirectionWS;
                #if defined(LIGHTMAP_ON)
                output.interp6.xy = input.lightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.interp7.xyz = input.sh;
                #endif
                output.interp8.xyzw = input.fogFactorAndVertexLight;
                output.interp9.xyzw = input.shadowCoord;
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

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp0.xyz;
                output.normalWS = input.interp1.xyz;
                output.tangentWS = input.interp2.xyzw;
                output.texCoord0 = input.interp3.xyzw;
                output.texCoord1 = input.interp4.xyzw;
                output.viewDirectionWS = input.interp5.xyz;
                #if defined(LIGHTMAP_ON)
                output.lightmapUV = input.interp6.xy;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.interp7.xyz;
                #endif
                output.fogFactorAndVertexLight = input.interp8.xyzw;
                output.shadowCoord = input.interp9.xyzw;
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
            float4 _MainTex_TexelSize;
            float4 _BumpMap_TexelSize;
            float4 _SpecGlossMap_TexelSize;
            float4 _OcclusionMap_TexelSize;
            half _OcclusionIntencity;
            CBUFFER_END

                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_BumpMap);
                SAMPLER(sampler_BumpMap);
                TEXTURE2D(_SpecGlossMap);
                SAMPLER(sampler_SpecGlossMap);
                TEXTURE2D(_OcclusionMap);
                SAMPLER(sampler_OcclusionMap);

                // -- Property used by ScenePickingPass
                #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
                #endif

                // -- Properties used by SceneSelectionPass
                #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
                #endif

                // Graph Includes
                // GraphIncludes: <None>

                // Graph Functions

                void Unity_Blend_Multiply_half4(half4 Base, half4 Blend, out half4 Out, half Opacity)
                {
                    Out = Base * Blend;
                    Out = lerp(Base, Out, Opacity);
                }

                void Unity_Saturate_half4(half4 In, out half4 Out)
                {
                    Out = saturate(In);
                }

                // Custom interpolators pre vertex
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                // Graph Vertex
                struct VertexDescription
                {
                    half3 Position;
                    half3 Normal;
                    half3 Tangent;
                };

                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }

                // Custom interpolators, pre surface
                #ifdef FEATURES_GRAPH_VERTEX
                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                {
                return output;
                }
                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                #endif

                // Graph Pixel
                struct SurfaceDescription
                {
                    half3 BaseColor;
                    half3 NormalTS;
                    half3 Emission;
                    half Metallic;
                    half Smoothness;
                    half Occlusion;
                };

                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    UnityTexture2D _Property_10fe7d19df084ea49913608ddae8394c_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                    half4 _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_10fe7d19df084ea49913608ddae8394c_Out_0.tex, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.samplerstate, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.GetTransformedUV(IN.uv0.xy));
                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_R_4 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.r;
                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_G_5 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.g;
                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_B_6 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.b;
                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_A_7 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.a;
                    UnityTexture2D _Property_770e05d7e80443d7a75127092eb0c28e_Out_0 = UnityBuildTexture2DStructNoScale(_OcclusionMap);
                    half4 _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_770e05d7e80443d7a75127092eb0c28e_Out_0.tex, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.samplerstate, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.GetTransformedUV(IN.uv1.xy));
                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_R_4 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.r;
                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_G_5 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.g;
                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_B_6 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.b;
                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_A_7 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.a;
                    half _Property_aea6ee9cf47244888e2362762a4060d5_Out_0 = _OcclusionIntencity;
                    half4 _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2;
                    Unity_Blend_Multiply_half4(_SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0, _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0, _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Property_aea6ee9cf47244888e2362762a4060d5_Out_0);
                    half4 _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1;
                    Unity_Saturate_half4(_Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1);
                    UnityTexture2D _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
                    half4 _UV_5d8c32001b284585ac83350fe0533110_Out_0 = IN.uv0;
                    half4 _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0 = SAMPLE_TEXTURE2D(_Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.tex, _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.samplerstate, _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.GetTransformedUV((_UV_5d8c32001b284585ac83350fe0533110_Out_0.xy)));
                    _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0);
                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_R_4 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.r;
                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_G_5 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.g;
                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_B_6 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.b;
                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_A_7 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.a;
                    UnityTexture2D _Property_4172ce25317b435987c9fc866993897e_Out_0 = UnityBuildTexture2DStructNoScale(_SpecGlossMap);
                    half4 _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4172ce25317b435987c9fc866993897e_Out_0.tex, _Property_4172ce25317b435987c9fc866993897e_Out_0.samplerstate, _Property_4172ce25317b435987c9fc866993897e_Out_0.GetTransformedUV(IN.uv0.xy));
                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_R_4 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.r;
                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_G_5 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.g;
                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_B_6 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.b;
                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_A_7 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.a;
                    surface.BaseColor = (_Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1.xyz);
                    surface.NormalTS = (_SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.xyz);
                    surface.Emission = half3(0, 0, 0);
                    surface.Metallic = 0;
                    surface.Smoothness = (_SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0).x;
                    surface.Occlusion = 1;
                    return surface;
                }

                // --------------------------------------------------
                // Build Graph Inputs

                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                    output.ObjectSpaceNormal = input.normalOS;
                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                    output.ObjectSpacePosition = input.positionOS;

                    return output;
                }
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





                    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                    output.uv0 = input.texCoord0;
                    output.uv1 = input.texCoord1;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                        return output;
                }

                void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                {
                    result.vertex = float4(attributes.positionOS, 1);
                    result.tangent = attributes.tangentOS;
                    result.normal = attributes.normalOS;
                    result.texcoord = attributes.uv0;
                    result.texcoord1 = attributes.uv1;
                    result.vertex = float4(vertexDescription.Position, 1);
                    result.normal = vertexDescription.Normal;
                    result.tangent = float4(vertexDescription.Tangent, 0);
                    #if UNITY_ANY_INSTANCING_ENABLED
                    #endif
                }

                void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                {
                    result.pos = varyings.positionCS;
                    result.worldPos = varyings.positionWS;
                    result.worldNormal = varyings.normalWS;
                    result.viewDir = varyings.viewDirectionWS;
                    // World Tangent isn't an available input on v2f_surf

                    result._ShadowCoord = varyings.shadowCoord;

                    #if UNITY_ANY_INSTANCING_ENABLED
                    #endif
                    #if UNITY_SHOULD_SAMPLE_SH
                    result.sh = varyings.sh;
                    #endif
                    #if defined(LIGHTMAP_ON)
                    result.lmap.xy = varyings.lightmapUV;
                    #endif
                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                        result.fogCoord = varyings.fogFactorAndVertexLight.x;
                        COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                    #endif

                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                }

                void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                {
                    result.positionCS = surfVertex.pos;
                    result.positionWS = surfVertex.worldPos;
                    result.normalWS = surfVertex.worldNormal;
                    // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                    // World Tangent isn't an available input on v2f_surf
                    result.shadowCoord = surfVertex._ShadowCoord;

                    #if UNITY_ANY_INSTANCING_ENABLED
                    #endif
                    #if UNITY_SHOULD_SAMPLE_SH
                    result.sh = surfVertex.sh;
                    #endif
                    #if defined(LIGHTMAP_ON)
                    result.lightmapUV = surfVertex.lmap.xy;
                    #endif
                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                        result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                        COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                    #endif

                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                }

                // --------------------------------------------------
                // Main

                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

                ENDHLSL
                }
                Pass
                {
                    Name "BuiltIn ForwardAdd"
                    Tags
                    {
                        "LightMode" = "ForwardAdd"
                    }

                    // Render State
                    Blend SrcAlpha One, One One
                    ZWrite Off

                    // Debug
                    // <None>

                    // --------------------------------------------------
                    // Pass

                    HLSLPROGRAM

                    // Pragmas
                    #pragma target 3.0
                    //#pragma multi_compile_instancing
                    //#pragma multi_compile_fog
                    //#pragma multi_compile_fwdadd_fullshadows
                    #pragma vertex vert
                    #pragma fragment frag

                    // DotsInstancingOptions: <None>
                    // HybridV1InjectedBuiltinProperties: <None>

                    // Keywords
                    //#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
                    //#pragma multi_compile _ LIGHTMAP_ON
                    //#pragma multi_compile _ DIRLIGHTMAP_COMBINED
                    //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                    //#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
                    //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
                    //#pragma multi_compile _ _SHADOWS_SOFT
                    //#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                    //#pragma multi_compile _ SHADOWS_SHADOWMASK
                    // GraphKeywords: <None>

                    // Defines
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
                    #define VARYINGS_NEED_TEXCOORD1
                    #define VARYINGS_NEED_VIEWDIRECTION_WS
                    #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                    #define FEATURES_GRAPH_VERTEX
                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                    #define SHADERPASS SHADERPASS_FORWARD_ADD
                    #define BUILTIN_TARGET_API 1
                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
                    #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
                    #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
                    #endif
                    #ifdef _BUILTIN_ALPHATEST_ON
                    #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
                    #endif
                    #ifdef _BUILTIN_AlphaClip
                    #define _AlphaClip _BUILTIN_AlphaClip
                    #endif
                    #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
                    #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
                    #endif


                    // custom interpolator pre-include
                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                    // Includes
                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

                    // --------------------------------------------------
                    // Structs and Packing

                    // custom interpolators pre packing
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

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
                         float4 texCoord1;
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
                         float4 uv0;
                         float4 uv1;
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
                         float3 interp0 : INTERP0;
                         float3 interp1 : INTERP1;
                         float4 interp2 : INTERP2;
                         float4 interp3 : INTERP3;
                         float4 interp4 : INTERP4;
                         float3 interp5 : INTERP5;
                         float2 interp6 : INTERP6;
                         float3 interp7 : INTERP7;
                         float4 interp8 : INTERP8;
                         float4 interp9 : INTERP9;
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

                    PackedVaryings PackVaryings(Varyings input)
                    {
                        PackedVaryings output;
                        ZERO_INITIALIZE(PackedVaryings, output);
                        output.positionCS = input.positionCS;
                        output.interp0.xyz = input.positionWS;
                        output.interp1.xyz = input.normalWS;
                        output.interp2.xyzw = input.tangentWS;
                        output.interp3.xyzw = input.texCoord0;
                        output.interp4.xyzw = input.texCoord1;
                        output.interp5.xyz = input.viewDirectionWS;
                        #if defined(LIGHTMAP_ON)
                        output.interp6.xy = input.lightmapUV;
                        #endif
                        #if !defined(LIGHTMAP_ON)
                        output.interp7.xyz = input.sh;
                        #endif
                        output.interp8.xyzw = input.fogFactorAndVertexLight;
                        output.interp9.xyzw = input.shadowCoord;
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

                    Varyings UnpackVaryings(PackedVaryings input)
                    {
                        Varyings output;
                        output.positionCS = input.positionCS;
                        output.positionWS = input.interp0.xyz;
                        output.normalWS = input.interp1.xyz;
                        output.tangentWS = input.interp2.xyzw;
                        output.texCoord0 = input.interp3.xyzw;
                        output.texCoord1 = input.interp4.xyzw;
                        output.viewDirectionWS = input.interp5.xyz;
                        #if defined(LIGHTMAP_ON)
                        output.lightmapUV = input.interp6.xy;
                        #endif
                        #if !defined(LIGHTMAP_ON)
                        output.sh = input.interp7.xyz;
                        #endif
                        output.fogFactorAndVertexLight = input.interp8.xyzw;
                        output.shadowCoord = input.interp9.xyzw;
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
                    float4 _MainTex_TexelSize;
                    float4 _BumpMap_TexelSize;
                    float4 _SpecGlossMap_TexelSize;
                    float4 _OcclusionMap_TexelSize;
                    half _OcclusionIntencity;
                    CBUFFER_END

                        // Object and Global properties
                        SAMPLER(SamplerState_Linear_Repeat);
                        TEXTURE2D(_MainTex);
                        SAMPLER(sampler_MainTex);
                        TEXTURE2D(_BumpMap);
                        SAMPLER(sampler_BumpMap);
                        TEXTURE2D(_SpecGlossMap);
                        SAMPLER(sampler_SpecGlossMap);
                        TEXTURE2D(_OcclusionMap);
                        SAMPLER(sampler_OcclusionMap);

                        // -- Property used by ScenePickingPass
                        #ifdef SCENEPICKINGPASS
                        float4 _SelectionID;
                        #endif

                        // -- Properties used by SceneSelectionPass
                        #ifdef SCENESELECTIONPASS
                        int _ObjectId;
                        int _PassValue;
                        #endif

                        // Graph Includes
                        // GraphIncludes: <None>

                        // Graph Functions

                        void Unity_Blend_Multiply_half4(half4 Base, half4 Blend, out half4 Out, half Opacity)
                        {
                            Out = Base * Blend;
                            Out = lerp(Base, Out, Opacity);
                        }

                        void Unity_Saturate_half4(half4 In, out half4 Out)
                        {
                            Out = saturate(In);
                        }

                        // Custom interpolators pre vertex
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                        // Graph Vertex
                        struct VertexDescription
                        {
                            half3 Position;
                            half3 Normal;
                            half3 Tangent;
                        };

                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                        {
                            VertexDescription description = (VertexDescription)0;
                            description.Position = IN.ObjectSpacePosition;
                            description.Normal = IN.ObjectSpaceNormal;
                            description.Tangent = IN.ObjectSpaceTangent;
                            return description;
                        }

                        // Custom interpolators, pre surface
                        #ifdef FEATURES_GRAPH_VERTEX
                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                        {
                        return output;
                        }
                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                        #endif

                        // Graph Pixel
                        struct SurfaceDescription
                        {
                            half3 BaseColor;
                            half3 NormalTS;
                            half3 Emission;
                            half Metallic;
                            half Smoothness;
                            half Occlusion;
                        };

                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                        {
                            SurfaceDescription surface = (SurfaceDescription)0;
                            UnityTexture2D _Property_10fe7d19df084ea49913608ddae8394c_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                            half4 _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_10fe7d19df084ea49913608ddae8394c_Out_0.tex, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.samplerstate, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.GetTransformedUV(IN.uv0.xy));
                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_R_4 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.r;
                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_G_5 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.g;
                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_B_6 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.b;
                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_A_7 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.a;
                            UnityTexture2D _Property_770e05d7e80443d7a75127092eb0c28e_Out_0 = UnityBuildTexture2DStructNoScale(_OcclusionMap);
                            half4 _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_770e05d7e80443d7a75127092eb0c28e_Out_0.tex, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.samplerstate, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.GetTransformedUV(IN.uv1.xy));
                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_R_4 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.r;
                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_G_5 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.g;
                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_B_6 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.b;
                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_A_7 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.a;
                            half _Property_aea6ee9cf47244888e2362762a4060d5_Out_0 = _OcclusionIntencity;
                            half4 _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2;
                            Unity_Blend_Multiply_half4(_SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0, _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0, _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Property_aea6ee9cf47244888e2362762a4060d5_Out_0);
                            half4 _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1;
                            Unity_Saturate_half4(_Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1);
                            UnityTexture2D _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
                            half4 _UV_5d8c32001b284585ac83350fe0533110_Out_0 = IN.uv0;
                            half4 _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0 = SAMPLE_TEXTURE2D(_Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.tex, _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.samplerstate, _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.GetTransformedUV((_UV_5d8c32001b284585ac83350fe0533110_Out_0.xy)));
                            _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0);
                            half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_R_4 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.r;
                            half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_G_5 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.g;
                            half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_B_6 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.b;
                            half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_A_7 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.a;
                            UnityTexture2D _Property_4172ce25317b435987c9fc866993897e_Out_0 = UnityBuildTexture2DStructNoScale(_SpecGlossMap);
                            half4 _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4172ce25317b435987c9fc866993897e_Out_0.tex, _Property_4172ce25317b435987c9fc866993897e_Out_0.samplerstate, _Property_4172ce25317b435987c9fc866993897e_Out_0.GetTransformedUV(IN.uv0.xy));
                            half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_R_4 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.r;
                            half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_G_5 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.g;
                            half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_B_6 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.b;
                            half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_A_7 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.a;
                            surface.BaseColor = (_Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1.xyz);
                            surface.NormalTS = (_SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.xyz);
                            surface.Emission = half3(0, 0, 0);
                            surface.Metallic = 0;
                            surface.Smoothness = (_SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0).x;
                            surface.Occlusion = 1;
                            return surface;
                        }

                        // --------------------------------------------------
                        // Build Graph Inputs

                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                        {
                            VertexDescriptionInputs output;
                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                            output.ObjectSpaceNormal = input.normalOS;
                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                            output.ObjectSpacePosition = input.positionOS;

                            return output;
                        }
                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                        {
                            SurfaceDescriptionInputs output;
                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





                            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                            output.uv0 = input.texCoord0;
                            output.uv1 = input.texCoord1;
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                        #else
                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                        #endif
                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                return output;
                        }

                        void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                        {
                            result.vertex = float4(attributes.positionOS, 1);
                            result.tangent = attributes.tangentOS;
                            result.normal = attributes.normalOS;
                            result.texcoord = attributes.uv0;
                            result.texcoord1 = attributes.uv1;
                            result.vertex = float4(vertexDescription.Position, 1);
                            result.normal = vertexDescription.Normal;
                            result.tangent = float4(vertexDescription.Tangent, 0);
                            #if UNITY_ANY_INSTANCING_ENABLED
                            #endif
                        }

                        void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                        {
                            result.pos = varyings.positionCS;
                            result.worldPos = varyings.positionWS;
                            result.worldNormal = varyings.normalWS;
                            result.viewDir = varyings.viewDirectionWS;
                            // World Tangent isn't an available input on v2f_surf

                            result._ShadowCoord = varyings.shadowCoord;

                            #if UNITY_ANY_INSTANCING_ENABLED
                            #endif
                            #if UNITY_SHOULD_SAMPLE_SH
                            result.sh = varyings.sh;
                            #endif
                            #if defined(LIGHTMAP_ON)
                            result.lmap.xy = varyings.lightmapUV;
                            #endif
                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                result.fogCoord = varyings.fogFactorAndVertexLight.x;
                                COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                            #endif

                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                        }

                        void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                        {
                            result.positionCS = surfVertex.pos;
                            result.positionWS = surfVertex.worldPos;
                            result.normalWS = surfVertex.worldNormal;
                            // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                            // World Tangent isn't an available input on v2f_surf
                            result.shadowCoord = surfVertex._ShadowCoord;

                            #if UNITY_ANY_INSTANCING_ENABLED
                            #endif
                            #if UNITY_SHOULD_SAMPLE_SH
                            result.sh = surfVertex.sh;
                            #endif
                            #if defined(LIGHTMAP_ON)
                            result.lightmapUV = surfVertex.lmap.xy;
                            #endif
                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                                COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                            #endif

                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                        }

                        // --------------------------------------------------
                        // Main

                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/PBRForwardAddPass.hlsl"

                        ENDHLSL
                        }
                        Pass
                        {
                            Name "BuiltIn Deferred"
                            Tags
                            {
                                "LightMode" = "Deferred"
                            }

                            // Render State
                            Cull Back
                            Blend One Zero
                            ZTest LEqual
                            ZWrite On

                            // Debug
                            // <None>

                            // --------------------------------------------------
                            // Pass

                            HLSLPROGRAM

                            // Pragmas
                            #pragma target 4.5
                            #pragma multi_compile_instancing
                            #pragma exclude_renderers nomrt
                            #pragma multi_compile_prepassfinal
                            #pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
                            #pragma vertex vert
                            #pragma fragment frag

                            // DotsInstancingOptions: <None>
                            // HybridV1InjectedBuiltinProperties: <None>

                            // Keywords
                            //#pragma multi_compile _ LIGHTMAP_ON
                            //#pragma multi_compile _ DIRLIGHTMAP_COMBINED
                            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                            //#pragma multi_compile _ _SHADOWS_SOFT
                           // #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                            //#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
                            //#pragma multi_compile _ _GBUFFER_NORMALS_OCT
                            // GraphKeywords: <None>

                            // Defines
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
                            #define VARYINGS_NEED_TEXCOORD1
                            #define VARYINGS_NEED_VIEWDIRECTION_WS
                            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                            #define FEATURES_GRAPH_VERTEX
                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                            #define SHADERPASS SHADERPASS_DEFERRED
                            #define BUILTIN_TARGET_API 1
                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
                            #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
                            #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
                            #endif
                            #ifdef _BUILTIN_ALPHATEST_ON
                            #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
                            #endif
                            #ifdef _BUILTIN_AlphaClip
                            #define _AlphaClip _BUILTIN_AlphaClip
                            #endif
                            #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
                            #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
                            #endif


                            // custom interpolator pre-include
                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                            // Includes
                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

                            // --------------------------------------------------
                            // Structs and Packing

                            // custom interpolators pre packing
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

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
                                 float4 texCoord1;
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
                                 float4 uv0;
                                 float4 uv1;
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
                                 float3 interp0 : INTERP0;
                                 float3 interp1 : INTERP1;
                                 float4 interp2 : INTERP2;
                                 float4 interp3 : INTERP3;
                                 float4 interp4 : INTERP4;
                                 float3 interp5 : INTERP5;
                                 float2 interp6 : INTERP6;
                                 float3 interp7 : INTERP7;
                                 float4 interp8 : INTERP8;
                                 float4 interp9 : INTERP9;
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

                            PackedVaryings PackVaryings(Varyings input)
                            {
                                PackedVaryings output;
                                ZERO_INITIALIZE(PackedVaryings, output);
                                output.positionCS = input.positionCS;
                                output.interp0.xyz = input.positionWS;
                                output.interp1.xyz = input.normalWS;
                                output.interp2.xyzw = input.tangentWS;
                                output.interp3.xyzw = input.texCoord0;
                                output.interp4.xyzw = input.texCoord1;
                                output.interp5.xyz = input.viewDirectionWS;
                                #if defined(LIGHTMAP_ON)
                                output.interp6.xy = input.lightmapUV;
                                #endif
                                #if !defined(LIGHTMAP_ON)
                                output.interp7.xyz = input.sh;
                                #endif
                                output.interp8.xyzw = input.fogFactorAndVertexLight;
                                output.interp9.xyzw = input.shadowCoord;
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

                            Varyings UnpackVaryings(PackedVaryings input)
                            {
                                Varyings output;
                                output.positionCS = input.positionCS;
                                output.positionWS = input.interp0.xyz;
                                output.normalWS = input.interp1.xyz;
                                output.tangentWS = input.interp2.xyzw;
                                output.texCoord0 = input.interp3.xyzw;
                                output.texCoord1 = input.interp4.xyzw;
                                output.viewDirectionWS = input.interp5.xyz;
                                #if defined(LIGHTMAP_ON)
                                output.lightmapUV = input.interp6.xy;
                                #endif
                                #if !defined(LIGHTMAP_ON)
                                output.sh = input.interp7.xyz;
                                #endif
                                output.fogFactorAndVertexLight = input.interp8.xyzw;
                                output.shadowCoord = input.interp9.xyzw;
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
                            float4 _MainTex_TexelSize;
                            float4 _BumpMap_TexelSize;
                            float4 _SpecGlossMap_TexelSize;
                            float4 _OcclusionMap_TexelSize;
                            half _OcclusionIntencity;
                            CBUFFER_END

                                // Object and Global properties
                                SAMPLER(SamplerState_Linear_Repeat);
                                TEXTURE2D(_MainTex);
                                SAMPLER(sampler_MainTex);
                                TEXTURE2D(_BumpMap);
                                SAMPLER(sampler_BumpMap);
                                TEXTURE2D(_SpecGlossMap);
                                SAMPLER(sampler_SpecGlossMap);
                                TEXTURE2D(_OcclusionMap);
                                SAMPLER(sampler_OcclusionMap);

                                // -- Property used by ScenePickingPass
                                #ifdef SCENEPICKINGPASS
                                float4 _SelectionID;
                                #endif

                                // -- Properties used by SceneSelectionPass
                                #ifdef SCENESELECTIONPASS
                                int _ObjectId;
                                int _PassValue;
                                #endif

                                // Graph Includes
                                // GraphIncludes: <None>

                                // Graph Functions

                                void Unity_Blend_Multiply_half4(half4 Base, half4 Blend, out half4 Out, half Opacity)
                                {
                                    Out = Base * Blend;
                                    Out = lerp(Base, Out, Opacity);
                                }

                                void Unity_Saturate_half4(half4 In, out half4 Out)
                                {
                                    Out = saturate(In);
                                }

                                // Custom interpolators pre vertex
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                // Graph Vertex
                                struct VertexDescription
                                {
                                    half3 Position;
                                    half3 Normal;
                                    half3 Tangent;
                                };

                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                {
                                    VertexDescription description = (VertexDescription)0;
                                    description.Position = IN.ObjectSpacePosition;
                                    description.Normal = IN.ObjectSpaceNormal;
                                    description.Tangent = IN.ObjectSpaceTangent;
                                    return description;
                                }

                                // Custom interpolators, pre surface
                                #ifdef FEATURES_GRAPH_VERTEX
                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                {
                                return output;
                                }
                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                #endif

                                // Graph Pixel
                                struct SurfaceDescription
                                {
                                    half3 BaseColor;
                                    half3 NormalTS;
                                    half3 Emission;
                                    half Metallic;
                                    half Smoothness;
                                    half Occlusion;
                                };

                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                {
                                    SurfaceDescription surface = (SurfaceDescription)0;
                                    UnityTexture2D _Property_10fe7d19df084ea49913608ddae8394c_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                    half4 _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_10fe7d19df084ea49913608ddae8394c_Out_0.tex, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.samplerstate, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.GetTransformedUV(IN.uv0.xy));
                                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_R_4 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.r;
                                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_G_5 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.g;
                                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_B_6 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.b;
                                    half _SampleTexture2D_27b5932900db428985068236a13a9de2_A_7 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.a;
                                    UnityTexture2D _Property_770e05d7e80443d7a75127092eb0c28e_Out_0 = UnityBuildTexture2DStructNoScale(_OcclusionMap);
                                    half4 _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_770e05d7e80443d7a75127092eb0c28e_Out_0.tex, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.samplerstate, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.GetTransformedUV(IN.uv1.xy));
                                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_R_4 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.r;
                                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_G_5 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.g;
                                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_B_6 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.b;
                                    half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_A_7 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.a;
                                    half _Property_aea6ee9cf47244888e2362762a4060d5_Out_0 = _OcclusionIntencity;
                                    half4 _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2;
                                    Unity_Blend_Multiply_half4(_SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0, _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0, _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Property_aea6ee9cf47244888e2362762a4060d5_Out_0);
                                    half4 _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1;
                                    Unity_Saturate_half4(_Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1);
                                    UnityTexture2D _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
                                    half4 _UV_5d8c32001b284585ac83350fe0533110_Out_0 = IN.uv0;
                                    half4 _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0 = SAMPLE_TEXTURE2D(_Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.tex, _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.samplerstate, _Property_791a6cb314ce4776b9a3dc21cb6add23_Out_0.GetTransformedUV((_UV_5d8c32001b284585ac83350fe0533110_Out_0.xy)));
                                    _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0);
                                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_R_4 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.r;
                                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_G_5 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.g;
                                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_B_6 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.b;
                                    half _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_A_7 = _SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.a;
                                    UnityTexture2D _Property_4172ce25317b435987c9fc866993897e_Out_0 = UnityBuildTexture2DStructNoScale(_SpecGlossMap);
                                    half4 _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4172ce25317b435987c9fc866993897e_Out_0.tex, _Property_4172ce25317b435987c9fc866993897e_Out_0.samplerstate, _Property_4172ce25317b435987c9fc866993897e_Out_0.GetTransformedUV(IN.uv0.xy));
                                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_R_4 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.r;
                                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_G_5 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.g;
                                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_B_6 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.b;
                                    half _SampleTexture2D_95145604c986425aacc5bf1e61858da4_A_7 = _SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0.a;
                                    surface.BaseColor = (_Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1.xyz);
                                    surface.NormalTS = (_SampleTexture2D_7746b97a38bd4bde8fc18bb55ff54319_RGBA_0.xyz);
                                    surface.Emission = half3(0, 0, 0);
                                    surface.Metallic = 0;
                                    surface.Smoothness = (_SampleTexture2D_95145604c986425aacc5bf1e61858da4_RGBA_0).x;
                                    surface.Occlusion = 1;
                                    return surface;
                                }

                                // --------------------------------------------------
                                // Build Graph Inputs

                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                {
                                    VertexDescriptionInputs output;
                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                    output.ObjectSpaceNormal = input.normalOS;
                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                    output.ObjectSpacePosition = input.positionOS;

                                    return output;
                                }
                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                {
                                    SurfaceDescriptionInputs output;
                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





                                    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                                    output.uv0 = input.texCoord0;
                                    output.uv1 = input.texCoord1;
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                #else
                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                #endif
                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                        return output;
                                }

                                void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                                {
                                    result.vertex = float4(attributes.positionOS, 1);
                                    result.tangent = attributes.tangentOS;
                                    result.normal = attributes.normalOS;
                                    result.texcoord = attributes.uv0;
                                    result.texcoord1 = attributes.uv1;
                                    result.vertex = float4(vertexDescription.Position, 1);
                                    result.normal = vertexDescription.Normal;
                                    result.tangent = float4(vertexDescription.Tangent, 0);
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    #endif
                                }

                                void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                                {
                                    result.pos = varyings.positionCS;
                                    result.worldPos = varyings.positionWS;
                                    result.worldNormal = varyings.normalWS;
                                    result.viewDir = varyings.viewDirectionWS;
                                    // World Tangent isn't an available input on v2f_surf

                                    result._ShadowCoord = varyings.shadowCoord;

                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    #endif
                                    #if UNITY_SHOULD_SAMPLE_SH
                                    result.sh = varyings.sh;
                                    #endif
                                    #if defined(LIGHTMAP_ON)
                                    result.lmap.xy = varyings.lightmapUV;
                                    #endif
                                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                        result.fogCoord = varyings.fogFactorAndVertexLight.x;
                                        COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                                    #endif

                                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                                }

                                void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                                {
                                    result.positionCS = surfVertex.pos;
                                    result.positionWS = surfVertex.worldPos;
                                    result.normalWS = surfVertex.worldNormal;
                                    // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                                    // World Tangent isn't an available input on v2f_surf
                                    result.shadowCoord = surfVertex._ShadowCoord;

                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    #endif
                                    #if UNITY_SHOULD_SAMPLE_SH
                                    result.sh = surfVertex.sh;
                                    #endif
                                    #if defined(LIGHTMAP_ON)
                                    result.lightmapUV = surfVertex.lmap.xy;
                                    #endif
                                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                        result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                                        COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                                    #endif

                                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                                }

                                // --------------------------------------------------
                                // Main

                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/PBRDeferredPass.hlsl"

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
                                    Blend One Zero
                                    ZTest LEqual
                                    ZWrite On
                                    ColorMask 0

                                    // Debug
                                    // <None>

                                    // --------------------------------------------------
                                    // Pass

                                    HLSLPROGRAM

                                    // Pragmas
                                    #pragma target 3.0
                                    #pragma multi_compile_shadowcaster
                                    #pragma vertex vert
                                    #pragma fragment frag

                                    // DotsInstancingOptions: <None>
                                    // HybridV1InjectedBuiltinProperties: <None>

                                    // Keywords
                                    //#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW
                                    // GraphKeywords: <None>

                                    // Defines
                                    #define _NORMALMAP 1
                                    #define _NORMAL_DROPOFF_TS 1
                                    #define ATTRIBUTES_NEED_NORMAL
                                    #define ATTRIBUTES_NEED_TANGENT
                                    #define FEATURES_GRAPH_VERTEX
                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                    #define SHADERPASS SHADERPASS_SHADOWCASTER
                                    #define BUILTIN_TARGET_API 1
                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
                                    #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                    #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                    #endif
                                    #ifdef _BUILTIN_ALPHATEST_ON
                                    #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
                                    #endif
                                    #ifdef _BUILTIN_AlphaClip
                                    #define _AlphaClip _BUILTIN_AlphaClip
                                    #endif
                                    #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
                                    #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
                                    #endif


                                    // custom interpolator pre-include
                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                    // Includes
                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                    // --------------------------------------------------
                                    // Structs and Packing

                                    // custom interpolators pre packing
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                    struct Attributes
                                    {
                                         float3 positionOS : POSITION;
                                         float3 normalOS : NORMAL;
                                         float4 tangentOS : TANGENT;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                         uint instanceID : INSTANCEID_SEMANTIC;
                                        #endif
                                    };
                                    struct Varyings
                                    {
                                         float4 positionCS : SV_POSITION;
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

                                    PackedVaryings PackVaryings(Varyings input)
                                    {
                                        PackedVaryings output;
                                        ZERO_INITIALIZE(PackedVaryings, output);
                                        output.positionCS = input.positionCS;
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

                                    Varyings UnpackVaryings(PackedVaryings input)
                                    {
                                        Varyings output;
                                        output.positionCS = input.positionCS;
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
                                    float4 _MainTex_TexelSize;
                                    float4 _BumpMap_TexelSize;
                                    float4 _SpecGlossMap_TexelSize;
                                    float4 _OcclusionMap_TexelSize;
                                    half _OcclusionIntencity;
                                    CBUFFER_END

                                        // Object and Global properties
                                        SAMPLER(SamplerState_Linear_Repeat);
                                        TEXTURE2D(_MainTex);
                                        SAMPLER(sampler_MainTex);
                                        TEXTURE2D(_BumpMap);
                                        SAMPLER(sampler_BumpMap);
                                        TEXTURE2D(_SpecGlossMap);
                                        SAMPLER(sampler_SpecGlossMap);
                                        TEXTURE2D(_OcclusionMap);
                                        SAMPLER(sampler_OcclusionMap);

                                        // -- Property used by ScenePickingPass
                                        #ifdef SCENEPICKINGPASS
                                        float4 _SelectionID;
                                        #endif

                                        // -- Properties used by SceneSelectionPass
                                        #ifdef SCENESELECTIONPASS
                                        int _ObjectId;
                                        int _PassValue;
                                        #endif

                                        // Graph Includes
                                        // GraphIncludes: <None>

                                        // Graph Functions
                                        // GraphFunctions: <None>

                                        // Custom interpolators pre vertex
                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                        // Graph Vertex
                                        struct VertexDescription
                                        {
                                            half3 Position;
                                            half3 Normal;
                                            half3 Tangent;
                                        };

                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                        {
                                            VertexDescription description = (VertexDescription)0;
                                            description.Position = IN.ObjectSpacePosition;
                                            description.Normal = IN.ObjectSpaceNormal;
                                            description.Tangent = IN.ObjectSpaceTangent;
                                            return description;
                                        }

                                        // Custom interpolators, pre surface
                                        #ifdef FEATURES_GRAPH_VERTEX
                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                        {
                                        return output;
                                        }
                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                        #endif

                                        // Graph Pixel
                                        struct SurfaceDescription
                                        {
                                        };

                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                        {
                                            SurfaceDescription surface = (SurfaceDescription)0;
                                            return surface;
                                        }

                                        // --------------------------------------------------
                                        // Build Graph Inputs

                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                        {
                                            VertexDescriptionInputs output;
                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                            output.ObjectSpaceNormal = input.normalOS;
                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                            output.ObjectSpacePosition = input.positionOS;

                                            return output;
                                        }
                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                        {
                                            SurfaceDescriptionInputs output;
                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                        #else
                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                        #endif
                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                return output;
                                        }

                                        void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                                        {
                                            result.vertex = float4(attributes.positionOS, 1);
                                            result.tangent = attributes.tangentOS;
                                            result.normal = attributes.normalOS;
                                            result.vertex = float4(vertexDescription.Position, 1);
                                            result.normal = vertexDescription.Normal;
                                            result.tangent = float4(vertexDescription.Tangent, 0);
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            #endif
                                        }

                                        void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                                        {
                                            result.pos = varyings.positionCS;
                                            // World Tangent isn't an available input on v2f_surf


                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            #endif
                                            #if UNITY_SHOULD_SAMPLE_SH
                                            #endif
                                            #if defined(LIGHTMAP_ON)
                                            #endif
                                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                result.fogCoord = varyings.fogFactorAndVertexLight.x;
                                                COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                                            #endif

                                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                                        }

                                        void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                                        {
                                            result.positionCS = surfVertex.pos;
                                            // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                                            // World Tangent isn't an available input on v2f_surf

                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            #endif
                                            #if UNITY_SHOULD_SAMPLE_SH
                                            #endif
                                            #if defined(LIGHTMAP_ON)
                                            #endif
                                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                                                COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                                            #endif

                                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                                        }

                                        // --------------------------------------------------
                                        // Main

                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

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
                                            Blend One Zero
                                            ZTest LEqual
                                            ZWrite On
                                            ColorMask 0

                                            // Debug
                                            // <None>

                                            // --------------------------------------------------
                                            // Pass

                                            HLSLPROGRAM

                                            // Pragmas
                                            #pragma target 3.0
                                            //#pragma multi_compile_instancing
                                            #pragma vertex vert
                                            #pragma fragment frag

                                            // DotsInstancingOptions: <None>
                                            // HybridV1InjectedBuiltinProperties: <None>

                                            // Keywords
                                            // PassKeywords: <None>
                                            // GraphKeywords: <None>

                                            // Defines
                                            #define _NORMALMAP 1
                                            #define _NORMAL_DROPOFF_TS 1
                                            #define ATTRIBUTES_NEED_NORMAL
                                            #define ATTRIBUTES_NEED_TANGENT
                                            #define FEATURES_GRAPH_VERTEX
                                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                            #define SHADERPASS SHADERPASS_DEPTHONLY
                                            #define BUILTIN_TARGET_API 1
                                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
                                            #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                            #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                            #endif
                                            #ifdef _BUILTIN_ALPHATEST_ON
                                            #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
                                            #endif
                                            #ifdef _BUILTIN_AlphaClip
                                            #define _AlphaClip _BUILTIN_AlphaClip
                                            #endif
                                            #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
                                            #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
                                            #endif


                                            // custom interpolator pre-include
                                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                            // Includes
                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                            // --------------------------------------------------
                                            // Structs and Packing

                                            // custom interpolators pre packing
                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                            struct Attributes
                                            {
                                                 float3 positionOS : POSITION;
                                                 float3 normalOS : NORMAL;
                                                 float4 tangentOS : TANGENT;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                 uint instanceID : INSTANCEID_SEMANTIC;
                                                #endif
                                            };
                                            struct Varyings
                                            {
                                                 float4 positionCS : SV_POSITION;
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

                                            PackedVaryings PackVaryings(Varyings input)
                                            {
                                                PackedVaryings output;
                                                ZERO_INITIALIZE(PackedVaryings, output);
                                                output.positionCS = input.positionCS;
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

                                            Varyings UnpackVaryings(PackedVaryings input)
                                            {
                                                Varyings output;
                                                output.positionCS = input.positionCS;
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
                                            float4 _MainTex_TexelSize;
                                            float4 _BumpMap_TexelSize;
                                            float4 _SpecGlossMap_TexelSize;
                                            float4 _OcclusionMap_TexelSize;
                                            half _OcclusionIntencity;
                                            CBUFFER_END

                                                // Object and Global properties
                                                SAMPLER(SamplerState_Linear_Repeat);
                                                TEXTURE2D(_MainTex);
                                                SAMPLER(sampler_MainTex);
                                                TEXTURE2D(_BumpMap);
                                                SAMPLER(sampler_BumpMap);
                                                TEXTURE2D(_SpecGlossMap);
                                                SAMPLER(sampler_SpecGlossMap);
                                                TEXTURE2D(_OcclusionMap);
                                                SAMPLER(sampler_OcclusionMap);

                                                // -- Property used by ScenePickingPass
                                                #ifdef SCENEPICKINGPASS
                                                float4 _SelectionID;
                                                #endif

                                                // -- Properties used by SceneSelectionPass
                                                #ifdef SCENESELECTIONPASS
                                                int _ObjectId;
                                                int _PassValue;
                                                #endif

                                                // Graph Includes
                                                // GraphIncludes: <None>

                                                // Graph Functions
                                                // GraphFunctions: <None>

                                                // Custom interpolators pre vertex
                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                // Graph Vertex
                                                struct VertexDescription
                                                {
                                                    half3 Position;
                                                    half3 Normal;
                                                    half3 Tangent;
                                                };

                                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                {
                                                    VertexDescription description = (VertexDescription)0;
                                                    description.Position = IN.ObjectSpacePosition;
                                                    description.Normal = IN.ObjectSpaceNormal;
                                                    description.Tangent = IN.ObjectSpaceTangent;
                                                    return description;
                                                }

                                                // Custom interpolators, pre surface
                                                #ifdef FEATURES_GRAPH_VERTEX
                                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                {
                                                return output;
                                                }
                                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                #endif

                                                // Graph Pixel
                                                struct SurfaceDescription
                                                {
                                                };

                                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                {
                                                    SurfaceDescription surface = (SurfaceDescription)0;
                                                    return surface;
                                                }

                                                // --------------------------------------------------
                                                // Build Graph Inputs

                                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                {
                                                    VertexDescriptionInputs output;
                                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                    output.ObjectSpaceNormal = input.normalOS;
                                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                    output.ObjectSpacePosition = input.positionOS;

                                                    return output;
                                                }
                                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                {
                                                    SurfaceDescriptionInputs output;
                                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                #else
                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                #endif
                                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                        return output;
                                                }

                                                void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                                                {
                                                    result.vertex = float4(attributes.positionOS, 1);
                                                    result.tangent = attributes.tangentOS;
                                                    result.normal = attributes.normalOS;
                                                    result.vertex = float4(vertexDescription.Position, 1);
                                                    result.normal = vertexDescription.Normal;
                                                    result.tangent = float4(vertexDescription.Tangent, 0);
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                    #endif
                                                }

                                                void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                                                {
                                                    result.pos = varyings.positionCS;
                                                    // World Tangent isn't an available input on v2f_surf


                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                    #endif
                                                    #if UNITY_SHOULD_SAMPLE_SH
                                                    #endif
                                                    #if defined(LIGHTMAP_ON)
                                                    #endif
                                                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                        result.fogCoord = varyings.fogFactorAndVertexLight.x;
                                                        COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                                                    #endif

                                                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                                                }

                                                void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                                                {
                                                    result.positionCS = surfVertex.pos;
                                                    // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                                                    // World Tangent isn't an available input on v2f_surf

                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                    #endif
                                                    #if UNITY_SHOULD_SAMPLE_SH
                                                    #endif
                                                    #if defined(LIGHTMAP_ON)
                                                    #endif
                                                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                        result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                                                        COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                                                    #endif

                                                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                                                }

                                                // --------------------------------------------------
                                                // Main

                                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

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
                                                    #pragma target 3.0
                                                    #pragma vertex vert
                                                    #pragma fragment frag

                                                    // DotsInstancingOptions: <None>
                                                    // HybridV1InjectedBuiltinProperties: <None>

                                                    // Keywords
                                                    //#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                                                    // GraphKeywords: <None>

                                                    // Defines
                                                    #define _NORMALMAP 1
                                                    #define _NORMAL_DROPOFF_TS 1
                                                    #define ATTRIBUTES_NEED_NORMAL
                                                    #define ATTRIBUTES_NEED_TANGENT
                                                    #define ATTRIBUTES_NEED_TEXCOORD0
                                                    #define ATTRIBUTES_NEED_TEXCOORD1
                                                    #define ATTRIBUTES_NEED_TEXCOORD2
                                                    #define VARYINGS_NEED_TEXCOORD0
                                                    #define VARYINGS_NEED_TEXCOORD1
                                                    #define FEATURES_GRAPH_VERTEX
                                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                    #define SHADERPASS SHADERPASS_META
                                                    #define BUILTIN_TARGET_API 1
                                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
                                                    #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                                    #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                                    #endif
                                                    #ifdef _BUILTIN_ALPHATEST_ON
                                                    #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
                                                    #endif
                                                    #ifdef _BUILTIN_AlphaClip
                                                    #define _AlphaClip _BUILTIN_AlphaClip
                                                    #endif
                                                    #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
                                                    #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
                                                    #endif


                                                    // custom interpolator pre-include
                                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                    // Includes
                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                                    // --------------------------------------------------
                                                    // Structs and Packing

                                                    // custom interpolators pre packing
                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

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
                                                         float4 texCoord0;
                                                         float4 texCoord1;
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
                                                         float4 uv0;
                                                         float4 uv1;
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
                                                         float4 interp0 : INTERP0;
                                                         float4 interp1 : INTERP1;
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

                                                    PackedVaryings PackVaryings(Varyings input)
                                                    {
                                                        PackedVaryings output;
                                                        ZERO_INITIALIZE(PackedVaryings, output);
                                                        output.positionCS = input.positionCS;
                                                        output.interp0.xyzw = input.texCoord0;
                                                        output.interp1.xyzw = input.texCoord1;
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

                                                    Varyings UnpackVaryings(PackedVaryings input)
                                                    {
                                                        Varyings output;
                                                        output.positionCS = input.positionCS;
                                                        output.texCoord0 = input.interp0.xyzw;
                                                        output.texCoord1 = input.interp1.xyzw;
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
                                                    float4 _MainTex_TexelSize;
                                                    float4 _BumpMap_TexelSize;
                                                    float4 _SpecGlossMap_TexelSize;
                                                    float4 _OcclusionMap_TexelSize;
                                                    half _OcclusionIntencity;
                                                    CBUFFER_END

                                                        // Object and Global properties
                                                        SAMPLER(SamplerState_Linear_Repeat);
                                                        TEXTURE2D(_MainTex);
                                                        SAMPLER(sampler_MainTex);
                                                        TEXTURE2D(_BumpMap);
                                                        SAMPLER(sampler_BumpMap);
                                                        TEXTURE2D(_SpecGlossMap);
                                                        SAMPLER(sampler_SpecGlossMap);
                                                        TEXTURE2D(_OcclusionMap);
                                                        SAMPLER(sampler_OcclusionMap);

                                                        // -- Property used by ScenePickingPass
                                                        #ifdef SCENEPICKINGPASS
                                                        float4 _SelectionID;
                                                        #endif

                                                        // -- Properties used by SceneSelectionPass
                                                        #ifdef SCENESELECTIONPASS
                                                        int _ObjectId;
                                                        int _PassValue;
                                                        #endif

                                                        // Graph Includes
                                                        // GraphIncludes: <None>

                                                        // Graph Functions

                                                        void Unity_Blend_Multiply_half4(half4 Base, half4 Blend, out half4 Out, half Opacity)
                                                        {
                                                            Out = Base * Blend;
                                                            Out = lerp(Base, Out, Opacity);
                                                        }

                                                        void Unity_Saturate_half4(half4 In, out half4 Out)
                                                        {
                                                            Out = saturate(In);
                                                        }

                                                        // Custom interpolators pre vertex
                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                        // Graph Vertex
                                                        struct VertexDescription
                                                        {
                                                            half3 Position;
                                                            half3 Normal;
                                                            half3 Tangent;
                                                        };

                                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                        {
                                                            VertexDescription description = (VertexDescription)0;
                                                            description.Position = IN.ObjectSpacePosition;
                                                            description.Normal = IN.ObjectSpaceNormal;
                                                            description.Tangent = IN.ObjectSpaceTangent;
                                                            return description;
                                                        }

                                                        // Custom interpolators, pre surface
                                                        #ifdef FEATURES_GRAPH_VERTEX
                                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                        {
                                                        return output;
                                                        }
                                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                        #endif

                                                        // Graph Pixel
                                                        struct SurfaceDescription
                                                        {
                                                            half3 BaseColor;
                                                            half3 Emission;
                                                        };

                                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                        {
                                                            SurfaceDescription surface = (SurfaceDescription)0;
                                                            UnityTexture2D _Property_10fe7d19df084ea49913608ddae8394c_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                                            half4 _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_10fe7d19df084ea49913608ddae8394c_Out_0.tex, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.samplerstate, _Property_10fe7d19df084ea49913608ddae8394c_Out_0.GetTransformedUV(IN.uv0.xy));
                                                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_R_4 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.r;
                                                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_G_5 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.g;
                                                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_B_6 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.b;
                                                            half _SampleTexture2D_27b5932900db428985068236a13a9de2_A_7 = _SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0.a;
                                                            UnityTexture2D _Property_770e05d7e80443d7a75127092eb0c28e_Out_0 = UnityBuildTexture2DStructNoScale(_OcclusionMap);
                                                            half4 _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_770e05d7e80443d7a75127092eb0c28e_Out_0.tex, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.samplerstate, _Property_770e05d7e80443d7a75127092eb0c28e_Out_0.GetTransformedUV(IN.uv1.xy));
                                                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_R_4 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.r;
                                                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_G_5 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.g;
                                                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_B_6 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.b;
                                                            half _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_A_7 = _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0.a;
                                                            half _Property_aea6ee9cf47244888e2362762a4060d5_Out_0 = _OcclusionIntencity;
                                                            half4 _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2;
                                                            Unity_Blend_Multiply_half4(_SampleTexture2D_27b5932900db428985068236a13a9de2_RGBA_0, _SampleTexture2D_7bca1c1295314b2f8ed717919611599e_RGBA_0, _Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Property_aea6ee9cf47244888e2362762a4060d5_Out_0);
                                                            half4 _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1;
                                                            Unity_Saturate_half4(_Blend_f7335d0c17644e21a29ff5cb0d855afc_Out_2, _Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1);
                                                            surface.BaseColor = (_Saturate_f48c98215bc641a8a683c05c58eb8c0a_Out_1.xyz);
                                                            surface.Emission = half3(0, 0, 0);
                                                            return surface;
                                                        }

                                                        // --------------------------------------------------
                                                        // Build Graph Inputs

                                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                        {
                                                            VertexDescriptionInputs output;
                                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                            output.ObjectSpaceNormal = input.normalOS;
                                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                            output.ObjectSpacePosition = input.positionOS;

                                                            return output;
                                                        }
                                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                        {
                                                            SurfaceDescriptionInputs output;
                                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







                                                            output.uv0 = input.texCoord0;
                                                            output.uv1 = input.texCoord1;
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                        #else
                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                        #endif
                                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                return output;
                                                        }

                                                        void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                                                        {
                                                            result.vertex = float4(attributes.positionOS, 1);
                                                            result.tangent = attributes.tangentOS;
                                                            result.normal = attributes.normalOS;
                                                            result.texcoord = attributes.uv0;
                                                            result.texcoord1 = attributes.uv1;
                                                            result.texcoord2 = attributes.uv2;
                                                            result.vertex = float4(vertexDescription.Position, 1);
                                                            result.normal = vertexDescription.Normal;
                                                            result.tangent = float4(vertexDescription.Tangent, 0);
                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                            #endif
                                                        }

                                                        void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                                                        {
                                                            result.pos = varyings.positionCS;
                                                            // World Tangent isn't an available input on v2f_surf


                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                            #endif
                                                            #if UNITY_SHOULD_SAMPLE_SH
                                                            #endif
                                                            #if defined(LIGHTMAP_ON)
                                                            #endif
                                                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                                result.fogCoord = varyings.fogFactorAndVertexLight.x;
                                                                COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                                                            #endif

                                                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                                                        }

                                                        void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                                                        {
                                                            result.positionCS = surfVertex.pos;
                                                            // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                                                            // World Tangent isn't an available input on v2f_surf

                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                            #endif
                                                            #if UNITY_SHOULD_SAMPLE_SH
                                                            #endif
                                                            #if defined(LIGHTMAP_ON)
                                                            #endif
                                                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                                result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                                                                COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                                                            #endif

                                                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                                                        }

                                                        // --------------------------------------------------
                                                        // Main

                                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

                                                        ENDHLSL
                                                        }
                                                        Pass
                                                        {
                                                            Name "SceneSelectionPass"
                                                            Tags
                                                            {
                                                                "LightMode" = "SceneSelectionPass"
                                                            }

                                                            // Render State
                                                            Cull Off

                                                            // Debug
                                                            // <None>

                                                            // --------------------------------------------------
                                                            // Pass

                                                            HLSLPROGRAM

                                                            // Pragmas
                                                            #pragma target 3.0
                                                            //#pragma multi_compile_instancing
                                                            #pragma vertex vert
                                                            #pragma fragment frag

                                                            // DotsInstancingOptions: <None>
                                                            // HybridV1InjectedBuiltinProperties: <None>

                                                            // Keywords
                                                            // PassKeywords: <None>
                                                            // GraphKeywords: <None>

                                                            // Defines
                                                            #define _NORMALMAP 1
                                                            #define _NORMAL_DROPOFF_TS 1
                                                            #define ATTRIBUTES_NEED_NORMAL
                                                            #define ATTRIBUTES_NEED_TANGENT
                                                            #define FEATURES_GRAPH_VERTEX
                                                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                            #define SHADERPASS SceneSelectionPass
                                                            #define BUILTIN_TARGET_API 1
                                                            #define SCENESELECTIONPASS 1
                                                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
                                                            #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                                            #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                                            #endif
                                                            #ifdef _BUILTIN_ALPHATEST_ON
                                                            #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
                                                            #endif
                                                            #ifdef _BUILTIN_AlphaClip
                                                            #define _AlphaClip _BUILTIN_AlphaClip
                                                            #endif
                                                            #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
                                                            #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
                                                            #endif


                                                            // custom interpolator pre-include
                                                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                            // Includes
                                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
                                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
                                                            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                                            // --------------------------------------------------
                                                            // Structs and Packing

                                                            // custom interpolators pre packing
                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                            struct Attributes
                                                            {
                                                                 float3 positionOS : POSITION;
                                                                 float3 normalOS : NORMAL;
                                                                 float4 tangentOS : TANGENT;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                 uint instanceID : INSTANCEID_SEMANTIC;
                                                                #endif
                                                            };
                                                            struct Varyings
                                                            {
                                                                 float4 positionCS : SV_POSITION;
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

                                                            PackedVaryings PackVaryings(Varyings input)
                                                            {
                                                                PackedVaryings output;
                                                                ZERO_INITIALIZE(PackedVaryings, output);
                                                                output.positionCS = input.positionCS;
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

                                                            Varyings UnpackVaryings(PackedVaryings input)
                                                            {
                                                                Varyings output;
                                                                output.positionCS = input.positionCS;
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
                                                            float4 _MainTex_TexelSize;
                                                            float4 _BumpMap_TexelSize;
                                                            float4 _SpecGlossMap_TexelSize;
                                                            float4 _OcclusionMap_TexelSize;
                                                            half _OcclusionIntencity;
                                                            CBUFFER_END

                                                                // Object and Global properties
                                                                SAMPLER(SamplerState_Linear_Repeat);
                                                                TEXTURE2D(_MainTex);
                                                                SAMPLER(sampler_MainTex);
                                                                TEXTURE2D(_BumpMap);
                                                                SAMPLER(sampler_BumpMap);
                                                                TEXTURE2D(_SpecGlossMap);
                                                                SAMPLER(sampler_SpecGlossMap);
                                                                TEXTURE2D(_OcclusionMap);
                                                                SAMPLER(sampler_OcclusionMap);

                                                                // -- Property used by ScenePickingPass
                                                                #ifdef SCENEPICKINGPASS
                                                                float4 _SelectionID;
                                                                #endif

                                                                // -- Properties used by SceneSelectionPass
                                                                #ifdef SCENESELECTIONPASS
                                                                int _ObjectId;
                                                                int _PassValue;
                                                                #endif

                                                                // Graph Includes
                                                                // GraphIncludes: <None>

                                                                // Graph Functions
                                                                // GraphFunctions: <None>

                                                                // Custom interpolators pre vertex
                                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                // Graph Vertex
                                                                struct VertexDescription
                                                                {
                                                                    half3 Position;
                                                                    half3 Normal;
                                                                    half3 Tangent;
                                                                };

                                                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                {
                                                                    VertexDescription description = (VertexDescription)0;
                                                                    description.Position = IN.ObjectSpacePosition;
                                                                    description.Normal = IN.ObjectSpaceNormal;
                                                                    description.Tangent = IN.ObjectSpaceTangent;
                                                                    return description;
                                                                }

                                                                // Custom interpolators, pre surface
                                                                #ifdef FEATURES_GRAPH_VERTEX
                                                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                {
                                                                return output;
                                                                }
                                                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                #endif

                                                                // Graph Pixel
                                                                struct SurfaceDescription
                                                                {
                                                                };

                                                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                {
                                                                    SurfaceDescription surface = (SurfaceDescription)0;
                                                                    return surface;
                                                                }

                                                                // --------------------------------------------------
                                                                // Build Graph Inputs

                                                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                {
                                                                    VertexDescriptionInputs output;
                                                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                    output.ObjectSpaceNormal = input.normalOS;
                                                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                    output.ObjectSpacePosition = input.positionOS;

                                                                    return output;
                                                                }
                                                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                {
                                                                    SurfaceDescriptionInputs output;
                                                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                #else
                                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                #endif
                                                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                        return output;
                                                                }

                                                                void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                                                                {
                                                                    result.vertex = float4(attributes.positionOS, 1);
                                                                    result.tangent = attributes.tangentOS;
                                                                    result.normal = attributes.normalOS;
                                                                    result.vertex = float4(vertexDescription.Position, 1);
                                                                    result.normal = vertexDescription.Normal;
                                                                    result.tangent = float4(vertexDescription.Tangent, 0);
                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                    #endif
                                                                }

                                                                void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                                                                {
                                                                    result.pos = varyings.positionCS;
                                                                    // World Tangent isn't an available input on v2f_surf


                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                    #endif
                                                                    #if UNITY_SHOULD_SAMPLE_SH
                                                                    #endif
                                                                    #if defined(LIGHTMAP_ON)
                                                                    #endif
                                                                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                                        result.fogCoord = varyings.fogFactorAndVertexLight.x;
                                                                        COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                                                                    #endif

                                                                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                                                                }

                                                                void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                                                                {
                                                                    result.positionCS = surfVertex.pos;
                                                                    // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                                                                    // World Tangent isn't an available input on v2f_surf

                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                    #endif
                                                                    #if UNITY_SHOULD_SAMPLE_SH
                                                                    #endif
                                                                    #if defined(LIGHTMAP_ON)
                                                                    #endif
                                                                    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                                        result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                                                                        COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                                                                    #endif

                                                                    DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                                                                }

                                                                // --------------------------------------------------
                                                                // Main

                                                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                                                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

                                                                ENDHLSL
                                                                }
                                                                Pass
                                                                {
                                                                    Name "ScenePickingPass"
                                                                    Tags
                                                                    {
                                                                        "LightMode" = "Picking"
                                                                    }

                                                                    // Render State
                                                                    Cull Back

                                                                    // Debug
                                                                    // <None>

                                                                    // --------------------------------------------------
                                                                    // Pass

                                                                    HLSLPROGRAM

                                                                    // Pragmas
                                                                    #pragma target 3.0
                                                                    //#pragma multi_compile_instancing
                                                                    #pragma vertex vert
                                                                    #pragma fragment frag

                                                                    // DotsInstancingOptions: <None>
                                                                    // HybridV1InjectedBuiltinProperties: <None>

                                                                    // Keywords
                                                                    // PassKeywords: <None>
                                                                    // GraphKeywords: <None>

                                                                    // Defines
                                                                    #define _NORMALMAP 1
                                                                    #define _NORMAL_DROPOFF_TS 1
                                                                    #define ATTRIBUTES_NEED_NORMAL
                                                                    #define ATTRIBUTES_NEED_TANGENT
                                                                    #define FEATURES_GRAPH_VERTEX
                                                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                    #define SHADERPASS ScenePickingPass
                                                                    #define BUILTIN_TARGET_API 1
                                                                    #define SCENEPICKINGPASS 1
                                                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
                                                                    #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                                                    #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
                                                                    #endif
                                                                    #ifdef _BUILTIN_ALPHATEST_ON
                                                                    #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
                                                                    #endif
                                                                    #ifdef _BUILTIN_AlphaClip
                                                                    #define _AlphaClip _BUILTIN_AlphaClip
                                                                    #endif
                                                                    #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
                                                                    #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
                                                                    #endif


                                                                    // custom interpolator pre-include
                                                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                    // Includes
                                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
                                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
                                                                    #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                                                    // --------------------------------------------------
                                                                    // Structs and Packing

                                                                    // custom interpolators pre packing
                                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                    struct Attributes
                                                                    {
                                                                         float3 positionOS : POSITION;
                                                                         float3 normalOS : NORMAL;
                                                                         float4 tangentOS : TANGENT;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                         uint instanceID : INSTANCEID_SEMANTIC;
                                                                        #endif
                                                                    };
                                                                    struct Varyings
                                                                    {
                                                                         float4 positionCS : SV_POSITION;
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

                                                                    PackedVaryings PackVaryings(Varyings input)
                                                                    {
                                                                        PackedVaryings output;
                                                                        ZERO_INITIALIZE(PackedVaryings, output);
                                                                        output.positionCS = input.positionCS;
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

                                                                    Varyings UnpackVaryings(PackedVaryings input)
                                                                    {
                                                                        Varyings output;
                                                                        output.positionCS = input.positionCS;
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
                                                                    float4 _MainTex_TexelSize;
                                                                    float4 _BumpMap_TexelSize;
                                                                    float4 _SpecGlossMap_TexelSize;
                                                                    float4 _OcclusionMap_TexelSize;
                                                                    half _OcclusionIntencity;
                                                                    CBUFFER_END

                                                                        // Object and Global properties
                                                                        SAMPLER(SamplerState_Linear_Repeat);
                                                                        TEXTURE2D(_MainTex);
                                                                        SAMPLER(sampler_MainTex);
                                                                        TEXTURE2D(_BumpMap);
                                                                        SAMPLER(sampler_BumpMap);
                                                                        TEXTURE2D(_SpecGlossMap);
                                                                        SAMPLER(sampler_SpecGlossMap);
                                                                        TEXTURE2D(_OcclusionMap);
                                                                        SAMPLER(sampler_OcclusionMap);

                                                                        // -- Property used by ScenePickingPass
                                                                        #ifdef SCENEPICKINGPASS
                                                                        float4 _SelectionID;
                                                                        #endif

                                                                        // -- Properties used by SceneSelectionPass
                                                                        #ifdef SCENESELECTIONPASS
                                                                        int _ObjectId;
                                                                        int _PassValue;
                                                                        #endif

                                                                        // Graph Includes
                                                                        // GraphIncludes: <None>

                                                                        // Graph Functions
                                                                        // GraphFunctions: <None>

                                                                        // Custom interpolators pre vertex
                                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                        // Graph Vertex
                                                                        struct VertexDescription
                                                                        {
                                                                            half3 Position;
                                                                            half3 Normal;
                                                                            half3 Tangent;
                                                                        };

                                                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                        {
                                                                            VertexDescription description = (VertexDescription)0;
                                                                            description.Position = IN.ObjectSpacePosition;
                                                                            description.Normal = IN.ObjectSpaceNormal;
                                                                            description.Tangent = IN.ObjectSpaceTangent;
                                                                            return description;
                                                                        }

                                                                        // Custom interpolators, pre surface
                                                                        #ifdef FEATURES_GRAPH_VERTEX
                                                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                        {
                                                                        return output;
                                                                        }
                                                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                        #endif

                                                                        // Graph Pixel
                                                                        struct SurfaceDescription
                                                                        {
                                                                        };

                                                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                        {
                                                                            SurfaceDescription surface = (SurfaceDescription)0;
                                                                            return surface;
                                                                        }

                                                                        // --------------------------------------------------
                                                                        // Build Graph Inputs

                                                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                        {
                                                                            VertexDescriptionInputs output;
                                                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                            output.ObjectSpaceNormal = input.normalOS;
                                                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                            output.ObjectSpacePosition = input.positionOS;

                                                                            return output;
                                                                        }
                                                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                        {
                                                                            SurfaceDescriptionInputs output;
                                                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                        #else
                                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                        #endif
                                                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                return output;
                                                                        }

                                                                        void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
                                                                        {
                                                                            result.vertex = float4(attributes.positionOS, 1);
                                                                            result.tangent = attributes.tangentOS;
                                                                            result.normal = attributes.normalOS;
                                                                            result.vertex = float4(vertexDescription.Position, 1);
                                                                            result.normal = vertexDescription.Normal;
                                                                            result.tangent = float4(vertexDescription.Tangent, 0);
                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                            #endif
                                                                        }

                                                                        void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
                                                                        {
                                                                            result.pos = varyings.positionCS;
                                                                            // World Tangent isn't an available input on v2f_surf


                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                            #endif
                                                                            #if UNITY_SHOULD_SAMPLE_SH
                                                                            #endif
                                                                            #if defined(LIGHTMAP_ON)
                                                                            #endif
                                                                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                                                result.fogCoord = varyings.fogFactorAndVertexLight.x;
                                                                                COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                                                                            #endif

                                                                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
                                                                        }

                                                                        void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
                                                                        {
                                                                            result.positionCS = surfVertex.pos;
                                                                            // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                                                                            // World Tangent isn't an available input on v2f_surf

                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                            #endif
                                                                            #if UNITY_SHOULD_SAMPLE_SH
                                                                            #endif
                                                                            #if defined(LIGHTMAP_ON)
                                                                            #endif
                                                                            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                                                result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                                                                                COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                                                                            #endif

                                                                            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
                                                                        }

                                                                        // --------------------------------------------------
                                                                        // Main

                                                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                                                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

                                                                        ENDHLSL
                                                                        }
        }
            CustomEditorForRenderPipeline "UnityEditor.Rendering.BuiltIn.ShaderGraph.BuiltInLitGUI" ""
                                                                            CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                                                            FallBack "Hidden/Shader Graph/FallbackError"
}