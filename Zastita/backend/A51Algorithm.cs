using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend
{
  class A51Algorithm
  {

    public ShiftRegister X = new ShiftRegister();
    public ShiftRegister Y = new ShiftRegister();
    public ShiftRegister Z = new ShiftRegister();

    public A51Algorithm()
    {
      this.SetDefaultKey();
      X.OperationMembers = new int[] { 13, 16, 17, 18 };
      Y.OperationMembers = new int[] { 20, 21 };
      Z.OperationMembers = new int[] { 7, 20, 21, 22 };
    }

    public A51Algorithm(string newKey) : this()
    {
      this.Key = newKey;
    }

    private string key;

    public string Key
    {
      get
      {
        return this.key;
      }
      set
      {
        this.key = value;
        char[] keyEls = key.ToCharArray();
        int i = 0;
        string pom = "";

        while (i < 19)
        {
          pom += keyEls[i];
          i++;
        }

        X.Register = pom;
        pom = "";

        while (i < 41)
        {
          pom += keyEls[i];
          i++;
        }

        Y.Register = pom;
        pom = "";

        while (i < 64)
        {
          pom += keyEls[i];
          i++;
        }
        Z.Register = pom;
      }
    }

    public void SetDefaultKey()
    {
      this.Key = "0101010101010101010101010101010101010101010101010101010101010101";
    }

    public char XOR(char a, char b)
    {
      if (a == b)
        return '0';
      else
        return '1';
    }

    public void ResetRegisters()
    {
      Key = key;
    }

    public void RegisterSteps(ShiftRegister sr)
    {
      char t = '0';
      foreach (int index in sr.OperationMembers)
      {
        t = XOR(t, sr.Register[index]);
      }

      sr.Shift(t);
    }

    public char MajorityVote()
    {
      //int sum = System.Convert.ToInt32(X.Register[8]);
      //sum += System.Convert.ToInt32(Y.Register[10]);
      //sum += System.Convert.ToInt32(Z.Register[10]);
      int sum = Int32.Parse(X.Register[8].ToString());
      sum += System.Convert.ToInt32(Y.Register[10].ToString());
      sum += System.Convert.ToInt32(Z.Register[10].ToString());

      if (sum > 1)
        return '1';
      else
        return '0';
    }

    public string Crypt(string source)
    {

      string outStr = "";

      string tmp = string.Join("", Encoding.UTF8.GetBytes(source).Select(n => Convert.ToString(n, 2).PadLeft(8, '0'))); //pretvaranje tekstualnog stringa u string bitova

      foreach (char c in tmp)
      {
        int r = c - '0';
        char m = MajorityVote();

        if (X.Register[8] == m)
          RegisterSteps(X);

        if (Y.Register[10] == m)
          RegisterSteps(Y);

        if (Z.Register[10] == m)
          RegisterSteps(Z);

        char s = '0';
        s = XOR(XOR(XOR(s, X.Output), Y.Output), Z.Output);

        outStr += XOR(s, c);
      }
      ResetRegisters();
      return outStr;
    }
    public string BinaryToString(string data)
    {
      List<Byte> byteList = new List<byte>();

      for (int i = 0; i < data.Length; i += 8)
      {
        byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
      }
      string a = Encoding.ASCII.GetString(byteList.ToArray());
      return a;
    }

    public string DeCrypt(string source)
    {
      char[] sourceEls = source.ToCharArray();

      string outStr = "";

      foreach (char c in sourceEls)
      {
        char m = MajorityVote();

        if (X.Register[8] == m)
          RegisterSteps(X);

        if (Y.Register[10] == m)
          RegisterSteps(Y);

        if (Z.Register[10] == m)
          RegisterSteps(Z);

        char s = '0';
        s = XOR(XOR(XOR(s, X.Output), Y.Output), Z.Output);

        outStr += XOR(s, c);
      }
      ResetRegisters();
      return BinaryToString(outStr);
    }
  }
}
