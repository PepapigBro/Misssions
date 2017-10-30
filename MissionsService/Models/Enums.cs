using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MissionsService.Models
{
    public enum State : byte
    {
        Waiting = 0,
        Changed = 1,
        Canceled = 2,
        Finished = 3
    }
}