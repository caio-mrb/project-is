using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Api.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }


        public bool isEqualTo(BaseModel other)
        {
            return (this.Id == other.Id
                && this.Name == other.Name
                && this.CreationDatetime == other.CreationDatetime);
        }

        public string GetDatabase()
        {
            PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));


            return "dbo." + pluralizationService.Pluralize(GetResType());
        }

        public abstract string GetResType();
    }
} 