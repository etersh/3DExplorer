using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using Game.Engine;

namespace Game;
public class GameWindowEx : GameWindow
{
    Shader _shader = null!;
    Texture _texBox = null!;
    Texture _texFloor = null!;
    Texture _texWall = null!;
    Mesh _meshFloor = null!;
    Mesh _meshCube = null!;
    Mesh _meshWall = null!;
    Camera _cam = null!;
    bool _lightOn = true;
    Vector3 _lightDir = new(-0.3f, -1.0f, -0.2f);
    float _collectDistance = 2.0f;
    float _spin = 0f;

    struct Box
    {
        public Vector3 Position;
        public Vector3 Scale;
        public bool Collected;
    }

    List<Box> _boxes = new();

    public GameWindowEx(int w, int h, string title)
        : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            ClientSize = (w, h),
            Title = title,
            Flags = ContextFlags.ForwardCompatible
        })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();
        CursorState = CursorState.Grabbed;
        GL.Enable(EnableCap.DepthTest);
        VSync = VSyncMode.On;

        _shader = new Shader("Shaders/vertex.glsl", "Shaders/fragment.glsl");
        _texBox = new Texture("Assets/texture_box.png");
        _texFloor = new Texture("Assets/texture_floor.png");
        _texWall = new Texture("Assets/texture_wall.png");
        _cam = new Camera(new Vector3(0, 1.5f, 3f), Size.X / (float)Size.Y);

        _meshCube = Mesh.CreateCube(1f);
        _meshFloor = Mesh.CreateTexturedQuadXZ(40f, 0f);
        _meshWall = Mesh.CreateQuadXY(40f, 0f);

        _boxes = new List<Box>
        {
            new Box { Position = new Vector3(0, 0.5f, 0), Scale = new Vector3(3, 3, 3) },
            new Box { Position = new Vector3(-6, 1f, -4), Scale = new Vector3(3, 3, 3) },
            new Box { Position = new Vector3(-10, 0.5f, 5), Scale = new Vector3(3, 3, 3) },
            new Box { Position = new Vector3(8, 1f, -6), Scale = new Vector3(3, 3, 3) },
            new Box { Position = new Vector3(12, 0.5f, 3), Scale = new Vector3(3, 3, 3) },
            new Box { Position = new Vector3(0, 0.5f, -10), Scale = new Vector3(4, 4, 4) },
            new Box { Position = new Vector3(-8, 0.5f, -12), Scale = new Vector3(3, 3, 3) },
            new Box { Position = new Vector3(8, 0.5f, -15), Scale = new Vector3(3, 3, 3) }
        };
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        if (!IsFocused) return;

        _spin += 60f * (float)e.Time;

        var kb = KeyboardState;
        if (kb.IsKeyPressed(Keys.Escape))
        {
            CursorState = CursorState == CursorState.Grabbed
                ? CursorState.Normal
                : CursorState.Grabbed;
        }

        if (kb.IsKeyPressed(Keys.E))
            _lightOn = !_lightOn;

        float speed = 4.0f;
        if (kb.IsKeyDown(Keys.W)) _cam.Move(Camera.Direction.Forward, speed * (float)e.Time);
        if (kb.IsKeyDown(Keys.S)) _cam.Move(Camera.Direction.Backward, speed * (float)e.Time);
        if (kb.IsKeyDown(Keys.A)) _cam.Move(Camera.Direction.Left, speed * (float)e.Time);
        if (kb.IsKeyDown(Keys.D)) _cam.Move(Camera.Direction.Right, speed * (float)e.Time);

        if (kb.IsKeyPressed(Keys.F))
        {
            for (int i = 0; i < _boxes.Count; i++)
            {
                if (_boxes[i].Collected) continue;
                float dist = (_cam.Position - _boxes[i].Position).Length;
                if (dist < _collectDistance)
                {
                    var b = _boxes[i];
                    b.Collected = true;
                    _boxes[i] = b;
                }
            }
        }
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        if (CursorState != CursorState.Grabbed) return;
        _cam.AddYawPitch(e.DeltaX * 0.1f, -e.DeltaY * 0.1f);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        _cam.Aspect = Size.X / (float)Size.Y;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.ClearColor(0.1f, 0.12f, 0.15f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();
        _shader.SetMatrix4("uView", _cam.GetViewMatrix());
        _shader.SetMatrix4("uProj", _cam.GetProjectionMatrix());
        _shader.SetVector3("uViewPos", _cam.Position);
        _shader.SetVector3("uLightDir", Vector3.Normalize(_lightDir));
        _shader.SetInt("uLightOn", _lightOn ? 1 : 0);

        _texFloor.Bind(TextureUnit.Texture0);
        _shader.SetInt("uTex", 0);
        _shader.SetMatrix4("uModel", Matrix4.Identity);
        _meshFloor.Draw();

        _texWall.Bind(TextureUnit.Texture0);
        _shader.SetInt("uTex", 0);

        Matrix4 modelBack = Matrix4.CreateTranslation(0, 0, -20f);
        Matrix4 modelFront = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(180)) * Matrix4.CreateTranslation(0, 0, 20f);
        Matrix4 modelLeft = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(-20f, 0f, 0f);
        Matrix4 modelRight = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-90)) * Matrix4.CreateTranslation(20f, 0f, 0f);

        _shader.SetMatrix4("uModel", modelBack);
        _meshWall.Draw();
        _shader.SetMatrix4("uModel", modelFront);
        _meshWall.Draw();
        _shader.SetMatrix4("uModel", modelLeft);
        _meshWall.Draw();
        _shader.SetMatrix4("uModel", modelRight);
        _meshWall.Draw();

        _texBox.Bind(TextureUnit.Texture0);
        _shader.SetInt("uTex", 0);

        foreach (var b in _boxes)
        {
            if (b.Collected) continue;
            var model = Matrix4.CreateScale(b.Scale)
                * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_spin))
                * Matrix4.CreateTranslation(b.Position);
            _shader.SetMatrix4("uModel", model);
            _meshCube.Draw();
        }

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _meshFloor.Dispose();
        _meshCube.Dispose();
        _meshWall.Dispose();
        _shader.Dispose();
        _texBox.Dispose();
        _texFloor.Dispose();
        _texWall.Dispose();
    }
}
