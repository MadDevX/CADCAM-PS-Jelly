#version 400
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoordsVert;
out vec3 NormalVert;

void main()
{
    TexCoordsVert = aTexCoords;
	NormalVert = aNormal;
    gl_Position = vec4(aPos, 1.0f);
}