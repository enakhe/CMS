using CMS.Application;
using CMS.Domain;
using CMS.Infrastructure;
using CMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Presentation
{
    public class DependencyInjection
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=ENAKHE;Database=CMSDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"));
            // Domain
            DomainDependencyInjection.AddDomainServices(services);

            // Application
            ApplicationDependencyInjection.AddApplicationServices(services);

            // Presentation
            PresentationDependencyInjection.AddPresentationServices(services);

            // Infrastructure
            InfrastructureDependencyInjection.AddInfrastructureServices(services);
        }
    }
}
