using OpenTK.Graphics.OpenGL;

namespace VoxelTK.Client.Shaders;

public class Shader : IDisposable
{
    private readonly int _handle;

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
        
        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        
        GL.LinkProgram(_handle);
        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var linkStatus);
        if (linkStatus == 0)
        {
            throw new Exception($"Failed to link shader program! Info log: {GL.GetProgramInfoLog(_handle)}");
        }
        
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }
    
    public void Use()
    {
        GL.UseProgram(_handle);
    }

    private bool _disposed;
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        
        GL.DeleteProgram(_handle);
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