using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelTK.Client.Shaders;

namespace VoxelTK.Client;

public class Game() : GameWindow(GameWindowSettings.Default, new NativeWindowSettings
{
    Title = "VoxelTK",
    ClientSize = (1600, 900),
    StartVisible = false
})
{
    private Shader _shader = null!;
    private Model _model = null!;
    private Camera _camera = null!;

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        _model = new Model();
        _camera = new Camera();
        _camera.UpdateViewport(Size.X, Size.Y);
        
        // Lock mouse
        CursorState = CursorState.Grabbed;
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _shader.Dispose();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        _camera.UpdateViewport(e.Width, e.Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        var dt = (float)e.Time;

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _camera.Move(_camera.Forward * dt);
        }
        else if (KeyboardState.IsKeyDown(Keys.S))
        {
            _camera.Move(-_camera.Forward * dt);
        }
        
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _camera.Move(-_camera.Right * dt);
        }
        else if (KeyboardState.IsKeyDown(Keys.D))
        {
            _camera.Move(_camera.Right * dt);
        }

        var md = MouseState.Delta;
        _camera.Pitch -= md.Y * 0.1f;
        _camera.Yaw += md.X * 0.1f;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _shader.Use();
        _camera.SetUniforms(_shader);
        
        _model.Render();

        SwapBuffers();
    }
}