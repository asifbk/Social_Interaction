#include "UnityCG.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityLightingCommonRero.cginc"

//-----------------------------------------------------------------------------
// Helper to convert smoothness to roughness
//-----------------------------------------------------------------------------

float3 ShadeSH9( float3 normal )
{
	return ShadeSH9(half4(normal, 1.0));
}

float smootherstep(float edge0, float edge1, float x) 
{
	// Scale, and clamp x to 0..1 range
	x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
	// Evaluate polynomial
	return x * x * x * (x * (x * 6 - 15) + 10);
}

bool IsInMirror()
{
  return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f;
}

half4 BRDF_Unity_Toon(half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
    float3 normal, float3 viewDir,
    UnityLight light, UnityIndirect gi)
{
	//Determine the lighting scenario
	float3 brightness = ShadeSH9(float4(0,0,0,1)) + light.color;
	float worldBrightness = saturate((brightness.r + brightness.g + brightness.b)/3);
	float reflBrightness = 1;
	atten = smootherstep(.9999 - _RampSmooth, 1, worldBrightness);
	float nAtten = pow(1-atten, 5);
    atten = saturate(atten + (1-nAtten));

	if (any(_WorldSpaceLightPos0)==0)
	{
		if(_FakeLight.w <= 0)
		{_FakeLight.w =0;}
		reflBrightness = worldBrightness;
		light.color = ShadeSH9(normal) * _FakeLight.w;
		light.dir = Lightprobe_vec();
		if (any(light.dir)==0)
		{light.dir = Unity_SafeNormalize(_FakeLight.xyz);}
		atten = 1;
	}

	#ifdef USING_STEREO_MATRICES
	_WorldSpaceCameraPos = lerp(unity_StereoWorldSpaceCameraPos[0], unity_StereoWorldSpaceCameraPos[1], 0.5);
	viewDir = Unity_SafeNormalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
	#endif
	if (IsInMirror())
	{viewDir = Unity_SafeNormalize(viewDir);}
	
    float perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
    float3 halfDir = Unity_SafeNormalize (float3(light.dir) + viewDir);

    half nv = abs(dot(normal, viewDir));    // This abs allow to limit artifact

	half nl = smootherstep(0.0, _RampSmooth, (dot(normal, light.dir) - _RampThreshold)) * atten;
	if (_RampSmooth == 1)
	{nl = saturate(dot(normal, light.dir) - _RampThreshold);}
    float nh = saturate(dot(normal, halfDir));

    half lv = saturate(dot(light.dir, viewDir));
    half lh = saturate(dot(light.dir, halfDir));

    // Diffuse term
    half diffuseTerm = DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;

    // Specular term
    // HACK: theoretically we should divide diffuseTerm by Pi and not multiply specularTerm!
    // BUT 1) that will make shader look significantly darker than Legacy ones
    // and 2) on engine side "Non-important" lights have to be divided by Pi too in cases when they are injected into ambient SH
    float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
#if UNITY_BRDF_GGX
    // GGX with roughtness to 0 would mean no specular at all, using max(roughness, 0.002) here to match HDrenderloop roughtness remapping.
    roughness = max(roughness, 0.002);
    float V = SmithJointGGXVisibilityTerm (nl, nv, roughness);
    float D = GGXTerm (nh, roughness);
#else
    // Legacy
    half V = SmithBeckmannVisibilityTerm (nl, nv, roughness);
    half D = NDFBlinnPhongNormalizedTerm (nh, PerceptualRoughnessToSpecPower(perceptualRoughness));
#endif

    float specularTerm = V*D * UNITY_PI; // Torrance-Sparrow model, Fresnel is applied later

#   ifdef UNITY_COLORSPACE_GAMMA
        specularTerm = sqrt(max(1e-4h, specularTerm));
#   endif

    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * nl);
#if defined(_SPECULARHIGHLIGHTS_OFF)
    specularTerm = 0.0;
#endif

    // surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(roughness^2+1)
    half surfaceReduction;
#   ifdef UNITY_COLORSPACE_GAMMA
        surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;      // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
#   else
        surfaceReduction = 1.0 / (roughness*roughness + 1.0);           // fade \in [0.5;1]
#   endif

    // To provide true Lambert lighting, we need to be able to kill specular completely.
    specularTerm *= any(specColor) ? 1.0 : 0.0;

    half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));
	if(_ShadowToggle == 0)
	{_ShadowColor.rgb = 1;}
	gi.diffuse *= float4(_ShadowColor.rgb, 0);
    half3 color =  diffColor * (gi.diffuse + light.color * diffuseTerm)
                    + specularTerm * light.color * FresnelTerm(specColor, lh)
                    + surfaceReduction * gi.specular * (FresnelLerp(specColor, grazingTerm, nv) * clamp(0, 1, reflBrightness * 100));

    return half4(color, 1);
}