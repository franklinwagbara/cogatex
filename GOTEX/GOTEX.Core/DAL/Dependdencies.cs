using System;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GOTEX.Core.DAL
{
    public static class Dependdencies
    {
        public static void ServiceInjections(this IServiceCollection services)
        {
            services.AddScoped<IAppConfiguration<Configuration>, AppConfiguration>();
            services.AddTransient<IElpsRepository, ElpsRepostory>();
            services.AddTransient<IRepository<Message>, MessageRepository>();
            services.AddTransient<IApplication<Application>, ApplicationRepository>();
            services.AddTransient<IAppHistory<ApplicationHistory>, AppHistoryRepository>();
            services.AddTransient<IPayment<PaymentApproval>, PaymentApprovalRepository>();
            services.AddTransient<IPermit<Permit>, PermitRepository>();
            services.AddTransient<IRepository<Log>, Logrepository>();
            services.AddTransient<IRepository<Product>, GasStreamsRepository>();
            services.AddTransient<IRepository<ApplicationType>, AppTypeRepository>();
            services.AddTransient<IRepository<WorkFlow>, WorkflowRepository>();
            services.AddTransient<IRepository<Quarter>, QuarterRepository>();
            services.AddTransient<IRepository<Facility>, FacilityRepository>();
            services.AddTransient<IApplicationTypeDocs<ApplicationTypeDocuments>, AppTypeDocsRepository>();
            services.AddTransient<IRepository<PaymentEvidence>, PaymentEvidenceRepository>();
            services.AddTransient<IRepository<Survey>, SurveyRepository>();
            services.AddTransient<IRepository<DeclarationForm>, DEclarationFormRepository>();
            services.AddTransient<IRepository<Leave>, LeaveRepository>();
            services.AddTransient<IRepository<LeaveRequest>, LeaveRequestRepository>();
        }
    }
}