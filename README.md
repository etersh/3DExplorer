# Mini 3D Explorer

## Controls
- WASD: Move
- Mouse: Look
- E: Toggle Light
- F: Collect Item (when close)
- ESC: Release/Lock Cursor

## GamePlay
- Explore a small walled area with multiple collectible boxes.
- Approach a box and press F to collect it — it will disappear.
- Press E to toggle between light on/off and observe changes in Phong lighting.
- Move freely around using WASD and control the camera with your mouse.
- Press ESC to release or re-lock the cursor.

## Features
- Phong lighting
- Textured floor
- Light toggle interaction
- Collectible item cube
- Camera movement and look

## Build
Make sure you have the .NET SDK installed and working:
dotnet --version
Navigate to the project folder and run:
dotnet run --project Game/Game.csproj

## Assets & Shader Setup
Ensure all shaders and textures are copied to the output directory after build.
In Game/Game.csproj, include the following if not already present:

``
<ItemGroup>
  <None Update="Shaders\*.glsl">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
  <None Update="Assets\*.png">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
``
