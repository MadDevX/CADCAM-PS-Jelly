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

vec3 tangent(vec3 a, vec3 b, vec3 c, vec3 d, float t)
{
    return normalize(bezier3(b, c, d, t) - bezier3(a, b, c, t));
}

vec2 bezier2(vec2 a, vec2 b, float t) 
{
   return mix(a, b, t);
}

vec2 bezier3(vec2 a, vec2 b, vec2 c, float t) 
{
   return mix(bezier2(a, b, t), bezier2(b, c, t), t);
}

vec2 bezier4(vec2 a, vec2 b, vec2 c, vec2 d, float t) 
{
   return mix(bezier3(a, b, c, t), bezier3(b, c, d, t), t);
}

layout(quads, fractional_odd_spacing) in;

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

   vec3 v1 = bezier4(gl_in[0].gl_Position.xyz, gl_in[1].gl_Position.xyz, gl_in[2].gl_Position.xyz, gl_in[3].gl_Position.xyz, v);
   vec3 v2 = bezier4(gl_in[4].gl_Position.xyz, gl_in[5].gl_Position.xyz, gl_in[6].gl_Position.xyz, gl_in[7].gl_Position.xyz, v);
   vec3 v3 = bezier4(gl_in[8].gl_Position.xyz, gl_in[9].gl_Position.xyz, gl_in[10].gl_Position.xyz, gl_in[11].gl_Position.xyz, v);
   vec3 v4 = bezier4(gl_in[12].gl_Position.xyz, gl_in[13].gl_Position.xyz, gl_in[14].gl_Position.xyz, gl_in[15].gl_Position.xyz, v);
   vec3 pos = bezier4(v1, v2, v3, v4, u);

   vec2 tv1 = bezier4(TexCoordsTesc[0], TexCoordsTesc[1], TexCoordsTesc[2], TexCoordsTesc[3], v);
   vec2 tv2 = bezier4(TexCoordsTesc[4], TexCoordsTesc[5], TexCoordsTesc[6], TexCoordsTesc[7], v);
   vec2 tv3 = bezier4(TexCoordsTesc[8], TexCoordsTesc[9], TexCoordsTesc[10], TexCoordsTesc[11], v);
   vec2 tv4 = bezier4(TexCoordsTesc[12], TexCoordsTesc[13], TexCoordsTesc[14], TexCoordsTesc[15], v);
   TexCoords = bezier4(tv1, tv2, tv3, tv4, u);

   vec3 tangentU = tangent(v1, v2, v3, v4, u);

   vec3 nv1V = bezier4(gl_in[0].gl_Position.xyz, gl_in[4].gl_Position.xyz, gl_in[8].gl_Position.xyz, gl_in[12].gl_Position.xyz, u);
   vec3 nv2V = bezier4(gl_in[1].gl_Position.xyz, gl_in[5].gl_Position.xyz, gl_in[9].gl_Position.xyz, gl_in[13].gl_Position.xyz, u);
   vec3 nv3V = bezier4(gl_in[2].gl_Position.xyz, gl_in[6].gl_Position.xyz, gl_in[10].gl_Position.xyz, gl_in[14].gl_Position.xyz, u);
   vec3 nv4V = bezier4(gl_in[3].gl_Position.xyz, gl_in[7].gl_Position.xyz, gl_in[11].gl_Position.xyz, gl_in[15].gl_Position.xyz, u);
   vec3 tangentV = normalize(tangent(nv1V, nv2V, nv3V, nv4V, v));
   Normal = normalize(cross(tangentU, tangentV));

   FragPos = (model * vec4(pos, 1.0f)).xyz;
   ViewVec = camPos - FragPos;

   gl_Position = projection * view * vec4(FragPos, 1.0f);
}