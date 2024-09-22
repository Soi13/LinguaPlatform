using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;




namespace LinguaPlatform.Models
{
    [Authorize]
    public class Signal : Hub
    {

    }
}