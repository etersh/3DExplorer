using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System;
using Game.Engine;

namespace Game;
public class GameWindowEx : GameWindow
{
    Shader _shader = null!;
    Texture _tex = null!;
    Mesh _meshFloor = null!;
    Mesh _meshCube = null!;
    Mesh _meshWall = null!;
    Camera _cam = null!;
    bool _lightOn = true;
    Vector3 _lightDir = new(-0.3f, -1.0f, -0.2f);
    Vector3 _itemPos = new(0, 0.5f, 0);
    bool _collected = false;
    float _collectDistance = 1.6f;
    float _spin = 0f;

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
        _tex = new Texture("Assets/texture.png");
        _cam = new Camera(new Vector3(0, 1.5f, 3f), Size.X / (float)Size.Y);

        _meshFloor = Mesh.CreateTexturedQuadXZ(10f, 0f);
        _meshCube = Mesh.CreateCube(1f);
        _meshWall = Mesh.CreateQuadXY(10f, -5f);
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

        float distToItem = (_cam.Position - _itemPos).Length;
        if (!_collected && distToItem < _collectDistance && kb.IsKeyPressed(Keys.F))
            _collected = true;
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

        _tex.Bind(TextureUnit.Texture0);
        _shader.SetInt("uTex", 0);

        _shader.SetMatrix4("uModel", Matrix4.Identity);
        _meshFloor.Draw();

        _shader.SetMatrix4("uModel", Matrix4.Identity);
        _meshWall.Draw();

        void DrawCubeAt(Vector3 pos, Vector3 scale)
        {
            var model = Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(pos);
            _shader.SetMatrix4("uModel", model);
            _meshCube.Draw();
        }

        DrawCubeAt(new Vector3(-2, 0.5f, -2), new Vector3(1, 1, 1));
        DrawCubeAt(new Vector3(2, 0.5f, -2), new Vector3(1, 1, 1));

        if (!_collected)
        {
            var modelItem = Matrix4.CreateScale(1f)
                * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_spin))
                * Matrix4.CreateTranslation(_itemPos);
            _shader.SetMatrix4("uModel", modelItem);
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
        _tex.Dispose();
    }
}
