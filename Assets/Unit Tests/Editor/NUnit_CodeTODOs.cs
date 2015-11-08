using System;
using NUnit.Framework;

[TestFixture()]
public class NUnit_CodeTODOs
{
    [Test()]
    public void Helper_AddQQQs()
    {
        CodeTODOsHelper.AddQQQs("Assets/TestScript.cs");
    }

    [Test()]
    public void Helper_FindAllScripts()
    {
        var scripts = CodeTODOsHelper.FindAllScripts();
        Assert.IsNotNull(scripts);
    }

    [Test()]
    public void Helper_GetQQQsFromAllScripts()
    {
        CodeTODOsHelper.GetQQQsFromAllScripts();
        Assert.IsNotNull(CodeTODOs.QQQs);
    }

    [Test()]
    public void Helper_GetQQQsFromScript()
    {
        CodeTODOsHelper.GetQQQsFromScript("Assets/TestScript.cs");
    }
}
