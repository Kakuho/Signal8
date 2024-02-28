using Memory;
using System;

public class Chip8{
  public byte[] gpr = new byte[16];
  public ushort index;
  public byte delay_timer;
  public byte sound_timer;
  public ushort pc = 0x200;
  public byte stack_pointer;
  public ushort[] stack = new ushort[16];
  public ushort ibuffer;
  // memory
  public Ram ram;
  public FrameBuffer frame;
  // rng generator
  private Random rng;

  Chip8(){
    ram = new Ram();
    frame = new FrameBuffer();
    rng = new Random();
  }

  public byte GetRegister(ushort index){
    return gpr[index];
  }

  public void SetRegister(ushort index, byte val){
    gpr[index] = val;
  }

  public void Cycle(ushort ibuffer){
    // beautiful c# artifact of not allowing implicit type conversions
    ushort arg_nnn = (ushort)(ibuffer & 0x0FFFu);
    // bytes are integral promoted when used in binary operations.
    byte arg_n = (byte)(ibuffer & 0xFu);
    byte arg_x = (byte)((ibuffer & 0xF00u) >> 8); // hope you do logical right shift
    byte arg_y = (byte)(ibuffer & 0xF0u); // hope you do logical right shift
    byte arg_kk = (byte)(ibuffer & 0xFFu);
    // now we actually doing decoding
    byte opcode = (byte)(ibuffer & (0xFu << 16));
    switch(opcode){
      case 0:
        if((ibuffer & 0x0F00u) > 0){
          op0nnn(arg_nnn);
        }
        else{
          op00E0();
        }
        break;
      case 1:
        op1nnn(arg_nnn);
        break;
      case 2:
        op2nnn(arg_nnn);
        break;
      case 3:
        op3xkk(arg_x, arg_kk);
        break;
      case 4:
        op4xkk(arg_x, arg_kk);
        break;
      case 5:
        op5xy0(arg_x, arg_y);
        break;
      case 6:
        op6xkk(arg_x, arg_kk);
        break;
      case 7:
        op7xkk(arg_x, arg_kk);
        break;
      case 8:
        // beautiful integral promotion
        byte secondOpcode = (byte)(ibuffer & 0xF);
        switch(secondOpcode){
          case 0:
            op8xy0(arg_x, arg_y);
            break;
          case 1:
            op8xy1(arg_x, arg_y);
            break;
          case 2:
            op8xy2(arg_x, arg_y);
            break;
          case 3:
            op8xy3(arg_x, arg_y);
            break;
          case 4:
            op8xy4(arg_x, arg_y);
            break;
          case 5:
            op8xy5(arg_x, arg_y);
            break;
          case 6:
            op8xy6(arg_x, arg_y);
            break;
        }
        break;
    }
  }

  public void op0nnn(ushort nnn){
    // no op
    return;
  }

  public void op00E0(){
    // clear framebuffer;
    frame.Clear();
  }

  public void op1nnn(ushort nnn){
    pc = nnn;
  }

  public void op2nnn(ushort nnn){
    stack_pointer++;
    stack[stack_pointer] = pc;
    pc = nnn;
  }

  public void op3xkk(byte x, byte kk){
    if(GetRegister(x) == kk){
      pc += 2;
    }
  }

  public void op4xkk(byte x, byte kk){
    if(GetRegister(x) != kk){
      pc += 2;
    }
  }
  
  public void op5xy0(byte x, byte y){
    if(GetRegister(x) == GetRegister(y)){
      pc += 2;
    }
  }

  public void op6xkk(byte x, byte kk){
    SetRegister(x, kk);
  }

  public void op7xkk(byte x, byte kk){
    // why does addition of bytes get promoted to int?
    SetRegister(x, (byte)(GetRegister(x) + kk));
  }

  public void op8xy0(byte x, byte y){
    SetRegister(y, GetRegister(x));
  }

  public void op8xy1(byte x, byte y){
    SetRegister(y, (byte)(GetRegister(x) | GetRegister(y)));
  }

  public void op8xy2(byte x, byte y){
    SetRegister(y, (byte)(GetRegister(x) & GetRegister(y)));
  }

  public void op8xy3(byte x, byte y){
    SetRegister(y, (byte)(GetRegister(x) ^ GetRegister(y)));
  }

  public void op8xy4(byte x, byte y){
    byte xval = GetRegister(x);
    byte yval = GetRegister(y);
    if(xval + yval < xval || xval + yval < yval){
      SetRegister(0xF, 1);
    }
    else{
      SetRegister(0xF, 0);
    }
    SetRegister(x, (byte)(xval + yval));
  }

  public void op8xy5(byte x, byte y){
    byte xval = GetRegister(x);
    byte yval = GetRegister(y);
    if(yval > xval){
      SetRegister(0xF, 1);
    }
    else{
      SetRegister(0xF, 0);
    }
    SetRegister(x, (byte)(xval - yval));

  }

  public void op8xy6(byte x, byte y){
    if( (GetRegister(x) & 0x01) > 0){
      SetRegister(0x0f, 1);
    }
    else{
      SetRegister(0x0f, 0);
    }
    SetRegister(x, (byte)(GetRegister(x) >> 1));

  }

  public void op8xy7(byte x, byte y){
    byte xval = GetRegister(x);
    byte yval = GetRegister(y);
    if(xval > yval){
      SetRegister(0xF, 1);
    }
    else{
      SetRegister(0xF, 0);
    }
    SetRegister(x, (byte)(yval - xval));
  }

  public void op8xyE(byte x, byte y){
    if( (GetRegister(x) & 0x80) > 0){
      SetRegister(0x0f, 1);
    }
    else{
      SetRegister(0x0f, 0);
    }
    SetRegister(x, (byte)(GetRegister(x) << 1));
  }

  public void op9xy0(byte x, byte y){
    if(GetRegister(x) != GetRegister(y)){
      pc += 2;
    }
  }

  public void opAnnn(ushort nnn){
    index = nnn;
  }

  public void opBnnn(ushort nnn){
    // huh, even more integral promotions
    pc = (ushort)(nnn + GetRegister(0));
  }

  public void opCxkk(byte x){
    int randbyte = rng.Next(256);
    SetRegister(x, (byte)(GetRegister(x) & randbyte));
  }


}
