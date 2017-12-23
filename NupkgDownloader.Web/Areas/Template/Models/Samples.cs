using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Areas.Template.Models
{
    public class Samples
    {
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

        public List<Question> Questions { get; }
        public HomeViewModel HomeViewModel { get; }
    }
}
