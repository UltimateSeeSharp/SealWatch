using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace UserManagement.ViewModels
{
    public class CompanyListViewModel
    {
        private readonly CompanyAccessService _companyAccessService = new();

        private List<CompanyListDto> _companyList;
        public List<CompanyListDto> CompanyList
        {
            get
            {
                if (_companyList == null || !_companyList.Any())
                {
                    _companyList = _companyAccessService.GetList();
                }
                return _companyList;
            }
        }
        public static List<CompanyListModel> GetListSCompanyListModels()
        {
            var listCompanyDto = new List<CompanyListDto>();
            var listCompanyModel = new List<CompanyListModel>();
            listCompanyModel.AddRange(listCompanyDto.Select(x => new CompanyListModel() { Name = x.Name, Id = x.Id, Synonym = x.Synonym }).ToList());
            return null;
        }

        private static List<CompanyListModel> ListDtoToListModel(List<CompanyListDto> inputList)
        {
            return inputList.Select(listItem => new CompanyListModel()
            {
                Id = listItem.Id,
                Name = listItem.Name,
                Synonym = listItem.Synonym
            }).ToList();
        }
    }
}

public class CompanyAccessService
{
    public List<CompanyListDto> GetList()
    {
        return null;
    }
}

public class CompanyListDto
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Synonym { get; set; }
}

public class CompanyListModel
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Synonym { get; set; }
}