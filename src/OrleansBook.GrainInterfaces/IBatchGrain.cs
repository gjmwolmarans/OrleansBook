using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansBook.GrainInterfaces;

public interface IBatchGrain : IGrainWithIntegerKey
{
    [Transaction(TransactionOption.Create)]
    Task AddInstructions((string, string)[] values);
}
