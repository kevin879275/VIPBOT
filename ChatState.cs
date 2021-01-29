using System.Collections.Generic;
namespace ChatState
{
  class ChatState
  {
    public string stage = "luis";
    public int step = 0;
    public Dictionary<string, string> userInputs;
    public string inputKeyNow;
  }

}

