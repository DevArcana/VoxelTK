using OpenTK.Mathematics;
using VoxelTK.Client.Shaders;

namespace VoxelTK.Client;

public sealed class Camera
{
    private Matrix4 _projection = Matrix4.Identity;
    private Matrix4 _view = Matrix4.Identity;
    
    private Vector3 _position = -Vector3.UnitZ;
    private float _pitch = 0.0f;
    private float _yaw = 0.0f;
    
    // These are calculated by pitch and yaw
    private Vector3 _forward = Vector3.Zero;
    private Vector3 _right = Vector3.Zero;
    private Vector3 _up = Vector3.Zero;

    public Camera()
    {
        UpdateVectors();
    }

    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            _pitch = MathHelper.DegreesToRadians(MathHelper.Clamp(value, -89.0f, 89.0f));
            UpdateVectors();
        }
    }
    
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            if (_yaw > MathHelper.TwoPi)
            {
                _yaw -= MathHelper.TwoPi;
            }
            else if (_yaw < 0.0f)
            {
                _yaw += MathHelper.TwoPi;
            }
            UpdateVectors();
        }
    }

    private void UpdateVectors()
    {
        _forward.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _forward.Y = MathF.Sin(_pitch);
        _forward.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        _forward = Vector3.Normalize(_forward);
        _right = Vector3.Normalize(Vector3.Cross(_forward, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _forward));
        _view = Matrix4.LookAt(_position, _position + _forward, _up);
    }

    public Vector3 Forward => _forward;
    public Vector3 Right => _right;
    public Vector3 Up => _up;
    
    public void UpdateViewport(int width, int height)
    {
        _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(80.0f), width / (float)height, 0.1f, 100.0f);
    }
    
    public void SetUniforms(Shader shader)
    {
        shader.SetMatrix4("uProjectionMatrix", _projection);
        shader.SetMatrix4("uViewMatrix", _view);
    }
    
    public void Move(Vector3 offset)
    {
        _position += offset;
    }
}