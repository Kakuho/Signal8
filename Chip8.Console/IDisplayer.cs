// general class which encapsulates a display controller

using Memory;

interface IDisplayer{
  void Render(FrameBuffer fb);
  void Initialise();
  bool KeyState();
}
