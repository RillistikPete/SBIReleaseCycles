using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public partial class EdfiEnrollmentSchool
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("schoolId")]
        public int SchoolId { get; set; }

        [JsonProperty("nameOfInstitution")]
        public string NameOfInstitution { get; set; }

        [JsonProperty("shortNameOfInstitution")]
        public string ShortNameOfInstitution { get; set; }

        [JsonProperty("webSite")]
        public string WebSite { get; set; }

        [JsonProperty("schoolTypeDescriptor")]
        public string SchoolTypeDescriptor { get; set; }

        [JsonProperty("addresses")]
        public List<Address> Addresses { get; set; }

        [JsonProperty("educationOrganizationCategories")]
        public List<EducationOrganizationCategory> EducationOrganizationCategories { get; set; }

        [JsonProperty("identificationCodes")]
        public List<IdentificationCode> IdentificationCodes { get; set; }

        [JsonProperty("institutionTelephones")]
        public List<InstitutionTelephone> InstitutionTelephones { get; set; }

        [JsonProperty("schoolCategories")]
        public List<SchoolCategory> SchoolCategories { get; set; }

        [JsonProperty("gradeLevels")]
        public List<GradeLevel> GradeLevels { get; set; }

        [JsonProperty("localEducationAgency")]
        public LocalEducationAgency LocalEducationAgency { get; set; }
    }

    public partial class EducationOrganizationCategory
    {
        [JsonProperty("category")]
        public string Category { get; set; }
    }

    public partial class SchoolCategory
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class IdentificationCode
    {
        [JsonProperty("educationOrganizationIdentificationSystemDescriptor")]
        public string EducationOrganizationIdentificationSystemDescriptor { get; set; }

        [JsonProperty("educationOrganizationIdentificationSystemType")]
        public string EducationOrganizationIdentificationSystemType { get; set; }
    }

    public partial class GradeLevel
    {
    }

    public partial class InstitutionTelephone
    {
        [JsonProperty("institutionTelephoneNumberTypeDescriptor")]
        public string InstitutionTelephoneNumberTypeDescriptor { get; set; }
    }

    public partial class LocalEducationAgency
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("localEducationAgencyId")]
        public long LocalEducationAgencyId { get; set; }
    }
}
