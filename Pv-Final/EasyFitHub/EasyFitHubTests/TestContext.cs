using EasyFitHub.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    internal class TestContext : EasyFitHubContext
    {
        public TestContext(DbContextOptions<EasyFitHubContext> options) : base(options)
        { 
        }
    }
    
}
