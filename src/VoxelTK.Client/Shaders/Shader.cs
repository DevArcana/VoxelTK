using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoxelTK.Client.Shaders;

public class Shader : IDisposable
{
    public int Handle { get; }
    
    private readonly Dictionary<string, int> _uniformLocations = new();

    public Shader(string vertexPath, string fragmentPath)
    {
        var vertexShaderSource = File.ReadAllText(vertexPath);
        var fragmentShaderSource = File.ReadAllText(fragmentPath);
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var vertexShaderStatus);
        if (vertexShaderStatus == 0)
        {
            throw new Exception($"Failed to compile vertex shader! Info log: {GL.GetShaderInfoLog(vertexShader)}");
        }
        
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var fragmentShaderStatus);
        if (fragmentShaderStatus == 0)
        {
            throw new Exception($"Failed to compile fragment shader! Info log: {GL.GetShaderInfoLog(fragmentShader)}");
        }
        
        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var linkStatus);
        if (linkStatus == 0)
        {
            throw new Exception($"Failed to link shader program! Info log: {GL.GetProgramInfoLog(Handle)}");
        }
        
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var uniformCount);
        for (var i = 0; i < uniformCount; i++)
        {
            var name = GL.GetActiveUniform(Handle, i, out _, out _);
            var location = GL.GetUniformLocation(Handle, name);
            _uniformLocations[name] = location;
        }
    }
    
    public void Use()
    {
        GL.UseProgram(Handle);
    }
    
    public void SetMatrix4(string name, Matrix4 matrix)
    {
        GL.UniformMatrix4(_uniformLocations[name], true, ref matrix);
    }
    
    public void SetVector3(string name, Vector3 vector)
    {
        GL.Uniform3(_uniformLocations[name], vector);
    }

    private bool _disposed;
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        
        GL.DeleteProgram(Handle);
        _disposed = true;
        
        GC.SuppressFinalize(this);
    }
    
    ~Shader()
    {
        if (!_disposed)
        {
            Console.Error.WriteLine("GPU resource leak! Shader was not disposed properly!");
        }
    }
}