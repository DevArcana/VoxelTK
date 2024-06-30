using VoxelTK.Client;

try
{
    Console.WriteLine("Initializing game...");
    using var game = new Game();
    Console.WriteLine("Game is now running.");
    game.CenterWindow();
    game.IsVisible = true;
    game.Run();
    Console.WriteLine("Game has exited.");
}
catch (Exception exception)
{
    Console.WriteLine("Game has run into an error!");
    Console.Error.WriteLine(exception);
}