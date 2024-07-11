﻿#version 330 core
layout (location = 0) in vec3 aPosition;

uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uViewMatrix * uProjectionMatrix;
}