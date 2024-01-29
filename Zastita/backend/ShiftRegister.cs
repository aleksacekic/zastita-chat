using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend
{
  class ShiftRegister
  {
    public ShiftRegister()
    {

    }

    private string register = "";

    public string Register
    {
      get
      {
        return this.register;
      }
      set
      {
        this.register = value;
      }
    }

    public char Shift(char newEntry)
    {
      if (this.register.Length == 0) return ' ';

      char[] regEls = this.register.ToCharArray();

      char ret = this.register[this.register.Length - 1];

      for (int i = this.register.Length - 1; i > 0; i--)
      {
        regEls[i] = this.register[i - 1];
      }

      regEls[0] = newEntry;

      this.register = this.FromCharArrayToString(regEls);

      return ret;
    }

    string FromCharArrayToString(char[] start)
    {
      string ret = "";
      for (int i = 0; i < start.Length; i++)
      {
        ret += start[i];
      }

      return ret;
    }

    private int[] opMembers;

    public int[] OperationMembers
    {
      get
      {
        return this.opMembers;
      }
      set
      {
        this.opMembers = value;
      }
    }

    public char Output
    {
      get
      {
        return this.register[this.register.Length - 1];
      }
    }
  }
}
