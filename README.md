# Surgical XR Training Simulation

## Description
Unity 3D mixed-reality / virtual-reality surgical simulation developed for Computacao Grafica coursework in 2025. The experience targets Meta Quest 3S / Android and places the user in a medical-training flow: lobby, clinical room, incision, heart removal, heart transplant, and reset.

## Tech Stack
- Unity 3D
- C#
- Universal Render Pipeline (URP)
- XR Interaction Toolkit / OpenXR
- Meta Quest 3S Android target
- Shader Graph-style holographic material control
- Spatial/reactive audio

## Highlights
- Hybrid VR/MR flow with a training lobby and surgical room.
- Scalpel/incision logic using trigger detection and tissue fade/dissolve behavior.
- Heart transplant interaction with XR socket selection behavior.
- Holographic anatomy transparency controlled from script.
- Heart monitor audio that reacts to organ removal/reinsertion.
- APK and video deliverables kept outside Git tracking.

## Structure
- `Trabalho3CGVR/Assets/Scripts/` contains the main custom C# scripts.
- `Trabalho3CGVR/Assets/Scenes/` contains Unity scenes.
- `Trabalho3CGVR/Packages/` and `Trabalho3CGVR/ProjectSettings/` contain Unity project configuration.
- Root APK, ZIP, and video files are ignored as release/demo artifacts.

## How to Run
Open `Trabalho3CGVR` with Unity. The project was built/tested for XR simulation and Meta Quest 3S deployment.
