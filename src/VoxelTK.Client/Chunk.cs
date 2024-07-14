using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoxelTK.Client;

public sealed class Chunk : IDisposable
{
    private const int Size = 16;
    private readonly byte[] _blocks = new byte[Size * Size * Size];
    
    private readonly int _vertexBufferObject;
    private readonly int _vertexArrayObject;
    private readonly int _elementBufferObject;
    
    private readonly float[] _vertices = new float[Size * Size * Size * 6 * 4 * 3];
    private readonly uint[] _indices = new uint[Size * Size * Size * 6 * 6];
    private uint _vi;
    private uint _ii;

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
            _blocks[i] = (byte) random.Next(0, 2);
        }
        
        for (var i = 0; i < _blocks.Length; i++)
        {
            if (_blocks[i] == 0)
            {
                continue;
            }
            
            var x = i % Size;
            var y = (i / Size) % Size;
            var z = i / (Size * Size);
            
            var top = x + (y + 1) * Size + z * Size * Size;
            var bottom = x + (y - 1) * Size + z * Size * Size;
            var back = x + y * Size + (z + 1) * Size * Size;
            var front = x + y * Size + (z - 1) * Size * Size;
            var left = (x - 1) + y * Size + z * Size * Size;
            var right = (x + 1) + y * Size + z * Size * Size;

            if (y == Size - 1 || (_blocks[top] & 1) == 0)
            {
                _blocks[i] |= 1 << 1;
            } 
            
            if (y == 0 || (_blocks[bottom] & 1) == 0)
            {
                _blocks[i] |= 1 << 2;
            }
            
            if (z == Size - 1 || (_blocks[back] & 1) == 0)
            {
                _blocks[i] |= 1 << 3;
            }
            
            if (z == 0 || (_blocks[front] & 1) == 0)
            {
                _blocks[i] |= 1 << 4;
            }

            if (x == 0 || (_blocks[left] & 1) == 0)
            {
                _blocks[i] |= 1 << 5;
            }
            
            if (x == Size - 1 || (_blocks[right] & 1) == 0)
            {
                _blocks[i] |= 1 << 6;
            }
        }
    }

    private void RebuildMesh()
    {
        _vi = 0;
        _ii = 0;
        // build cubes for each block non zero
        var cubes = 0;
        var skipped = 0;
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                for (var z = 0; z < Size; z++)
                {
                    var block = _blocks[x + y * Size + z * Size * Size];
                    if (block >> 1 == 0)
                    {
                        skipped++;
                        continue;
                    }

                    cubes++;

                    var fx = (float)x;
                    var fy = (float)y;
                    var fz = (float)z;
                    uint indicesCount = 0;
                    
                    // top face
                    if (((block >> 1) & 1) == 1)
                    {
                        indicesCount = _vi / 3;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz + 1;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 1;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount + 3;
                        _indices[_ii++] = indicesCount + 2;
                    }
                    
                    // bottom face
                    if (((block >> 2) & 1) == 1)
                    {
                        indicesCount = _vi / 3;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz + 1;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount + 1;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 3;
                        _indices[_ii++] = indicesCount;
                    }
                    // front face
                    if (((block >> 4) & 1) == 1)
                    {
                        indicesCount = _vi / 3;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount + 1;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 3;
                        _indices[_ii++] = indicesCount;
                    }
                    // back face
                    if (((block >> 3) & 1) == 1)
                    {
                        indicesCount = _vi / 3;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz + 1;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 1;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount + 3;
                        _indices[_ii++] = indicesCount + 2;
                    }
                    
                    // left face
                    if (((block >> 5) & 1) == 1)
                    {
                        indicesCount = _vi / 3;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 1;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount + 3;
                        _indices[_ii++] = indicesCount + 2;
                    }
                    
                    // right face
                    if (((block >> 6) & 1) == 1)
                    {
                        indicesCount = _vi / 3;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz + 1;
                        _vertices[_vi++] = fx + 1;
                        _vertices[_vi++] = fy - 1;
                        _vertices[_vi++] = fz;
                        _indices[_ii++] = indicesCount;
                        _indices[_ii++] = indicesCount + 1;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 2;
                        _indices[_ii++] = indicesCount + 3;
                        _indices[_ii++] = indicesCount;
                    }
                }
            }
        }
        Console.WriteLine("Meshed " + cubes + " cubes.");
        Console.WriteLine("Skipped " + skipped + " cubes.");
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, (int) _vi * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        GL.BindVertexArray(_vertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (int) _ii * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
    }
    
    public void Render()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, (int) _ii, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteBuffer(_elementBufferObject);
    }
}