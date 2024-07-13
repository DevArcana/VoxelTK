using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoxelTK.Client;

public sealed class Chunk : IDisposable
{
    private const int Size = 128;
    private readonly int[] _blocks = new int[Size * Size * Size];
    
    private readonly int _vertexBufferObject;
    private readonly int _vertexArrayObject;
    private readonly int _elementBufferObject;
    private int _indices;
    
    public Chunk()
    {
        _vertexBufferObject = GL.GenBuffer();
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        _elementBufferObject = GL.GenBuffer();
        
        var stopwatch = new Stopwatch();
        
        stopwatch.Start();
        Generate();
        Console.WriteLine($"Generated chunk in {stopwatch.ElapsedMilliseconds}ms.");
        
        stopwatch.Restart();
        RebuildMesh();
        Console.WriteLine($"Mesh rebuilt in {stopwatch.ElapsedMilliseconds}ms.");
    }

    private void Generate()
    {
        // fill blocks with random data
        var random = new Random();
        for (var i = 0; i < _blocks.Length; i++)
        {
            _blocks[i] = random.Next(0, 2);
        }
    }

    private void RebuildMesh()
    {
        var vertices = new List<float>(Size * Size * Size * 6 * 4 * 3);
        var indices = new List<uint>(Size * Size * Size * 6 * 6);
        
        // build cubes for each block non zero
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                for (var z = 0; z < Size; z++)
                {
                    var block = _blocks[x + y * Size + z * Size * Size];
                    if (block == 0)
                    {
                        continue;
                    }

                    var fx = (float)x;
                    var fy = (float)y;
                    var fz = (float)z;
                    
                    // top face
                    var indicesCount = (uint) vertices.Count / 3;
                    vertices.AddRange([fx, fy, fz, fx + 1, fy, fz, fx + 1, fy, fz + 1, fx, fy, fz + 1]);
                    indices.AddRange([indicesCount + 2, indicesCount + 1, indicesCount, indicesCount, indicesCount + 3, indicesCount + 2]);
                    
                    // bottom face
                    indicesCount = (uint) vertices.Count / 3;
                    vertices.AddRange([fx, fy - 1, fz, fx + 1, fy - 1, fz, fx + 1, fy - 1, fz + 1, fx, fy - 1, fz + 1]);
                    indices.AddRange([indicesCount, indicesCount + 1, indicesCount + 2, indicesCount + 2, indicesCount + 3, indicesCount]);
                    
                    // front face
                    indicesCount = (uint) vertices.Count / 3;
                    vertices.AddRange([fx, fy, fz, fx + 1, fy, fz, fx + 1, fy - 1, fz, fx, fy - 1, fz]);
                    indices.AddRange([indicesCount, indicesCount + 1, indicesCount + 2, indicesCount + 2, indicesCount + 3, indicesCount]);
                    
                    // back face
                    indicesCount = (uint) vertices.Count / 3;
                    vertices.AddRange([fx, fy, fz + 1, fx + 1, fy, fz + 1, fx + 1, fy - 1, fz + 1, fx, fy - 1, fz + 1]);
                    indices.AddRange([indicesCount, indicesCount + 1, indicesCount + 2, indicesCount + 2, indicesCount + 3, indicesCount]);
                    
                    // left face
                    indicesCount = (uint) vertices.Count / 3;
                    vertices.AddRange([fx, fy, fz, fx, fy, fz + 1, fx, fy - 1, fz + 1, fx, fy - 1, fz]);
                    indices.AddRange([indicesCount + 2, indicesCount + 1, indicesCount, indicesCount, indicesCount + 3, indicesCount + 2]);

                    // right face
                    indicesCount = (uint) vertices.Count / 3;
                    vertices.AddRange([fx + 1, fy, fz, fx + 1, fy, fz + 1, fx + 1, fy - 1, fz + 1, fx + 1, fy - 1, fz]);
                    indices.AddRange([indicesCount, indicesCount + 1, indicesCount + 2, indicesCount + 2, indicesCount + 3, indicesCount]);
                }
            }
        }

        var verticesArray = vertices.ToArray();
        var indicesArray = indices.ToArray();
        _indices = indicesArray.Length;
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, verticesArray.Length * sizeof(float), verticesArray, BufferUsageHint.StaticDraw);
        GL.BindVertexArray(_vertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indicesArray.Length * sizeof(uint), indicesArray, BufferUsageHint.StaticDraw);
    }
    
    public void Render()
    {
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indices, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteBuffer(_elementBufferObject);
    }
}