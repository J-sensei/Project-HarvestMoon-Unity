#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void CalculateMainLight_float(float3 WorldPos, out float3 Direction, out float3 Color) {
#if SHADERGRAPH_PREVIEW
	Direction = float3(0.5, 0.5, 0);
	Color = 1;
#else
	Light mainLight = GetMainLight(0);
	Direction = mainLight.direction;
	Color = mainLight.color;
#endif
}

#endif

void GetLightingInformation_float(out float3 Direction, out float3 Color, out float Attenuation)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = float3(-0.5, 0.5, -0.5);
    Color = float3(1, 1, 1);
    Attenuation = 0.4;
#else
    Light light = GetMainLight();
    Direction = light.direction;
    Attenuation = light.distanceAttenuation;
    Color = light.color;
#endif
}