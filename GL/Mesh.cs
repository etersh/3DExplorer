using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;

namespace Game.Engine;

public class Mesh : IDisposable
{
    private readonly int _vao, _vbo, _ebo;
    private readonly int _indexCount;

    public Mesh(float[] vertices, uint[] indices)
    {
        _indexCount = indices.Length;
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        int stride = (3 + 3 + 2) * sizeof(float);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
        GL.BindVertexArray(0);
    }

    public static Mesh CreateTexturedQuadXZ(float size, float y)
    {
        float half = size * 0.5f;
        float[] vertices = {
            -half, y, -half,  0, 1, 0,  0, 0,
             half, y, -half,  0, 1, 0,  1, 0,
             half, y,  half,  0, 1, 0,  1, 1,
            -half, y,  half,  0, 1, 0,  0, 1,
        };
        uint[] indices = { 0, 1, 2, 0, 2, 3 };
        return new Mesh(vertices, indices);
    }

    public static Mesh CreateCube(float size = 1f)
    {
        float h = size * 0.5f;
        float[] vertices = CubeVertices(h);
        uint[] indices = CubeIndices();
        return new Mesh(vertices, indices);
    }

    public static Mesh CreateQuadXY(float size, float z)
    {
        float hs = size * 0.5f;
        float[] v = {
            -hs, -hs, z,  0, 0, 1,  0, 0,
             hs, -hs, z,  0, 0, 1,  1, 0,
             hs,  hs, z,  0, 0, 1,  1, 1,
            -hs,  hs, z,  0, 0, 1,  0, 1,
        };
        uint[] i = { 0, 1, 2, 0, 2, 3 };
        return new Mesh(v, i);
    }

    private static float[] CubeVertices(float h)
    {
        Vector3[] normals = {
            new(0, 0, 1), new(0, 0, -1), new(-1, 0, 0),
            new(1, 0, 0), new(0, 1, 0), new(0, -1, 0)
        };

        var faces = new (Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 n)[]
        {
            (new(-h,-h, h), new( h,-h, h), new( h, h, h), new(-h, h, h), normals[0]),
            (new( h,-h,-h), new(-h,-h,-h), new(-h, h,-h), new( h, h,-h), normals[1]),
            (new(-h,-h,-h), new(-h,-h, h), new(-h, h, h), new(-h, h,-h), normals[2]),
            (new( h,-h, h), new( h,-h,-h), new( h, h,-h), new( h, h, h), normals[3]),
            (new(-h, h, h), new( h, h, h), new( h, h,-h), new(-h, h,-h), normals[4]),
            (new(-h,-h,-h), new( h,-h,-h), new( h,-h, h), new(-h,-h, h), normals[5]),
        };

        float[] v = new float[6 * 4 * 8];
        int k = 0;
        foreach (var f in faces)
        {
            Vector2[] uvs = { new(0, 0), new(1, 0), new(1, 1), new(0, 1) };
            Vector3[] p = { f.a, f.b, f.c, f.d };
            for (int i = 0; i < 4; i++)
            {
                v[k++] = p[i].X; v[k++] = p[i].Y; v[k++] = p[i].Z;
                v[k++] = f.n.X; v[k++] = f.n.Y; v[k++] = f.n.Z;
                v[k++] = uvs[i].X; v[k++] = uvs[i].Y;
            }
        }
        return v;
    }

    private static uint[] CubeIndices()
    {
        uint[] indices = new uint[6 * 6];
        for (uint f = 0; f < 6; f++)
        {
            uint baseV = f * 4;
            int offset = (int)f * 6;
            indices[offset + 0] = baseV + 0;
            indices[offset + 1] = baseV + 1;
            indices[offset + 2] = baseV + 2;
            indices[offset + 3] = baseV + 0;
            indices[offset + 4] = baseV + 2;
            indices[offset + 5] = baseV + 3;
        }
        return indices;
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteVertexArray(_vao);
    }
}
