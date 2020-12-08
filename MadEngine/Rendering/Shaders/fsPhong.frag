#version 400
out vec4 FragColor;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;
in vec3 ViewVec;

uniform vec4 color = vec4(1.0f, 1.0f, 1.0f, 1.0f);
uniform vec4 bgColor = vec4(0.157f, 0.157f, 0.157f, 1.0f);
uniform vec3 lightPos = vec3(0.0f, 0.5f, 0.0f);
uniform vec3 lightCol = vec3(1.0f, 1.0f, 1.0f);

float near = 0.01f; //if changed, also change in Camera.cs
float far = 100.0f; //if changed, also change in Camera.cs
float fogDistance = 90.0f;
float LinearizeDepth(float depth);

float kd = 0.5f, ks = 0.2f, m = 100.0f, ka = 0.1f;

void main()
{
	vec3 resultColor = vec3(0.0f, 0.0f, 0.0f);
	vec3 lightVec = normalize(lightPos - FragPos);
	vec3 halfVec = normalize(ViewVec + lightVec);

	vec3 normal = gl_FrontFacing == false ? -Normal : Normal;

	resultColor += lightCol * color.xyz * ka;
	resultColor += lightCol * color.xyz * kd * clamp(dot(normal, lightVec), 0.0f, 1.0f); //diffuse color
	
	if(dot(normal, lightVec) >= 0.0f)
	{
		float nh = dot(normal, halfVec);
		nh = clamp(nh, 0.0f, 1.0f);
		nh = pow(nh, m);
		nh *= ks;
		resultColor += lightCol * nh;
	}    
	float depth = min(LinearizeDepth(gl_FragCoord.z)/fogDistance, 1.0f);
	FragColor = vec4(mix(resultColor.rgb, bgColor.rgb, depth), 1.0f);
}


float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}