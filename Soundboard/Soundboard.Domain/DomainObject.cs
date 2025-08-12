using System;
using System.ComponentModel.DataAnnotations;

namespace Soundboard.Domain;

public class DomainObject
{
    [Key]
    public Guid Guid { get; set; }
}
