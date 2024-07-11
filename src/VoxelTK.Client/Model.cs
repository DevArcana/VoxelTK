using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoxelTK.Client;

public sealed class Model : IDisposable
{
    private readonly float[] _vertices = {
        0.5f,  0.5f, 0.0f,  // top right
        0.5f, -0.5f, 0.0f,  // bottom right
        -0.5f, -0.5f, 0.0f,  // bottom left
        -0.5f,  0.5f, 0.0f   // top left
    };
    
    private readonly uint[] _indices = {  // note that we start from 0!
        0, 1, 3,   // first triangle
        1, 2, 3    // second triangle
    };
    
    private readonly int _vertexBufferObject;
    private readonly int _vertexArrayObject;
    private readonly int _elementBufferObject;
    
    public Model()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        // Instead of 0 I can get the location GL.GetAttribLocation(Handle, attribName);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0); // NOTE: Bound to the currently bound VBO.
        
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
    }
    
    public void Render()
    {
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteBuffer(_elementBufferObject);
    }
}