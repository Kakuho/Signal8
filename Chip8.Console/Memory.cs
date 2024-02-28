namespace Memory{
  public sealed class Ram{
    public byte[] data = new byte[4096];

    public Ram(){
      Fontset fs_tmp = new Fontset();
      CopyFontset(fs_tmp);
    }

    public byte Get(uint index){
      return data[index];
    }

    public void Set(uint index, byte val){
      data[index] = val;
    }

    private void CopyFontset(Fontset fs){
      uint fontstart = 0x00;
      for(uint i = fontstart; i < fs.data.Length; i++){
        data[i] = fs.data[i];
      }
    }
  }

  public sealed class FrameBuffer{
    // envision as a set of Frame where each cell is either on or off
    public bool[] data = new bool[64*31];

    public bool Get(uint index){
      return data[index];
    }

    public void Set(uint index, bool state){
      data[index] = state;
    }

    public void Clear(){
      /*
      foreach(ref bool cell in data){
        cell = false;
      }
      */
      for(int i = 0; i < data.Length; i++){
        data[i] = false;
      }
    }
  }

  public sealed class Fontset{
    public byte[] data = [
      0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
      0x20, 0x60, 0x20, 0x20, 0x70, // 1
      0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
      0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
      0x90, 0x90, 0xF0, 0x10, 0x10, // 4
      0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
      0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
      0xF0, 0x10, 0x20, 0x40, 0x40, // 7
      0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
      0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
      0xF0, 0x90, 0xF0, 0x90, 0x90, // A
      0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
      0xF0, 0x80, 0x80, 0x80, 0xF0, // C
      0xE0, 0x90, 0x90, 0x90, 0xE0, // D
      0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
      0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    ];
  }
}
