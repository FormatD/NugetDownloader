using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Areas.Template.Models
{
    public class Samples
    {
        private static Random _random = new Random();
        public static Samples Instance = new Samples();

        private Samples()
        {
            Questions = new List<Question>
                {
                    new Question
                    {
                        Id=1,
                        Creator = new User{ Name ="限薪",Id =1 },
                        Tag ="动态",
                        Subject = "基于 layui 的极简社区页面模版1",
                        ViewCount = 198765,
                        CommentCount = 72,
                        IsFinished = true,
                    },
                    new Question
                    {
                        Id=2,
                        Creator = new User{ Name ="限薪",Id =1 },
                        Tag ="公告",
                        Subject = "基于 layui 的极简社区页面模版2",
                        ViewCount = 198765,
                        CommentCount = 72,
                        IsFinished = true,
                    },
                    new Question
                    {
                        Id=3,
                        Creator = new User{ Name ="限薪",Id =1 },
                        Tag ="动态",
                        Subject = "基于 layui 的极简社区页面模版33",
                        ViewCount = 198765,
                        CommentCount = 72,
                        IsFinished = true,
                    },
                    new Question
                    {
                        Id=4,
                        Creator = new User{ Name ="限薪",Id =2,IsVip =true,VipLevel=3 },
                        Tag ="动态",
                        Subject = "基于 layui 的极简社区页面模版33",
                        ViewCount = 198765,
                        CommentCount = 72,
                        IsFinished = true,
                    },
                };

            for (int i = 0; i < 60; i++)
            {
                QuestionDetails.Add(new QuestionDetail
                {
                    Id = i,
                    Creator = new User { Name = "限薪", Id = 2, IsVip = true, VipLevel = 3 },
                    Tag = "动态",
                    Subject = "基于 layui 的极简社区页面模版" + i.ToString(),
                    ViewCount = _random.Next(100, 1000000),
                    CommentCount = _random.Next(0, 20),
                    IsFinished = true,
                    Content = @"<p>
                        该模版由 layui官方社区（<a href=""http://fly.layui.com/"" target=""_blank"">fly.layui.com</a>）倾情提供，只为表明我们对 layui 执着的信念、以及对未来持续加强的承诺。该模版基于 layui 搭建而成，可作为极简通用型社区的页面支撑。
                      </p>
                      <p>更新日志：</p>
                    <pre>
                    # v3.0 2017-11-30
                    * 采用 layui 2.2.3 作为 UI 支撑
                    * 全面同步最新的 Fly 社区风格，各种细节得到大幅优化
                    * 更友好的响应式适配能力
                    </pre>
                      下载<hr>
                      <p>
                        官网：<a href=""http://www.layui.com/template/fly/"" target= ""_blank"" > http://www.layui.com/template/fly/</a><br>
                        码云：<a href= ""https://gitee.com/sentsin/fly/"" target =""_blank"" > https://gitee.com/sentsin/fly/</a><br>
                        GitHub：<a href=""https://github.com/layui/fly"" target=""_blank"" >https://github.com/layui/fly</a>
                      </p>
                      封面<hr>
                      <p>
                        <img src=""~/res/images/fly.jpg"" alt = ""Fly社区"" >
                      </p>",
                });
            }

            for (int i = 0; i < 50; i++)
            {
                Questions.Add(new Question
                {
                    Id = 10 + i,
                    Creator = new User { Name = "限薪", Id = 2, IsVip = true, VipLevel = 3 },
                    Tag = "动态",
                    Subject = "基于 layui 的极简社区页面模版33",
                    ViewCount = 198765,
                    CommentCount = 72,
                    IsFinished = true,
                });
            }

            HomeViewModel = new HomeViewModel
            {
                TopQuestions = Questions.Take(5),
                MostViewedQuestions = Questions.Take(5),
            };
        }

        public List<Question> Questions { get; } = new List<Question>();

        public List<QuestionDetail> QuestionDetails { get; } = new List<QuestionDetail>();

        public HomeViewModel HomeViewModel { get; }
    }
}
