using System;
using System.Collections.Generic;

namespace Win_Dev.Data.Interfaces
{
    interface ILinkedRepository : IDisposable
    {
        void CreateLink(Guid bindable1,Guid bindable2);
        void DeleteLink(Guid bindable1,Guid bindable2);

        IEquatable<Guid> FindAllLinksByGuid(Guid source,object toType);        

    }
}
