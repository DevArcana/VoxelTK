﻿using ImGuiNET;
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
    private Chunk _chunk = null!;
    private Camera _camera = null!;
    private ImGuiController _imgui = null!;

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        _chunk = new Chunk();
        _camera = new Camera();
        _camera.UpdateViewport(Size.X, Size.Y);
        _imgui = new ImGuiController(ClientSize.X, ClientSize.Y);
        
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
        _imgui.WindowResized(ClientSize.X, ClientSize.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        if (KeyboardState.IsKeyPressed(Keys.Tab))
        {
            CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;

            if (CursorState == CursorState.Grabbed)
            {
                ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NoMouse;
            }
            else
            {
                ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.NoMouse;
            }
        }
        
        if (CursorState == CursorState.Grabbed)
        {
            var dt = (float)e.Time;
            var speed = 5.0f;

            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                speed *= 2.0f;
            }
        
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                _camera.Move(Vector3.UnitY * dt * speed);
            }
            else if (KeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _camera.Move(-Vector3.UnitY * dt * speed);
            }

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                _camera.Move(_camera.Forward * dt * speed);
            }
            else if (KeyboardState.IsKeyDown(Keys.S))
            {
                _camera.Move(-_camera.Forward * dt * speed);
            }
        
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                _camera.Move(-_camera.Right * dt * speed);
            }
            else if (KeyboardState.IsKeyDown(Keys.D))
            {
                _camera.Move(_camera.Right * dt * speed);
            }

            var md = MouseState.Delta;
            _camera.Pitch -= md.Y * 0.1f;
            _camera.Yaw += md.X * 0.1f;
        }
    }
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        _imgui.Update(this, (float) e.Time);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _shader.Use();
        _camera.SetUniforms(_shader);
        
        _chunk.Render();

        ImGui.Begin("Budget");
        ImGui.Text($"FPS: {1.0f / e.Time:0}");
        ImGui.Text($"Frame time (ms): {1000.0f * e.Time:F}");
        ImGui.End();
        
        _imgui.Render();
        ImGuiController.CheckGLError("End of frame");
        
        SwapBuffers();
    }
}