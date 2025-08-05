using Luban.Core.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Plugin.Common
{
    [PCmdGroup(Name = "common", Description = "公共命令组")]
    internal class CommonCommand
    {
        [PCmd(Name = "test", Description = "测试命令")]
        [return: PCmdRet(Name = "result", Description = "返回结果")]
        public string Test(
            [PCmdArg(Name = "A", Description = "整形")] int a,
            [PCmdArg(Name = "B", Description = "字符串")] string b,
            [PCmdArg(Name = "C", Description = "布尔")] bool c)
        {
            return $"A: {a}, B: {b}, C: {c}";
        }

        [PCmd(Name = "test2", Description = "测试命令2")]
        [return: PCmdRet(Name = "result2", Description = "返回结果2")]
        public Task<string> Test2(
            [PCmdArg(Name = "A2", Description = "整形")] int a,
            [PCmdArg(Name = "B2", Description = "字符串")] string b,
            [PCmdArg(Name = "C2", Description = "布尔")] bool c)
        {
            return Task.FromResult($"A2: {a}, B2: {b}, C2: {c}");
        }
    }
}