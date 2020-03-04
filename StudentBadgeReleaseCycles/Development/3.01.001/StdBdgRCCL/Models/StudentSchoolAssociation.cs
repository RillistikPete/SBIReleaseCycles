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

        [JsonProperty("entryDate")]
        public string EntryDate { get; set; }

        [JsonProperty("calendarReference")]
        public CalendarReference CalendarReference { get; set; }

        [JsonProperty("classOfSchoolYearTypeReference")]
        public SchoolYearTypeReference ClassOfSchoolYearTypeReference { get; set; }

        [JsonProperty("graduationPlanReference")]
        public GraduationPlanReference GraduationPlanReference { get; set; }

        [JsonProperty("schoolReference")]
        public SchoolReference SchoolReference { get; set; }

        [JsonProperty("schoolYearTypeReference")]
        public SchoolYearTypeReference SchoolYearTypeReference { get; set; }

        [JsonProperty("studentReference")]
        public StudentReference StudentReference { get; set; }

        [JsonProperty("educationPlans")]
        public List<EducationPlan> EducationPlans { get; set; }

        [JsonProperty("employedWhileEnrolled")]
        public bool EmployedWhileEnrolled { get; set; }

        [JsonProperty("entryGradeLevelDescriptor")]
        public string EntryGradeLevelDescriptor { get; set; }

        [JsonProperty("entryGradeLevelReasonDescriptor")]
        public string EntryGradeLevelReasonDescriptor { get; set; }

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

        [JsonProperty("_etag")]
        public string Etag { get; set; }

    }

    public partial class Link
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public partial class CalendarReference
    {
        [JsonProperty("calendarCode")]
        public string CalendarCode { get; set; }

        [JsonProperty("schoolId")]
        public long SchoolId { get; set; }

        [JsonProperty("link")]
        public Link Link { get; set; }
    }

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

    public partial class SchoolYearTypeReference
    {
        [JsonProperty("schoolYear")]
        public long SchoolYear { get; set; }

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

        [JsonProperty("graduationPlanTypeDescriptor")]
        public string GraduationPlanTypeDescriptor { get; set; }
    }
}
