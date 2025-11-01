using OpenTK.Mathematics;
using System;

namespace Game.Engine;

public class Camera
{
    public enum Direction { Forward, Backward, Left, Right }

    public Vector3 Position;
    private float _yaw = -90f;
    private float _pitch = 0f;
    public float Fov = MathHelper.DegreesToRadians(60f);
    public float Aspect;

    public Camera(Vector3 position, float aspect)
    {
        Position = position;
        Aspect = aspect;
    }

    public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + Front, Vector3.UnitY);
    public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(Fov, Aspect, 0.1f, 100f);

    public Vector3 Front => new(
        MathF.Cos(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch)),
        MathF.Sin(MathHelper.DegreesToRadians(_pitch)),
        MathF.Sin(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch))
    );

    public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
    public Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, Front));

    public void Move(Direction dir, float delta)
    {
        switch (dir)
        {
            case Direction.Forward: Position += Front * delta; break;
            case Direction.Backward: Position -= Front * delta; break;
            case Direction.Left: Position -= Right * delta; break;
            case Direction.Right: Position += Right * delta; break;
        }
    }

    public void AddYawPitch(float yawDelta, float pitchDelta)
    {
        _yaw += yawDelta;
        _pitch = Math.Clamp(_pitch + pitchDelta, -89f, 89f);
    }
}
