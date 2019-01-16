using System;
using System.IO;
using Nancy;

namespace NancyTest.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = p => "Welcome to Once Data Storage API";

            Get["/get/{gid}"] = p => File.Exists($"./data/{p.gid}/storage.t") ? File.ReadAllLines($"./data/{p.gid}/storage.t")[0] : "not found";

            Get["/set/{gid}/{mContext}"] = p =>
            {
                if (!Directory.Exists($"./data/{p.gid}"))
                {
                    Directory.CreateDirectory($"./data/{p.gid}");
                    if (!File.Exists($"./data/{p.gid}/storage.t"))
                        File.Create($"./data/{p.gid}/storage.t").Close();
                }
                File.WriteAllText($"./data/{p.gid}/storage.t",
                    p.mContext.ToString() + Environment.NewLine + File.ReadAllText($"./data/{p.gid}/storage.t"));
                return "OK";
            };
        }
    }
}
