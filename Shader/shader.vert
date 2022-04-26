#version 330 core

layout(location = 0) in vec3 aPositon;

//layout(location = 1) in vec3 aColors;

//var yg dipasing ke .frag
out vec4 vertexColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main (void)
{
    gl_Position = vec4(aPositon,1.0) * model * view * projection;

    vertexColor = vec4(0.0,1.0,0.0,1.0);
}
