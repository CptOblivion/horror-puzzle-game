# Untitled PSX-style horror puzzle game
### A proof-of-concept for rendering a PSX-style horror game, with on-the-fly generated pre-rendered backgrounds
- In the style of Resident Evil or old PC point-and-click adventures, but using modern rasterization to generate the backgrounds on scene transition
- Allowing for rapid development of complicated scenes, rendered with more fidelity than a phone or older computer could keep up with in real-time
Unity 2019.2.6f1
- dynamic elements (characters, items, animated objects in the scene) are rendered separately, with a very simple vertex shaded pipeline, both for performance and to further imitate the style of early 3d games
- elements in the pre-rendered scene are still game objects that can be modified (and then visually updated by re-rendering the scene), allowing for a great deal of flexibility in designing the gameplay and interactions with the environment without adding much rendering overhead or breaking the visual style.

## Screenshots
