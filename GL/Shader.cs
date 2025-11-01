using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.IO;

namespace Game.Engine;

public class Shader : IDisposable
{
    private readonly int _program;

    public Shader(string vertexPath, string fragmentPath)
    {
        int vertex = Compile(ShaderType.VertexShader, File.ReadAllText(vertexPath));
        int fragment = Compile(ShaderType.FragmentShader, File.ReadAllText(fragmentPath));

        _program = GL.CreateProgram();
        GL.AttachShader(_program, vertex);
        GL.AttachShader(_program, fragment);
        GL.LinkProgram(_program);

        GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out int status);
        if (status == 0)
            throw new Exception(GL.GetProgramInfoLog(_program));

        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    private static int Compile(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);

        if (status == 0)
            throw new Exception(GL.GetShaderInfoLog(shader));

        return shader;
    }

    public void Use() => GL.UseProgram(_program);

    private int GetLocation(string name) => GL.GetUniformLocation(_program, name);

    public void SetMatrix4(string name, Matrix4 matrix)
        => GL.UniformMatrix4(GetLocation(name), false, ref matrix);

    public void SetVector3(string name, Vector3 value)
        => GL.Uniform3(GetLocation(name), value);

    public void SetInt(string name, int value)
        => GL.Uniform1(GetLocation(name), value);

    public void Dispose() => GL.DeleteProgram(_program);
}
