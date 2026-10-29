using LR4__2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LR4__2
{
    public class BeautySingleton
    {
        private static BeautySalonDbContext? dbContext { get; set; }

        public static BeautySalonDbContext? GetDbContext()
        {
            if (dbContext == null) dbContext = new BeautySalonDbContext();

            return dbContext;
        }
    }
}
