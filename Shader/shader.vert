// ini dulu baru frag
#version 330 core

// nyimpennya layout (location = x) itu angka yang kita pakai buat start vertex
layout(location = 0) in vec3 aPositon;

//layout(location = 1) in vec3 vColors;

//menyediakan variable yang bisa dikirim ke next step -> .frag
out vec4 vertexcolor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main (void)
{
//	gl_Position = vec4(aPositon,1.0);
	gl_Position = vec4(aPositon,1.0) * model * view * projection;

//	vertexcolor = vec4(vColors,1.0);
	vertexcolor = vec4(0.32,0.57,0.44,1.0);
}