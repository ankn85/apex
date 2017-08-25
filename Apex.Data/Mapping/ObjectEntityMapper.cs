using Apex.Data.Mapping.Accounts;
using Apex.Data.Mapping.Emails;
using Apex.Data.Mapping.Logs;
using Apex.Data.Mapping.Menus;
using Apex.Data.Mapping.Settings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Apex.Data.Mapping
{
    public sealed class ObjectEntityMapper : IEntityMapper
    {
        public IEnumerable<IEntityMap> Mappings
        {
            get
            {
                return new List<IEntityMap>()
                {
                    new LogMap(),
                    new SettingMap(),
                    new EmailAccountMap(),
                    new QueuedEmailMap(),
                    new MenuMap(),
                    new ApplicationRoleMenuMap(),

                    //new GroupMap(),
                    //new QTagMap(),
                    //new QuestionMap(),
                    //new AnswerMap(),
                    //new ExamPaperMap(),
                    //new QuestionExamPaperMap(),
                    //new StudentMap(),
                    //new ExamMap(),
                    //new StudentExamMap(),
                    //new StudentExamPaperResultMap()
                };
            }
        }

        public void MapEntities(ModelBuilder modelBuilder)
        {
            foreach (IEntityMap map in Mappings)
            {
                map.Map(modelBuilder);
            }
        }
    }
}
