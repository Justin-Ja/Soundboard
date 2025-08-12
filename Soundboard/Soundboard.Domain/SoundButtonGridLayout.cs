#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundboard.Domain.DataAccess.Implementations;

public class SoundButtonGridLayout : DomainObject
{
    public string Name { get; set; }
    public List<SoundButton> SoundButtons { get; set; } = new();
}

