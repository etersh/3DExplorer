#version 330 core
in vec3 vFragPos;
in vec3 vNormal;
in vec2 vUV;

out vec4 FragColor;

uniform sampler2D uTex;
uniform vec3 uViewPos;
uniform vec3 uLightDir;
uniform int uLightOn;

void main()
{
    vec3 albedo = texture(uTex, vUV).rgb;
    vec3 N = normalize(vNormal);
    vec3 L = normalize(-uLightDir);
    vec3 V = normalize(uViewPos - vFragPos);
    vec3 R = reflect(-L, N);

    float diff = max(dot(N, L), 0.0);
    float spec = pow(max(dot(V, R), 0.0), 32.0);

    vec3 ambient = 0.2 * albedo;
    vec3 diffuse = diff * albedo;
    vec3 specular = 0.3 * spec * vec3(1.0);

    vec3 color = (uLightOn == 1) ? (ambient + diffuse + specular) : ambient;
    FragColor = vec4(color, 1.0);
}
