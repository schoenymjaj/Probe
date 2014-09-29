using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Probe.Models
{

    public interface IEntity : IDateCreated, ICreatedBy, IDateUpdated, IUpdatedBy
    {

    }


    public interface IDateCreated
    {
        DateTime? DateCreated { get; set; }
    }

    public interface ICreatedBy
    {
        String CreatedBy { get; set; }
    }

    public interface IDateUpdated
    {
        DateTime? DateUpdated { get; set; }
    }

    public interface IUpdatedBy
    {
        String UpdatedBy { get; set; }
    }



}