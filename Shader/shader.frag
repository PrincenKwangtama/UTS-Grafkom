#version 330 

out vec4 output_color;

in vec4 vertexcolor; //  variable in dan out harus sama

uniform vec4 unicolor;

void main (){
//	output_color = vec4(1.0,0.30,0.40,1.0); // ini satu warna
//	output_color = vertexcolor; // ini rgb
	output_color = unicolor;
}

