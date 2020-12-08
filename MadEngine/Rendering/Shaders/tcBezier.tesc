#version 400 core

layout (vertices = 16) out; // 16 points per patch

uniform int tessLevelOuter;
uniform int tessLevelInner;

in vec2 TexCoordsVert[];
in vec3 NormalVert[];

out vec2 TexCoordsTesc[];
out vec3 NormalTesc[];

void main() 
{
   gl_out[gl_InvocationID].gl_Position = gl_in[gl_InvocationID].gl_Position;
   if(gl_InvocationID == 0) // levels only need to be set once per patch
   { 
       gl_TessLevelOuter[0] = tessLevelOuter;
       gl_TessLevelOuter[1] = tessLevelOuter;
       gl_TessLevelOuter[2] = tessLevelOuter;
       gl_TessLevelOuter[3] = tessLevelOuter;
       gl_TessLevelInner[0] = tessLevelInner;
       gl_TessLevelInner[1] = tessLevelInner;
   }
   TexCoordsTesc[gl_InvocationID] = TexCoordsVert[gl_InvocationID];
   NormalTesc[gl_InvocationID] = NormalVert[gl_InvocationID];
}