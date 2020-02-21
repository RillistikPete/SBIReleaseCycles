using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public partial class StudentSchoolAssociation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("graduationPlanReference")]
        public GraduationPlanReference GraduationPlanReference { get; set; }

        [JsonProperty("schoolReference")]
        public SchoolReference SchoolReference { get; set; }

        [JsonProperty("studentReference")]
        public StudentReference StudentReference { get; set; }

        [JsonProperty("entryDate")]
        public string EntryDate { get; set; }

        [JsonProperty("entryGradeLevelDescriptor")]
        public string EntryGradeLevelDescriptor { get; set; }

        [JsonProperty("entryTypeDescriptor")]
        public string EntryTypeDescriptor { get; set; }

        [JsonProperty("exitWithdrawDate")]
        public string ExitWithdrawDate { get; set; }

        [JsonProperty("exitWithdrawTypeDescriptor")]
        public string ExitWithdrawTypeDescriptor { get; set; }

        [JsonProperty("primarySchool")]
        public bool PrimarySchool { get; set; }

        [JsonProperty("repeatGradeIndicator")]
        public bool RepeatGradeIndicator { get; set; }

        [JsonProperty("residencyStatusDescriptor")]
        public string ResidencyStatusDescriptor { get; set; }

        [JsonProperty("schoolChoiceTransfer")]
        public bool SchoolChoiceTransfer { get; set; }

        [JsonProperty("educationPlans")]
        public IList<EducationPlan> EducationPlans { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }

    }

    public partial class Link { }

    public partial class EducationPlan
    {
        [JsonProperty("educationPlanType")]
        public string EducationPlanType { get; set; }
    }

    public partial class StudentReference
    {
        [JsonProperty("studentUniqueId")]
        public string StudentUniqueId { get; set; }

        [JsonProperty("link")]
        public Link Link { get; set; }
    }

    public partial class SchoolReference
    {
        [JsonProperty("schoolId")]
        public int SchoolId { get; set; }

        [JsonProperty("link")]
        public Link Link { get; set; }
    }

    public partial class GraduationPlanReference
    {
        [JsonProperty("typeDescriptor")]
        public string TypeDescriptor { get; set; }

        [JsonProperty("educationOrganizationId")]
        public int EducationOrganizationId { get; set; }

        [JsonProperty("graduationSchoolYear")]
        public int GraduationSchoolYear { get; set; }

        [JsonProperty("link")]
        public Link Link { get; set; }

        //V3 SSA New property

        [JsonProperty("graduationPlanTypeDescriptor")]
        public string GraduationPlanTypeDescriptor { get; set; }
    }
}
