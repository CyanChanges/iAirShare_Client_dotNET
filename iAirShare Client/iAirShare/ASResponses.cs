using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace iAirShare_Client.iAirShare;

public class ASResponse
{
    public JObject? data;
    public string? msg;
    public int status;
}

public struct ASDirResult
{
    public int total;
    public int next;
    public List<ASFile?> files;
}

public class ASDirResponse : ASResponse
{
    public new ASDirResult? data;
}