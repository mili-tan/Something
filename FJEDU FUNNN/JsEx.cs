using MSScriptControl;

namespace Playground_Fjedu
{
    static class JsEx
    {
        // JsEx.ExecuteJScript(@"encrypt(""12345678"")", File.ReadAllText("encrypt.js")

        public static string ExecuteJScript(string sExpression, string sCode)
        {
            ScriptControl scriptControl = new ScriptControl {UseSafeSubset = true, Language = "JScript"};
            scriptControl.AddCode(sCode);

            string str = scriptControl.Eval(sExpression).ToString();
            return str;
        }

    }

}
