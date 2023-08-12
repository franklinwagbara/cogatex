using AutoMapper;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.ViewModels
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<SurveyViewModel, Survey>().ReverseMap();
            CreateMap<ApplicationViewModel, Application>().ReverseMap();
            CreateMap<DFormViewModel, DeclarationForm>().ReverseMap();
        }
    }
}
