using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Helpers
{
    public class LogEvents
    {
        public const int GenerateItems = 1000;
        public const int ListItems = 1001;
        public const int GetItem = 1002;
        public const int InsertItem = 1003;
        public const int UpdateItem = 1004;
        public const int DeleteItem = 1005;

        public const int AccessAllowed = 2000;

        public const int TestItem = 3000;
        
        public const int GetItemNotFound = 4002;
        public const int InsertItemFaild = 4003;
        public const int UpdateItemNotFound = 4004;
        public const int DeleteItemFaild = 4005;

        public const int Forbiden = 4010;
        public const int InvalidToken = 5000;
    }
}
