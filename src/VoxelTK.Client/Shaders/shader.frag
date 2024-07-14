#version 330 core 

out vec4 FragColor;

in vec3 vPosition;

void main()
{
    // small value to mitigate precision issues
    float epsilon = 1e-5;

    // modulo to get the checkerboard pattern, adjusted for precision issues
    float x = abs(fract(vPosition.x + epsilon));
    float y = abs(fract(vPosition.y + epsilon));
    float z = abs(fract(vPosition.z + epsilon));

    FragColor = vec4(x, y, z, 1.0f);
}