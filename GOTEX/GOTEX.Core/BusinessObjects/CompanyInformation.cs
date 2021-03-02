using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class CompanyInformation
    {
        public CompanyModel Company { get; set; }
        public RegisteredAddress RegisteredAddress { get; set; }
        public RegisteredAddress OperationalAddress { get; set; }
        public List<DocumentModel> DocumentModel { get; set; }
        public DirectorModel Director { get; set; }
    }
    public class CompanyModel
    {
        public string user_Id { get; set; }
        public string name { get; set; }
        public string business_Type { get; set; }
        public string registered_Address_Id { get; set; }
        public string operational_Address_Id { get; set; }
        public string affiliate { get; set; }
        public string nationality { get; set; }
        public string contact_FirstName { get; set; }
        public string contact_LastName { get; set; }
        public string contact_Phone { get; set; }
        public string year_Incorporated { get; set; }
        public string rC_Number { get; set; }
        public string tin_Number { get; set; }
        public string no_Staff { get; set; }
        public string no_Expatriate { get; set; }
        public string total_Asset { get; set; }
        public string yearly_Revenue { get; set; }
        public string accident { get; set; }
        public string accident_Report { get; set; }
        public string training_Program { get; set; }
        public string mission_Vision { get; set; }
        public string hse { get; set; }
        public string hseDoc { get; set; }
        public string date { get; set; }
        public string isCompleted { get; set; }
        public string elps_Id { get; set; }
        public int id { get; set; }
        public string CEOSignature { get; set; }
    }
    public class RegisteredAddress
    {
        public string address_1 { get; set; }
        public string address_2 { get; set; }
        public string city { get; set; }
        public int country_Id { get; set; }
        public int stateId { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string postal_code { get; set; }
        public string type { get; set; }
        public int id { get; set; }
    }
    public class DocumentModel
    {
        public string DocumentName { get; set; }
        public int DocId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public string Source { get; set; }
        public string UniqueId { get; set; }
        public int FileId { get; set; }
        public string DocumentTypeName { get; set; }
        public string BaseorTran { get; set; }
        public string IsMandatory { get; set; }
        public string UploadDocumentUrl { get; set; }

    }
    public class DirectorModel
    {
        public int Id { get; set; }
        public int Company_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Address_Id { get; set; }
        public string Telephone { get; set; }
        public int Nationality { get; set; }
        public RegisteredAddress Address { get; set; }
    }
    public class DocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Selected { get; set; }
        public bool ParentSelected { get; set; }
        public string Document_Name { get; set; }
        public string UniqueId { get; set; }
        public string Source { get; set; }
        public int CoyFileId { get; set; }
        public int Document_Id { get; set; }
    }
    public class CompanyDocument
    {
        public int Id { get; set; }
        public int Company_Id { get; set; }
        public int Document_Type_Id { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public DateTime Date_Modified { get; set; }
        public DateTime Date_Added { get; set; }
        public bool Status { get; set; }
        public bool Archived { get; set; }
        public string UniqueId { get; set; }
        public string Document_Name { get; set; }
        public string DocumentTypeName { get; set; }
        public int? Elps_Id { get; set; }
    }
}