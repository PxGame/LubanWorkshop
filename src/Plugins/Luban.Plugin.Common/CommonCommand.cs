using Luban.Core.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Plugin.Common
{
    [PCmdGroup(Name = "common", Description = "公共命令组")]
    public class CommonCommand
    {
        public class ResultData
        {
            public int a3;
            public string b3;
            public bool c3;
        }

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
        public async Task<ResultData> Test2(
             int a,
            [PCmdArg(Description = "字符串")] string b,
            [PCmdArg(Name = "C2", Description = "布尔")] bool c)
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Wait {i}");
                await Task.Delay(1000);
            }

            //throw new Exception("测试异常");
            return await Task.FromResult(new ResultData()
            {
                a3 = a,
                b3 = b,
                c3 = c
            });
        }
    }
}