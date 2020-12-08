#version 400 core

layout (vertices = 16) out; // 16 points per patch

uniform int tessLevelOuter;
uniform int tessLevelInner;

uniform vec3 camPos;

in vec2 TexCoordsVert[];
in vec3 NormalVert[];

out vec2 TexCoordsTesc[];
out vec3 NormalTesc[];

const float invLog = 1.0f / log(10);

float log10(float a)
{
    return invLog * log(a);
}

float tessFactor(float dist)
{
    return -1.0f * log10(dist * 0.03f);
}

void main() 
{
   gl_out[gl_InvocationID].gl_Position = gl_in[gl_InvocationID].gl_Position;
   if(gl_InvocationID == 0) // levels only need to be set once per patch
   { 
       vec3 top =    mix(gl_in[12].gl_Position.xyz, gl_in[15].gl_Position.xyz, 0.5f);
       vec3 bottom = mix(gl_in[0]. gl_Position.xyz, gl_in[3]. gl_Position.xyz, 0.5f);
       vec3 left =   mix(gl_in[0]. gl_Position.xyz, gl_in[12].gl_Position.xyz, 0.5f);
       vec3 right =  mix(gl_in[3]. gl_Position.xyz, gl_in[15].gl_Position.xyz, 0.5f);
       vec3 mid = mix(top, bottom, 0.5f);
       
       float topDist =    sqrt(dot((top - camPos),     (top - camPos)));
       float bottomDist = sqrt(dot((bottom - camPos),  (bottom - camPos)));
       float leftDist =   sqrt(dot((left - camPos),    (left - camPos)));
       float rightDist =  sqrt(dot((right - camPos),   (right - camPos)));
       float midDist =    sqrt(dot((mid - camPos),     (mid - camPos)));

       gl_TessLevelOuter[2] = max(1.0f, tessLevelOuter * tessFactor(topDist));
       gl_TessLevelOuter[1] = max(1.0f, tessLevelOuter * tessFactor(leftDist));
       gl_TessLevelOuter[0] = max(1.0f, tessLevelOuter * tessFactor(bottomDist));
       gl_TessLevelOuter[3] = max(1.0f, tessLevelOuter * tessFactor(rightDist));
       gl_TessLevelInner[0] = max(1.0f, tessLevelInner * tessFactor(midDist));
       gl_TessLevelInner[1] = max(1.0f, tessLevelInner * tessFactor(midDist));

   }
   TexCoordsTesc[gl_InvocationID] = TexCoordsVert[gl_InvocationID];
   NormalTesc[gl_InvocationID] = NormalVert[gl_InvocationID];
}