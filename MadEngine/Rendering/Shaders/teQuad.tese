#version 400

vec3 bezier2(vec3 a, vec3 b, float t) 
{
   return mix(a, b, t);
}

vec3 bezier3(vec3 a, vec3 b, vec3 c, float t) 
{
   return mix(bezier2(a, b, t), bezier2(b, c, t), t);
}

vec3 bezier4(vec3 a, vec3 b, vec3 c, vec3 d, float t) 
{
   return mix(bezier3(a, b, c, t), bezier3(b, c, d, t), t);
}

layout(quads, equal_spacing) in;

in vec2 TexCoordsTesc[];
in vec3 NormalTesc[];

out vec2 TexCoords;
out vec3 Normal;
out vec3 FragPos;
out vec3 ViewVec;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform mat3 normal;

uniform vec3 camPos;

void main() {
   float u = gl_TessCoord.x;
   float v = gl_TessCoord.y;
   
   vec3 v1 = mix(gl_in[0].gl_Position.xyz, gl_in[1].gl_Position.xyz, v); 
   vec3 v2 = mix(gl_in[2].gl_Position.xyz, gl_in[3].gl_Position.xyz, v);
   vec3 pos = mix(v1, v2, u);

   vec2 texCoord1 = mix(TexCoordsTesc[0], TexCoordsTesc[1], v); 
   vec2 texCoord2 = mix(TexCoordsTesc[2], TexCoordsTesc[3], v);
   TexCoords = mix(texCoord1, texCoord2, u);

   vec3 normal1 = mix(NormalTesc[0], NormalTesc[1], v); 
   vec3 normal2 = mix(NormalTesc[2], NormalTesc[3], v);
   Normal = normalize(normal * mix(normal1, normal2, u));
   
   FragPos = (model * vec4(pos, 1.0f)).xyz;
   ViewVec = camPos - FragPos;

   gl_Position = projection * view * vec4(FragPos, 1.0f);
}